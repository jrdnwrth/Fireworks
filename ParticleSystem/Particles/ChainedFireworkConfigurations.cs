using ParticleSystem.Utils;
using System;
using static ParticleSystem.Utils.MathUtils;

namespace ParticleSystem.Particles;

public static class ChainedFireworkConfigurations
{
    /// <summary>
    /// Creates a multi-stage firework that demonstrates effect chaining
    /// Stage 1: Single upward-shooting emitter (rocket trail)
    /// Stage 2: Burst into multiple emitters at peak (main explosion)
    /// Stage 3: Secondary sparkle effects from each burst emitter
    /// </summary>
    public static void CreateChainedFirework()
    {
        // Get an emitter from the pool instead of creating new one
        var e = EmitterManager.GetEmitter();

        // Set all the properties directly
        e.PosX = 350f;                    // Center of screen horizontally
        e.PosY = 800f;                    // Near bottom of screen
        e.VelX = 30f;                     // No horizontal velocity
        e.VelY = -500f;                   // Strong upward velocity
        e.SparkleTrail();                 // Apply preset for rocket trail
        e.OnComplete = CreateBurstExplosion;  // Chain to burst explosion
    }

    /// <summary>
    /// Stage 2: Creates the main burst explosion at the rocket's final position
    /// </summary>
    private static void CreateBurstExplosion(float posX, float posY, float velX, float velY)
    {
        int total = 20;
        for (int i = 0; i < total; i++)
        {
            var e = EmitterManager.GetEmitter();
            e.PosX = posX;
            e.PosY = posY;
            e.SetVelocity_RoundBurst(velX, velY, i, total, 160f, 140f);
            e.Star_1(FireworkColors.BrightBlue);
            e.OnComplete = CreateSparkleEffect;  // Chain to sparkle effects
        }
    }

    /// <summary>
    /// Stage 3: Creates sparkle effects at each burst emitter's final position
    /// </summary>
    private static void CreateSparkleEffect(float posX, float posY, float velX, float velY)
    {
        // Create a final sparkle burst
        var e = EmitterManager.GetEmitter();
        e.PosX = posX;
        e.PosY = posY;
        e.VelX = velX;    // Inherit some of the parent velocity
        e.VelY = velY;
        e.GlitterPop();  // Apply preset for glitter pop
    }
}