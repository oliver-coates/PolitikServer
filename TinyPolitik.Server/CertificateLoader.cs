using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace TinyPolitik.Core;

public static class CertificateLoader
{
    private static X509Certificate2? _Cert;

    public static void Setup(WebApplicationBuilder builder, GameConfig config)
    {
        var certPath = Path.Combine(builder.Environment.ContentRootPath, "server.pfx");
        _Cert = SelfSignedCertHelper.GetOrCreate(certPath, password: "banana");

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(config.Port, listenOptions => listenOptions.UseHttps(_Cert));
        });
    }

    public static void NotifyInConsole()
    {
        if (_Cert == null)
        {
            throw new Exception("Certificate cannot be null!");
        }

        // Print the fingerprint prominently on startup — the host needs this to share with friends.
        var fingerprint = Convert.ToHexString(SHA256.HashData(_Cert.RawData)).ToLowerInvariant();
        Console.WriteLine($"\n!!! - Certificate: - !!!: {fingerprint}");
        Console.WriteLine($"\nServer certificate fingerprint: {fingerprint}");
        Console.WriteLine("Share this fingerprint with friends alongside the password so they can verify it on first connect.\n");
    }
}