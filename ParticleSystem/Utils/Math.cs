using System;

namespace ParticleSystem.Utils;

public static class MathUtils
{
    private static Random? _random = null;
    public static Random random
    {
        get
        {
            if (_random is null)
                _random = new Random();
            return _random;
        }
    }

    /// <summary>
    /// Generates a normally distributed random number using the Box-Muller transform.
    /// Maintains state for spare normal value.
    /// </summary>
    public static float NextGaussian(ref bool hasSpareNormal, ref float spareNormal)
    {
        if (hasSpareNormal)
        {
            hasSpareNormal = false;
            return spareNormal;
        }

        hasSpareNormal = true;

        float u1, u2;
        do
        {
            u1 = (float)random.NextDouble();
            u2 = (float)random.NextDouble();
        }
        while (u1 <= float.Epsilon);

        float magnitude = (float)(Math.Sqrt(-2.0 * Math.Log(u1)));
        float angle = (float)(2.0 * Math.PI * u2);

        spareNormal = magnitude * (float)Math.Sin(angle);
        return magnitude * (float)Math.Cos(angle);
    }
}