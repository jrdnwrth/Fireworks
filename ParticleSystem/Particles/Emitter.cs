using System;
using System.Collections.Generic;
using ParticleSystem.Utils;
using static ParticleSystem.Utils.MathUtils;

namespace ParticleSystem.Particles;

public class Emitter
{
    public float PosX = 100f;
    public float PosY = 100f;
    public float VelX = 0f;
    public float VelY = 0f;
    public float Lifetime = 0f;
    public float EmissionTimer = 0f;

    // New tuple struct properties
    public Position Pos
    {
        get => new(PosX, PosY);
        set { PosX = value.X; PosY = value.Y; }
    }

    public Velocity Vel
    {
        get => new(VelX, VelY);
        set { VelX = value.X; VelY = value.Y; }
    }

    // Physics properties
    public float Gravity = 200.0f;
    public float Drag = 0.999f;

    // Emission properties
    public float EmissionRate = 100f; // Particles per second
    public int ParticleColor = FireworkColors.Gold;
    public float? initial_particle_size = null;
    public float MinParticleLifetime = 0.1f;
    public float MaxParticleLifetime = 1.0f;
    public float RandomVelocityMagnitude = 40f;
    public float MinParticleDrag = 0.8f;
    public float MaxParticleDrag = 0.93f;
    public ParticleType ParticleType = ParticleType.Decay;
    public bool decay_particle_size = false;
    public bool decay_emission = false;

    // Box-Muller transform state
    public bool HasSpareNormal = false;
    public float SpareNormal;

    // Callback for chaining effects
    public bool CallbackInvoked = false;

    public bool IsAlive => Lifetime > 0;

    /// <summary>
    /// Callback function to invoke when the emitter completes its lifetime
    /// </summary>
    public OnCompleteCallback? OnComplete { get; set; }

    /// <summary>
    /// This is called by the EmitterManager when reusing an emitter from the pool.
    /// </summary>
    public void Clear()
    {
        PosX = 0;
        PosY = 0;
        VelX = 0;
        VelY = 0;
        Lifetime = 0;
        EmissionTimer = 0;
        Gravity = 200.0f;
        Drag = 0.999f;
        EmissionRate = 1f;
        ParticleColor = FireworkColors.Gold;
        initial_particle_size = null;
        MinParticleLifetime = 0.1f;
        MaxParticleLifetime = 1f;
        RandomVelocityMagnitude = 50f;
        MinParticleDrag = 0.8f;
        MaxParticleDrag = 0.93f;
        ParticleType = ParticleType.Decay;
        HasSpareNormal = false;
        SpareNormal = 0f;
        CallbackInvoked = false;
        OnComplete = null;
    }

    public void Update(float deltaTime, int window_height)
    {
        if (!IsAlive) return;

        // Store previous lifetime to detect when emitter dies
        float previousLifetime = Lifetime;

        // Update lifetime
        Lifetime -= deltaTime;

        // Check if emitter just died and invoke callback if available
        if (previousLifetime > 0 && Lifetime <= 0 && !CallbackInvoked && OnComplete != null)
        {
            CallbackInvoked = true;
            OnComplete(Pos, Vel);
        }

        if (!IsAlive) return;

        // Update physics (same as particles)
        float gDt = Gravity * deltaTime;
        VelY += gDt;
        VelX *= Drag;
        VelY *= Drag;

        PosX += VelX * deltaTime;
        PosY += VelY * deltaTime;

        //if (PosY > window_height - 20)
        //{
        //    PosY = window_height - 20;
        //    VelY = -VelY * 0.8f;
        //}

        // Update emission timer
        EmissionTimer += deltaTime;
    }

    public void EmitParticles()
    {
        if (!IsAlive) return;

        // Calculate how many particles to emit based on emission rate and elapsed time
        float particlesToEmit = EmissionRate * EmissionTimer;
        int particleCount = (int)particlesToEmit;

        if (particleCount <= 0)
            return;

        // Reset timer, keeping the fractional part for next frame
        EmissionTimer -= particleCount / EmissionRate;

        // Emit particles
        for (int i = 0; i < particleCount; i++)
        {
            // Generate random velocity using normal distribution
            float randomAngle = (float)(random.NextDouble() * 2.0 * Math.PI);

            // Use normal distribution for speed where RandomVelocityMagnitude is 2-sigma
            // This means 95% of particles will have speed between 0 and RandomVelocityMagnitude
            float randomSpeed = Math.Abs(NextGaussian(ref HasSpareNormal, ref SpareNormal) * (RandomVelocityMagnitude / 2.0f));

            float randomVelX = (float)(Math.Cos(randomAngle) * randomSpeed);
            float randomVelY = (float)(Math.Sin(randomAngle) * randomSpeed);

            // Combine emitter velocity with random velocity
            float finalVelX = VelX + randomVelX;
            float finalVelY = VelY + randomVelY;

            // Generate random lifetime using Gaussian distribution
            // Use normal distribution where MaxParticleLifetime is 2-sigma
            // This means 95% of particles will have lifetime between MinParticleLifetime and (MinParticleLifetime + MaxParticleLifetime)
            float randomLifetime = Math.Abs(NextGaussian(ref HasSpareNormal, ref SpareNormal) * (MaxParticleLifetime / 2.0f));
            float particleLifetime = MinParticleLifetime + randomLifetime;

            // Generate random drag within range
            float particleDrag = MinParticleDrag +
                (float)(random.NextDouble() * (MaxParticleDrag - MinParticleDrag));

            // Create particle at emitter position with combined velocity, individual drag, and specified particle type
            var createdParticles = ParticleManager.CreateParticles(
                1, PosX, PosY, finalVelX, finalVelY, ParticleColor, particleLifetime, particleDrag,
                size: initial_particle_size, ParticleType);
        }
    }

    public void Kill()
    {
        Lifetime = 0;
    }
}