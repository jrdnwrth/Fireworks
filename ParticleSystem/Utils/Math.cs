using System;

namespace ParticleSystem.Utils;

public static class MathUtils
{
    /// <summary>
    /// Generates a normally distributed random number using the Box-Muller transform.
    /// Maintains state for spare normal value.
    /// </summary>
    public static float NextGaussian(Random random, ref bool hasSpareNormal, ref float spareNormal)
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

        float magnitude = (float)(System.Math.Sqrt(-2.0 * System.Math.Log(u1)));
        float angle = (float)(2.0 * System.Math.PI * u2);

        spareNormal = magnitude * (float)System.Math.Sin(angle);
        return magnitude * (float)System.Math.Cos(angle);
    }
}