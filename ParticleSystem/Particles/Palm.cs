using ParticleSystem.Utils;
using System;
using static ParticleSystem.Utils.MathUtils;

namespace ParticleSystem.Particles;

public static partial class Palm
{
    public static void Create(int color, float xPos, float yPos, bool flicker, bool sphere_burst)
    {
        // Get an emitter from the pool instead of creating new one
        var e = EmitterManager.GetEmitter();

        // Set position and velocity using tuple structs
        e.Pos = new Position(xPos, yPos);    // Center horizontally, near bottom
        e.Vel = new Velocity(random_float(-60f, 60f), -530f + random_float(-60f, 60f));    // Slight horizontal, strong upward velocity
        e.SparkleTrail();                    // Apply preset for rocket trail
        e.EmissionRate = 70f;
        e.initial_particle_size = 4f;
        e.ParticleType = ParticleType.Decay;
        e.initial_lifetime = random_float(1.7f, 2.0f);
        if (sphere_burst)
            e.OnComplete = (pos, vel) => CreateSphereBurstExplosion(pos, vel, color, flicker); // Chain to burst explosion
        else
            e.OnComplete = (pos, vel) => CreateBurstExplosion(pos, vel, color, flicker); // Chain to burst explosion
    }

    private static void CreateSphereBurstExplosion(Position pos, Velocity vel, int color, bool flicker)
    {
        int total = (int)random_float(60, 80);

        // Convert total to layers.
        var layers = (int)total / 20f;

        // Generate each particle.
        foreach ((var x_unit, var y_unit, var z_unit) in generate_unit_sphere(layers))
        {
            float randomSpeed = random_float(130f, 150f);

            //float randomVelX = (float)(x_unit * randomSpeed);
            //float randomVelY = (float)(y_unit * randomSpeed);
            var e = EmitterManager.GetEmitter();
            e.Pos = pos;
            e.Vel = new Velocity(vel.X + x_unit * randomSpeed, vel.Y + y_unit * randomSpeed);
            e.initial_lifetime = random_float(1.3f, 1.5f);                // Short lifetime - just the rocket trail
            e.EmissionTimer = 0f;
            e.EmissionRate = 850f;            // Emit trail particles
            e.ParticleColor = color;
            e.MinParticleLifetime = 1.1f;
            e.MaxParticleLifetime = 1.2f;
            e.RandomVelocityMagnitude = 80f;
            e.MinParticleDrag = 0.88f;
            e.MaxParticleDrag = 0.9f;

            e.decay_particle_size = true;
            e.decay_emission = true;
            e.decay_particle_velocity_by = 0.02f;
            if (flicker)
            {
                e.initial_particle_size = 8f;
                e.ParticleType = ParticleType.Flicker;
            }
            else
            {
                e.initial_particle_size = 6f;
                e.ParticleType = ParticleType.DecayHemisphere;
            }
        }
    }

    /// <summary>
    /// Stage 2: Creates the main burst explosion at the rocket's final position
    /// </summary>
    private static void CreateBurstExplosion(Position pos, Velocity vel, int color, bool flicker)
    {
        int total = (int)(random_float(9, 13));
        for (int i = 0; i < total; i++)
        {
            var e = EmitterManager.GetEmitter();
            e.Pos = pos;
            e.SetVelocity_RoundBurst(vel, i, total, 140f, 140f);
            e.initial_lifetime = random_float(1.3f, 1.5f);                // Short lifetime - just the rocket trail
            e.EmissionTimer = 0f;
            e.EmissionRate = 650f;            // Emit trail particles
            e.ParticleColor = color;
            e.MinParticleLifetime = 1.1f;
            e.MaxParticleLifetime = 1.2f;
            e.RandomVelocityMagnitude = 80f;
            e.MinParticleDrag = 0.88f;
            e.MaxParticleDrag = 0.9f;

            e.decay_particle_size = true;
            e.decay_emission = true;
            e.decay_particle_velocity_by = 0.02f;
            if (flicker)
            {
                e.initial_particle_size = 8f;
                e.ParticleType = ParticleType.Flicker;
            }
            else
            {
                e.initial_particle_size = 6f;
                e.ParticleType = ParticleType.DecayHemisphere;
            }
        }
    }

}