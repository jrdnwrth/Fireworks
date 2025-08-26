using ParticleSystem.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ParticleSystem.Utils.MathUtils;

namespace ParticleSystem.Particles;

public static class EmitterPresets
{
    public static void SparkleTrail(this Emitter e)
    {
        e.Lifetime = 1.7f;                // Short lifetime - just the rocket trail
        e.EmissionRate = 150f;            // Emit trail particles
        e.ParticleColor = FireworkColors.Gold;
        e.MinParticleLifetime = 0.6f;
        e.MaxParticleLifetime = 0.8f;
        e.RandomVelocityMagnitude = 50f;
        e.MinParticleDrag = 0.85f;
        e.MaxParticleDrag = 0.9f;
        e.ParticleType = ParticleType.Flicker;
    }

    public static void GlitterPop(this Emitter e)
    {
        e.Lifetime = 0.1f;       // Short sparkle burst
        e.EmissionRate = 1000f;   // High emission rate for intense sparkle
        e.ParticleColor = FireworkColors.Gold;
        e.MinParticleLifetime = 0.0f;
        e.MaxParticleLifetime = 0.5f;
        e.RandomVelocityMagnitude = 850f;
        e.MinParticleDrag = 0.8f;
        e.MaxParticleDrag = 0.9f;
        e.ParticleType = ParticleType.Flash;
    }

    public static void Star_1(this Emitter e, int color)
    {
        e.Lifetime = 1.0f + random_float() * 0.2f;            // Medium lifetime for burst
        e.EmissionRate = 30f;
        e.ParticleColor = color;
        e.MinParticleLifetime = 0.1f;
        e.MaxParticleLifetime = 0.1f;
        e.RandomVelocityMagnitude = 100f;
        e.MinParticleDrag = 0.75f;
        e.MaxParticleDrag = 0.85f;
        e.ParticleType = ParticleType.Decay;
    }

    public static void Fountain(this Emitter e)
    {
        e.Lifetime = float.MaxValue;     // Infinite lifetime
        e.EmissionTimer = 0f;
        e.EmissionRate = 100f;           
        e.ParticleColor = FireworkColors.PurpleViolet;
        e.MinParticleLifetime = 1.0f;
        e.MaxParticleLifetime = 2.0f;
        e.RandomVelocityMagnitude = 50f; 
        e.MinParticleDrag = 0.9f;        
        e.MaxParticleDrag = 0.95f;
        e.ParticleType = ParticleType.Decay;
    }

    public static void Snowfall(this Emitter e)
    {
        e.Lifetime = float.MaxValue;     // Infinite lifetime
        e.EmissionTimer = 0f;
        e.EmissionRate = 50f;            
        e.ParticleColor = FireworkColors.White;
        e.MinParticleLifetime = 5.0f;    
        e.MaxParticleLifetime = 10.0f;
        e.RandomVelocityMagnitude = 10f; 
        e.MinParticleDrag = 0.98f;       
        e.MaxParticleDrag = 0.99f;
        e.ParticleType = ParticleType.Decay;
    }

    public static void SetVelocity_RoundBurst(this Emitter e, Velocity parentVel, int i, int total_count, float speedX, float speedY)
    {
        float angle = (float)(Math.PI * 2f / total_count * i); // Evenly spaced angles around circle
        var rand = random.NextDouble();
        var circular_scaler = Math.Sqrt(1 - rand * rand);
        float burstVelX = (float)(Math.Cos(angle) * speedX * circular_scaler) + parentVel.X;  // Inherit parent velocity
        float burstVelY = (float)(Math.Sin(angle) * speedY * circular_scaler) + parentVel.Y;  // Inherit parent velocity
        e.Vel = new Velocity(burstVelX, burstVelY);
    }
}
