using ParticleSystem.Utils;
using System;
using System.Collections.Generic;

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
        for (int i = 0; i < 18; i++)
        {
            var random = new Random();
            float angle = (float)(random.NextDouble() * Math.PI * 2); // Random angle 0 to 2π
            float burstVelX = (float)(Math.Cos(angle) * 160f * Math.Cos(random.NextDouble() * 32)) + velX;  // Inherit parent velocity
            float burstVelY = (float)(Math.Sin(angle) * 140f * Math.Cos(random.NextDouble() * 32)) + velY;  // Inherit parent velocity

            var burstEmitter = new Emitter(
                posX: posX,
                posY: posY,
                velX: burstVelX,
                velY: burstVelY,
                lifetime: 1.0f,            // Medium lifetime for burst
                emissionRate: 80f,
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