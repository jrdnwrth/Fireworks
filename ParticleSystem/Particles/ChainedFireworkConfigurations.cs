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
            posX: 640f,                    // Center of screen horizontally
            posY: 600f,                    // Near bottom of screen
            velX: -30f,                      // No horizontal velocity
            velY: -400f,                   // Strong upward velocity
            lifetime: 1.5f,                // Short lifetime - just the rocket trail
            emissionRate: 150f,            // Emit trail particles
            particleColor: FireworkColors.Gold,
            minParticleLifetime: 0.3f,
            maxParticleLifetime: 0.8f,
            randomVelocityMagnitude: 50f,
            screenWidth: screenWidth,
            screenHeight: screenHeight,
            minParticleDrag: 0.85f,
            maxParticleDrag: 0.9f,
            onComplete: CreateBurstExplosion);  // Chain to burst explosion

        EmitterManager.AddEmitter(rocketEmitter);
    }

    /// <summary>
    /// Stage 2: Creates the main burst explosion at the rocket's final position
    /// </summary>
    private static void CreateBurstExplosion(float posX, float posY, float velX, float velY)
    {
        for (int i = 0; i < 8; i++)
        {
            var random = new Random();
            float angle = (float)(random.NextDouble() * System.Math.PI * 2); // Random angle 0 to 2π
            float burstVelX = (float)(System.Math.Cos(angle) * 100f) + velX;  // Inherit parent velocity
            float burstVelY = (float)(System.Math.Sin(angle) * 100f) + velY;  // Inherit parent velocity

            var burstEmitter = new Emitter(
                posX: posX,
                posY: posY,
                velX: burstVelX,
                velY: burstVelY,
                lifetime: 1.3f,            // Medium lifetime for burst
                emissionRate: 200f,
                particleColor: i % 2 == 0 ? FireworkColors.Gold : FireworkColors.Gold,
                minParticleLifetime: 0.1f,
                maxParticleLifetime: 1.0f,
                randomVelocityMagnitude: 100f,
                screenWidth: 1280,         // You might want to pass these as parameters
                screenHeight: 720,
                minParticleDrag: 0.75f,
                maxParticleDrag: 0.85f,
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
            emissionRate: 2500f,   // High emission rate for intense sparkle
            particleColor: FireworkColors.Gold,
            minParticleLifetime: 0.0f,
            maxParticleLifetime: 0.7f,
            randomVelocityMagnitude: 1050f,
            screenWidth: 1280,
            screenHeight: 720,
            minParticleDrag: 0.8f,
            maxParticleDrag: 0.9f);

        // Add sparkle emitter to the global EmitterManager
        EmitterManager.AddEmitter(sparkleEmitter);
    }
}