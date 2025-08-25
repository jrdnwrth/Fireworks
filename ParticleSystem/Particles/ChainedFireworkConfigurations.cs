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
    public static void CreateChainedFirework(int screenWidth, int screenHeight)
    {
        // Get an emitter from the pool instead of creating new one
        var e = EmitterManager.GetEmitter();
        if (e != null)
        {
            // Set all the properties directly
            e.PosX = 350f;                    // Center of screen horizontally
            e.PosY = 800f;                    // Near bottom of screen
            e.VelX = 30f;                     // No horizontal velocity
            e.VelY = -500f;                   // Strong upward velocity
            e.Lifetime = 1.7f;                // Short lifetime - just the rocket trail
            e.EmissionTimer = 0f;
            e.EmissionRate = 150f;            // Emit trail particles
            e.ParticleColor = FireworkColors.Gold;
            e.MinParticleLifetime = 0.6f;
            e.MaxParticleLifetime = 0.8f;
            e.RandomVelocityMagnitude = 50f;
            e.MinParticleDrag = 0.85f;
            e.MaxParticleDrag = 0.9f;
            e.ParticleType = ParticleType.Flicker;
            e.OnComplete = CreateBurstExplosion;  // Chain to burst explosion
            e.CallbackInvoked = false;
            e.HasSpareNormal = false;
            e.SpareNormal = 0f;

            // The emitter is already in the pool, no need to add it
        }
    }

    /// <summary>
    /// Stage 2: Creates the main burst explosion at the rocket's final position
    /// </summary>
    private static void CreateBurstExplosion(float posX, float posY, float velX, float velY)
    {
        int count = 20;
        for (int i = 0; i < count; i++)
        {
            float angle = (float)(Math.PI * 2f / count * i); // Random angle 0 to 2π
            var rand = random.NextDouble();
            var circular_scaler = Math.Sqrt( 1 - rand * rand);
            float burstVelX = (float)(Math.Cos(angle) * 160f * circular_scaler) + velX;  // Inherit parent velocity
            float burstVelY = (float)(Math.Sin(angle) * 140f * circular_scaler) + velY;  // Inherit parent velocity

            var e = EmitterManager.GetEmitter();
            if (e != null)
            {
                e.PosX = posX;
                e.PosY = posY;
                e.VelX = burstVelX;
                e.VelY = burstVelY;
                e.Lifetime = 1.0f;            // Medium lifetime for burst
                e.EmissionTimer = 0f;
                e.EmissionRate = 20f;
                e.ParticleColor = FireworkColors.BrightBlue;
                e.MinParticleLifetime = 0.05f;
                e.MaxParticleLifetime = 0.2f;
                e.RandomVelocityMagnitude = 100f;
                e.MinParticleDrag = 0.75f;
                e.MaxParticleDrag = 0.85f;
                e.ParticleType = ParticleType.Decay;
                e.OnComplete = CreateSparkleEffect;  // Chain to sparkle effects
                e.CallbackInvoked = false;
                e.HasSpareNormal = false;
                e.SpareNormal = 0f;
            }
        }
    }

    /// <summary>
    /// Stage 3: Creates sparkle effects at each burst emitter's final position
    /// </summary>
    private static void CreateSparkleEffect(float posX, float posY, float velX, float velY)
    {
        // Create a final sparkle burst
        var e = EmitterManager.GetEmitter();
        if (e != null)
        {
            e.PosX = posX;
            e.PosY = posY;
            e.VelX = velX;    // Inherit some of the parent velocity
            e.VelY = velY;
            e.Lifetime = 0.1f;       // Short sparkle burst
            e.EmissionTimer = 0f;
            e.EmissionRate = 1000f;   // High emission rate for intense sparkle
            e.ParticleColor = FireworkColors.Gold;
            e.MinParticleLifetime = 0.0f;
            e.MaxParticleLifetime = 0.5f;
            e.RandomVelocityMagnitude = 850f;
            e.MinParticleDrag = 0.8f;
            e.MaxParticleDrag = 0.9f;
            e.ParticleType = ParticleType.Flash;
            e.CallbackInvoked = false;
            e.HasSpareNormal = false;
            e.SpareNormal = 0f;
        }
    }
}