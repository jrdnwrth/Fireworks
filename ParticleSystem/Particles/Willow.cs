using ParticleSystem.Utils;
using System;
using static ParticleSystem.Utils.MathUtils;

namespace ParticleSystem.Particles;

public static partial class Willow
{
    public static void Create(int color, float xPos)
    {
        // Get an emitter from the pool instead of creating new one
        var e = EmitterManager.GetEmitter();

        // Set position and velocity using tuple structs
        e.Pos = new Position(xPos, 800f);    // Center horizontally, near bottom
        e.Vel = new Velocity(random_float(-60f, 60f), -530f + random_float(-60f, 60f));    // Slight horizontal, strong upward velocity
        e.SparkleTrail();                    // Apply preset for rocket trail
        e.initial_particle_size= 6f;
        e.ParticleType = ParticleType.Flicker;
        e.Lifetime = random_float(1.7f, 2.0f);
        e.OnComplete = (pos, vel) => CreateBurstExplosion(pos, vel, color); // Chain to burst explosion
    }

    /// <summary>
    /// Stage 2: Creates the main burst explosion at the rocket's final position
    /// </summary>
    private static void CreateBurstExplosion(Position pos, Velocity vel, int color)
    {
        {
            // Create initial burst
            var e = EmitterManager.GetEmitter();
            e.Pos = pos;
            e.Vel = vel;    // Inherit some of the parent velocity

            e.Lifetime = 0.1f;
            e.EmissionRate = 1500f;
            e.initial_particle_size = 10f;
            e.ParticleColor = FireworkColors.Gold;
            e.MinParticleLifetime = 0.01f;
            e.MaxParticleLifetime = 0.1f;
            e.RandomVelocityMagnitude = 140f;
            e.MinParticleDrag = 0.95f;
            e.MaxParticleDrag = 0.99f;
            e.ParticleType = ParticleType.Decay;
        }
        {
            // Create lingering sparks
            var e = EmitterManager.GetEmitter();
            e.Pos = pos;
            e.Vel = vel;    // Inherit some of the parent velocity

            e.Lifetime = 0.1f;
            e.EmissionRate = 2500f;
            e.initial_particle_size = 10f;
            e.ParticleColor = FireworkColors.Gold;
            e.MinParticleLifetime = 1.8f;
            e.MaxParticleLifetime = 2f;
            e.RandomVelocityMagnitude = 140f;
            e.MinParticleDrag = 0.95f;
            e.MaxParticleDrag = 0.99f;
            e.ParticleType = ParticleType.Decay;
        }





        //int total = (int)(random_float(9, 17));
        //for (int i = 0; i < total; i++)
        //{
        //    var e = EmitterManager.GetEmitter();
        //    e.Pos = pos;
        //    e.SetVelocity_RoundBurst(vel, i, total, 140f, 140f);
        //    e.Lifetime = 1.7f;                
        //    e.EmissionTimer = 0f;
        //    e.EmissionRate = 150f;           
        //    e.ParticleColor = color;
        //    e.MinParticleLifetime = 0.9f;
        //    e.MaxParticleLifetime = 1f;
        //    e.RandomVelocityMagnitude = 50f;
        //    e.MinParticleDrag = 0.85f;
        //    e.MaxParticleDrag = 0.9f;
        //}
    }

    ///// <summary>
    ///// Stage 3: Creates sparkle effects at each burst emitter's final position
    ///// </summary>
    //private static void CreateSparkleEffect(Position pos, Velocity vel)
    //{
    //    // Create a final sparkle burst
    //    var e = EmitterManager.GetEmitter();
    //    e.Pos = pos;
    //    e.Vel = vel;    // Inherit some of the parent velocity
    //    e.GlitterPop(); // Apply preset for glitter pop
    //    e.EmissionRate = 400f;
    //}
}