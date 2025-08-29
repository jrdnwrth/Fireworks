using ParticleSystem.Utils;
using System;
using static ParticleSystem.Utils.MathUtils;

namespace ParticleSystem.Particles;

public static partial class Chrysanthemum3
{
    public static void Create(int color, float xPos, float yPos, bool splitter_finish, bool glitter)
    {
        // Get an emitter from the pool instead of creating new one
        var e = EmitterManager.GetEmitter();

        // Set position and velocity using tuple structs
        e.Pos = new Position(xPos, yPos);    // Center horizontally, near bottom
        e.Vel = new Velocity(random_float(-60f, 60f), -500f + random_float(-90f, 60f));    // Slight horizontal, strong upward velocity
        e.SparkleTrail();                    // Apply preset for rocket trail
        e.EmissionRate = 100f;            // Emit trail particles
        e.initial_lifetime = random_float(1.7f, 2.5f);

        e.OnComplete = (pos, vel) => CreateBurst(pos, vel, color, splitter_finish, glitter); // Chain to burst explosion
    }

    /// <summary>
    /// Stage 3: Creates sparkle effects at each burst emitter's final position
    /// </summary>
    private static void CreateBurst(Position pos, Velocity vel, int color, bool splitter_finish, bool glitter)
    {
        // Create a final sparkle burst
        var e = EmitterManager.GetEmitter();
        e.Pos = pos;
        e.Vel = vel;    // Inherit some of the parent velocity
        e.initial_lifetime = 0.04f;       // Short sparkle burst
        e.EmissionRate = random_float(100f, 350f);   // Not used when sphere_emitter is enabled.
        e.ParticleColor = color;
        e.MinParticleLifetime = 0.6f;
        e.MaxParticleLifetime = 0.7f;
        e.RandomVelocityMagnitude = 230f;
        e.MinParticleDrag = 0.98f;
        e.MaxParticleDrag = 0.985f;
        if (glitter)
        {
            e.initial_particle_size = 12;
            e.ParticleType = ParticleType.Flicker;
        }
        else
        {
            e.initial_particle_size = 9;
            e.ParticleType = ParticleType.DecayHemisphere;
        }
        e.decay_particle_velocity_by = 0.0f;  // Probably doesn't do anything.
        e.sphere_emitter = true;
        if(splitter_finish)
            e.OnEachParticleComplete = (pos, vel) => CreateSplitEffect(pos, vel, color);
    }

    private static void CreateSplitEffect(Position pos, Velocity vel, int color)
    {
        // Create a final sparkle burst
        var e = EmitterManager.GetEmitter();
        e.Pos = pos;
        e.Vel = vel;                    // Inherit some of the parent velocity
        e.initial_lifetime = 0.04f;      // Short sparkle burst
        e.EmissionRate = 50f;           // High emission rate for intense sparkle
        e.ParticleColor = color;
        e.MinParticleLifetime = 0.1f;
        e.MaxParticleLifetime = 0.5f;
        e.RandomVelocityMagnitude = 550f;
        e.initial_particle_size = 7;
        e.MinParticleDrag = 0.8f;
        e.MaxParticleDrag = 0.9f;
        e.ParticleType = ParticleType.Decay;
    }
}