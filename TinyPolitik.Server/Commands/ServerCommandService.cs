namespace PolitikServer.Core;

public class ServerCommandService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IHostApplicationLifetime _lifeTime;
    private readonly Dictionary<string, Dictionary<string, Func<string[], Task>>> _commands = new(StringComparer.OrdinalIgnoreCase);

    public ServerCommandService(IServiceProvider services, IHostApplicationLifetime lifetime)
    {
        _services = services;
        _lifeTime = lifetime;
        RegisterCommands();
    }

    private void RegisterCommands()
    {
        Register("turn", "listTimes", _ =>
        {
            var config = _services.GetRequiredService<GameConfig>();
            Console.WriteLine($"Turns are scheduled for {string.Join(',', config.TurnTimesLocal)} daily (local time).");   
            return Task.CompletedTask;
        });

        Register("turn", "getNext", _ =>
        {
           var turnManager = _services.GetRequiredService<TurnManager>();
           Console.WriteLine($"The next turn is scheduled for: {turnManager.NextTurnTime}"); 
           return Task.CompletedTask;
        });

        Register("turn", "forceAdvance", async _ =>
        {
            var turnProcessor = _services.GetRequiredService<TurnProcessor>();
            Console.WriteLine($"Forcing turn advance...");
            await turnProcessor.ProcessTurnAsync();
            Console.WriteLine("Done."); 
        });

        Register("server", "shutdown", _ =>
        {
            Console.WriteLine("Goodbye!, Thanks for playing.");
            _lifeTime.StopApplication();
            return Task.CompletedTask;
        });
    }

    private void Register(string ns, string verb, Func<string[], Task> handler)
    {
        if (!_commands.TryGetValue(ns, out var verbs))
        {
            _commands[ns] = verbs = new (StringComparer.OrdinalIgnoreCase);
        }

        verbs[verb] = handler;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var line = await Task.Run(Console.ReadLine, stoppingToken);

            if (string.IsNullOrWhiteSpace(line)) { continue; }

            // Parse read line:
            var parts = line.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string ns = parts[0];
            string? verb = null;
            if (parts.Length > 1)
            {
                verb = parts[1];
            }
            string[] args = parts.Skip(2).ToArray();

            // Pull event handler and call it with the provided arguments:
            if (verb is null || !_commands.TryGetValue(ns, out var verbs) || !verbs.TryGetValue(verb, out Func<string[], Task>? handler))
            {
                Console.WriteLine($"Unknown command: '{line}'\nGo to www.TinyPolitik.net/serverGuide for a list of commands");
                continue;
            }

            try
            {
                await handler(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Command failed: {ex.Message}");
            }
        }
    }
}