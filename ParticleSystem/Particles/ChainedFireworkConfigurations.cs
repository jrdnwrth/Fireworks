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
        // Stage 1: Rocket trail emitter
        var rocketEmitter = new Emitter(
            posX: 350f,                    // Center of screen horizontally
            posY: 800f,                    // Near bottom of screen
            velX: 30f,                      // No horizontal velocity
            velY: -500f,                   // Strong upward velocity
            lifetime: 1.7f,                // Short lifetime - just the rocket trail
            emissionRate: 150f,            // Emit trail particles
            particleColor: FireworkColors.Gold,
            minParticleLifetime: 0.6f,
            maxParticleLifetime: 0.8f,
            randomVelocityMagnitude: 50f,
            minParticleDrag: 0.85f,
            maxParticleDrag: 0.9f,
            particleType: ParticleType.Flicker,
            onComplete: CreateBurstExplosion);  // Chain to burst explosion

        EmitterManager.AddEmitter(rocketEmitter);
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

            var burstEmitter = new Emitter(
                posX: posX,
                posY: posY,
                velX: burstVelX,
                velY: burstVelY,
                lifetime: 1.0f,            // Medium lifetime for burst
                emissionRate: 20f,
                particleColor: FireworkColors.BrightBlue,
                minParticleLifetime: 0.05f,
                maxParticleLifetime: 0.2f,
                randomVelocityMagnitude: 100f,
                minParticleDrag: 0.75f,
                maxParticleDrag: 0.85f,
                particleType: ParticleType.Decay,
                onComplete: CreateSparkleEffect);  // Chain to sparkle effects

            EmitterManager.AddEmitter(burstEmitter);
        }
    }

    /// <summary>
    /// Stage 3: Creates sparkle effects at each burst emitter's final position
    /// </summary>
    private static void CreateSparkleEffect(float posX, float posY, float velX, float velY)
    {
        // Create a final sparkle burst
        var sparkleEmitter = new Emitter(
            posX: posX,
            posY: posY,
            velX: velX ,    // Inherit some of the parent velocity
            velY: velY,
            lifetime: 0.1f,       // Short sparkle burst
            emissionRate: 1000f,   // High emission rate for intense sparkle
            particleColor: FireworkColors.Gold,
            minParticleLifetime: 0.0f,
            maxParticleLifetime: 0.5f,
            randomVelocityMagnitude: 850f,
            minParticleDrag: 0.8f,
            maxParticleDrag: 0.9f,
            particleType: ParticleType.Flash);

        // Add sparkle emitter to the global EmitterManager
        EmitterManager.AddEmitter(sparkleEmitter);
    }
}