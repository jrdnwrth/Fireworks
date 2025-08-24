using ParticleSystem.Utils;
using System.Collections.Generic;

namespace ParticleSystem.Particles;

public static class EmitterConfigurations
{
    /// <summary>
    /// Creates a collection of firework emitters for a particle display
    /// </summary>
    /// <param name="screenWidth">Screen width for boundary calculations</param>
    /// <param name="screenHeight">Screen height for boundary calculations</param>
    /// <returns>List of configured emitters</returns>
    public static List<Emitter> CreateFireworkEmitters(int screenWidth, int screenHeight)
    {
        var emitters = new List<Emitter>();

        // Create an upward-shooting red particle emitter
        var upwardEmitter = new Emitter(
            posX: 640f,                    // Center of screen horizontally
            posY: 600f,                    // Near bottom of screen
            velX: 5f,                      // No horizontal velocity
            velY: -400f,                   // Strong upward velocity
            lifetime: 3.5f,                // Emitter lives for 2.5 seconds
            emissionRate: 250f,            // Emit 100 particles per second
            particleColor: FireworkColors.BrightRed,     // Bright red
            minParticleLifetime: 0.05f,     // Particles live 1-3 seconds
            maxParticleLifetime: 0.2f,
            randomVelocityMagnitude: 80f,  // Random velocity spread
            minParticleDrag: 0.85f,
            maxParticleDrag: 0.9f);

        // Create an upward-shooting blue particle emitter
        var upwardEmitter2 = new Emitter(
            posX: 600f,                    // Center of screen horizontally
            posY: 600f,                    // Near bottom of screen
            velX: 20f,
            velY: -500f,                   // Strong upward velocity
            lifetime: 4.5f,                // Emitter lives for 3.5 seconds
            emissionRate: 300f,            // Emit 300 particles per second
            particleColor: FireworkColors.BrightBlue,     // Bright blue
            minParticleLifetime: 0.5f,     // Particles live 1-3 seconds
            maxParticleLifetime: 2.5f,
            randomVelocityMagnitude: 160f);  // Random velocity spread

        // Create an upward-shooting green particle emitter
        var upwardEmitter3 = new Emitter(
            posX: 680f,                    // Center of screen horizontally
            posY: 600f,                    // Near bottom of screen
            velX: -70f,
            velY: -140f,                   // Strong upward velocity
            lifetime: 2.5f,                // Emitter lives for 3.5 seconds
            emissionRate: 1500f,            // Emit 300 particles per second
            particleColor: FireworkColors.BrightGreen,     // Bright green
            minParticleLifetime: 0.05f,     // Particles live 1-3 seconds
            maxParticleLifetime: 0.3f,
            randomVelocityMagnitude: 900f);  // Random velocity spread

        // Create an upward-shooting gold particle emitter
        var upwardEmitter4 = new Emitter(
            posX: 800f,                    // Center of screen horizontally
            posY: 600f,                    // Near bottom of screen
            velX: 50f,
            velY: -460f,                   // Strong upward velocity
            lifetime: 3f,                // Emitter lives for 3.5 seconds
            emissionRate: 3000f,            // Emit 300 particles per second
            particleColor: FireworkColors.Gold,     // Gold
            minParticleLifetime: 0.05f,     // Particles live 1-3 seconds
            maxParticleLifetime: 0.5f,
            randomVelocityMagnitude: 100f);  // Random velocity spread

        emitters.Add(upwardEmitter);
        emitters.Add(upwardEmitter2);
        emitters.Add(upwardEmitter3);
        emitters.Add(upwardEmitter4);

        return emitters;
    }
}