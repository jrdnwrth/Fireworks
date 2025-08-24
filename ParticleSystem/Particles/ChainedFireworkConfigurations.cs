using ParticleSystem.Utils;
using System.Collections.Generic;

namespace ParticleSystem.Particles
{
    public static class ChainedFireworkConfigurations
    {
        /// <summary>
        /// Creates a multi-stage firework that demonstrates effect chaining
        /// Stage 1: Single upward-shooting emitter (rocket trail)
        /// Stage 2: Burst into multiple emitters at peak (main explosion)
        /// Stage 3: Secondary sparkle effects from each burst emitter
        /// </summary>
        public static List<Emitter> CreateChainedFirework(int screenWidth, int screenHeight)
        {
            var emitters = new List<Emitter>();

            // Stage 1: Rocket trail emitter
            var rocketEmitter = new Emitter(
                posX: 640f,                    // Center of screen horizontally
                posY: 600f,                    // Near bottom of screen
                velX: 0f,                      // No horizontal velocity
                velY: -450f,                   // Strong upward velocity
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

            emitters.Add(rocketEmitter);
            return emitters;
        }

        /// <summary>
        /// Stage 2: Creates the main burst explosion at the rocket's final position
        /// </summary>
        private static void CreateBurstExplosion(float posX, float posY, float velX, float velY, ParticleManager particleManager)
        {
            // Create multiple emitters spreading out from the explosion point
            var explosionEmitters = new List<Emitter>();
            
            for (int i = 0; i < 8; i++)
            {
                float angle = (float)(i * System.Math.PI * 2 / 8); // 8 directions
                float burstVelX = (float)(System.Math.Cos(angle) * 300f);
                float burstVelY = (float)(System.Math.Sin(angle) * 300f);

                var burstEmitter = new Emitter(
                    posX: posX,
                    posY: posY,
                    velX: burstVelX,
                    velY: burstVelY,
                    lifetime: 0.8f,            // Medium lifetime for burst
                    emissionRate: 400f,
                    particleColor: i % 2 == 0 ? FireworkColors.BrightRed : FireworkColors.BrightBlue,
                    minParticleLifetime: 0.5f,
                    maxParticleLifetime: 2.0f,
                    randomVelocityMagnitude: 100f,
                    screenWidth: 1280,         // You might want to pass these as parameters
                    screenHeight: 720,
                    minParticleDrag: 0.75f,
                    maxParticleDrag: 0.85f,
                    onComplete: CreateSparkleEffect);  // Chain to sparkle effects

                explosionEmitters.Add(burstEmitter);
            }

            // Add burst emitters to a global list or queue (you'll need to implement this)
            // For now, this demonstrates the concept
        }

        /// <summary>
        /// Stage 3: Creates sparkle effects at each burst emitter's final position
        /// </summary>
        private static void CreateSparkleEffect(float posX, float posY, float velX, float velY, ParticleManager particleManager)
        {
            // Create a final sparkle burst
            var sparkleEmitter = new Emitter(
                posX: posX,
                posY: posY,
                velX: velX * 0.2f,    // Inherit some of the parent velocity
                velY: velY * 0.2f,
                lifetime: 0.3f,       // Short sparkle burst
                emissionRate: 800f,   // High emission rate for intense sparkle
                particleColor: FireworkColors.Gold,
                minParticleLifetime: 0.1f,
                maxParticleLifetime: 0.5f,
                randomVelocityMagnitude: 150f,
                screenWidth: 1280,
                screenHeight: 720,
                minParticleDrag: 0.8f,
                maxParticleDrag: 0.9f);

            // Add sparkle emitter to global list (implementation needed)
        }

        /// <summary>
        /// Creates a simple two-stage firework for testing
        /// </summary>
        public static List<Emitter> CreateSimpleChainedFirework(int screenWidth, int screenHeight, float startX)
        {
            var emitters = new List<Emitter>();

            var primaryEmitter = new Emitter(
                posX: startX,
                posY: 600f,
                velX: 0f,
                velY: -350f,
                lifetime: 2f,
                emissionRate: 100f,
                particleColor: FireworkColors.Gold,
                minParticleLifetime: 0.2f,
                maxParticleLifetime: 0.6f,
                randomVelocityMagnitude: 40f,
                screenWidth: screenWidth,
                screenHeight: screenHeight,
                onComplete: (x, y, vx, vy, pm) => CreateSimpleBurst(x, y, screenWidth, screenHeight));

            emitters.Add(primaryEmitter);
            return emitters;
        }

        private static void CreateSimpleBurst(float posX, float posY, int screenWidth, int screenHeight)
        {
            // This would create a simple burst - you'd need to add this to your emitter management system
            // For demonstration purposes only
        }
    }
}