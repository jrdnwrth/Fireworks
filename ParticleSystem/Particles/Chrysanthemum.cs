using ParticleSystem.Utils;
using System;
using static ParticleSystem.Utils.MathUtils;

namespace ParticleSystem.Particles;

public static partial class Chrysanthemum
{
    /// <summary>
    /// Creates a multi-stage firework that demonstrates effect chaining
    /// Stage 1: Single upward-shooting emitter (rocket trail)
    /// Stage 2: Burst into multiple emitters at peak (main explosion)
    /// Stage 3: Secondary sparkle effects from each burst emitter
    /// </summary>
    public static void Create(int color, float xPos, float yPos, bool sparkle_finish)
    {
        // Get an emitter from the pool instead of creating new one
        var e = EmitterManager.GetEmitter();

        // Set position and velocity using tuple structs
        e.Pos = new Position(xPos, yPos);    // Center horizontally, near bottom
        e.Vel = new Velocity(random_float(-60f, 60f), -500f + random_float(-60f, 60f));    // Slight horizontal, strong upward velocity
        e.SparkleTrail();                    // Apply preset for rocket trail
        e.initial_lifetime = random_float(1.7f, 2.5f);
        e.OnComplete = (pos, vel) => CreateBurstExplosion(pos, vel, color, sparkle_finish); // Chain to burst explosion
    }

    /// <summary>
    /// Stage 2: Creates the main burst explosion at the rocket's final position
    /// </summary>
    private static void CreateBurstExplosion(Position pos, Velocity vel, int color, bool sparkle_finish)
    {
        int total = (int)(random_float(30, 50));
        for (int i = 0; i < total; i++)
        {
            var e = EmitterManager.GetEmitter();
            e.Pos = pos;
            e.SetVelocity_RoundBurst(vel, i, total, 160f, 140f);
            e.Star_1(color);

            if (sparkle_finish) 
                e.OnComplete = CreateSparkleEffect;  // Chain to sparkle effects
        }
    }

    /// <summary>
    /// Stage 3: Creates sparkle effects at each burst emitter's final position
    /// </summary>
    private static void CreateSparkleEffect(Position pos, Velocity vel)
    {
        // Create a final sparkle burst
        var e = EmitterManager.GetEmitter();
        e.Pos = pos;
        e.Vel = vel;    // Inherit some of the parent velocity
        e.GlitterPop(); // Apply preset for glitter pop
        e.EmissionRate = 400f;
    }
}