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
        var upwardEmitter = new Emitter
        {
            PosX = 640f,                    // Center of screen horizontally
            PosY = 600f,                    // Near bottom of screen
            VelX = 5f,                      // No horizontal velocity
            VelY = -400f,                   // Strong upward velocity
            Lifetime = 3.5f,                // Emitter lives for 3.5 seconds
            EmissionTimer = 0f,
            EmissionRate = 250f,            // Emit 250 particles per second
            ParticleColor = FireworkColors.BrightRed,     // Bright red
            MinParticleLifetime = 0.05f,     // Particles live 0.05-0.2 seconds
            MaxParticleLifetime = 0.2f,
            RandomVelocityMagnitude = 80f,  // Random velocity spread
            MinParticleDrag = 0.85f,
            MaxParticleDrag = 0.9f
        };

        // Create an upward-shooting blue particle emitter
        var upwardEmitter2 = new Emitter
        {
            PosX = 600f,                    // Center of screen horizontally
            PosY = 600f,                    // Near bottom of screen
            VelX = 20f,
            VelY = -500f,                   // Strong upward velocity
            Lifetime = 4.5f,                // Emitter lives for 4.5 seconds
            EmissionTimer = 0f,
            EmissionRate = 300f,            // Emit 300 particles per second
            ParticleColor = FireworkColors.BrightBlue,     // Bright blue
            MinParticleLifetime = 0.5f,     // Particles live 0.5-2.5 seconds
            MaxParticleLifetime = 2.5f,
            RandomVelocityMagnitude = 160f  // Random velocity spread
        };

        // Create an upward-shooting green particle emitter
        var upwardEmitter3 = new Emitter
        {
            PosX = 680f,                    // Center of screen horizontally
            PosY = 600f,                    // Near bottom of screen
            VelX = -70f,
            VelY = -140f,                   // Strong upward velocity
            Lifetime = 2.5f,                // Emitter lives for 2.5 seconds
            EmissionTimer = 0f,
            EmissionRate = 1500f,           // Emit 1500 particles per second
            ParticleColor = FireworkColors.BrightGreen,     // Bright green
            MinParticleLifetime = 0.05f,    // Particles live 0.05-0.3 seconds
            MaxParticleLifetime = 0.3f,
            RandomVelocityMagnitude = 900f  // Random velocity spread
        };

        // Create an upward-shooting gold particle emitter
        var upwardEmitter4 = new Emitter
        {
            PosX = 800f,                    // Center of screen horizontally
            PosY = 600f,                    // Near bottom of screen
            VelX = 50f,
            VelY = -460f,                   // Strong upward velocity
            Lifetime = 3f,                  // Emitter lives for 3 seconds
            EmissionTimer = 0f,
            EmissionRate = 3000f,           // Emit 3000 particles per second
            ParticleColor = FireworkColors.Gold,     // Gold
            MinParticleLifetime = 0.05f,    // Particles live 0.05-0.5 seconds
            MaxParticleLifetime = 0.5f,
            RandomVelocityMagnitude = 100f  // Random velocity spread
        };

        emitters.Add(upwardEmitter);
        emitters.Add(upwardEmitter2);
        emitters.Add(upwardEmitter3);
        emitters.Add(upwardEmitter4);

        return emitters;
    }
}