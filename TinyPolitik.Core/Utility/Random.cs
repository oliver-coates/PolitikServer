namespace PolitikServer.Core;

/// <summary>
/// Utility class to help with randomness.
/// </summary>
public static class RandomUtil
{
    private static Random R;

    static RandomUtil()
    {
        R = new Random();
    }
    
    public enum VarianceMethod
    {
        Additive,
        Multiplicative
    }


    /// <summary>
    /// Varies a base value by a provided variance multiplier and method.
    /// E.g A base value of 10, with a variance of 2 will generate values between 8 - 12 if additive,
    /// and a base value of 10, with a variance of 0.1 will generate values between 9 - 11 if multiplicative
    /// </summary>
    public static float ApplyVariance(float baseValue, float variance, VarianceMethod method = VarianceMethod.Multiplicative)
    {
        switch (method)
        {
            case (VarianceMethod.Additive):
                return baseValue + Range(-variance, variance);
        
            case (VarianceMethod.Multiplicative):
                return baseValue * (1 + Range(-variance, variance));

            default:
                throw new Exception("Unhandled Variance Method.");
        }
    }

    /// <summary>
    /// Get a random float between two provided float values (inclusive).
    /// </summary>
    public static float Range(float min, float max)
    {
        float rng = (float) R.NextDouble();
        float diff = max - min;
        return min + (diff * rng);
    }

    /// <summary>
    /// Get a random int between two provided integer values (inclusive).
    /// </summary>
    public static int Range(int min, int max)
    {
        float rng = (float) R.NextDouble();
        float diff = max - min;
        return min + (int)(diff * rng);
    }

    public static T Pick<T>(IList<T> list)
    {
        return list[Range(0, list.Count-1)];
    }
}