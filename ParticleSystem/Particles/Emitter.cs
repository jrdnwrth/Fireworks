using System;
using System.Collections.Generic;

namespace ParticleSystem.Particles;

public class Emitter
{
    private float _posX;
    private float _posY;
    private float _velX;
    private float _velY;
    private float _lifetime;
    private float _emissionTimer;
    
    // Physics properties (similar to particles)
    private readonly int _screenWidth;
    private readonly int _screenHeight;
    private readonly float _gravity;
    private readonly float _drag;
    
    // Emission properties
    private readonly float _emissionRate; // Particles per second
    private readonly int _particleColor;
    private readonly float _minParticleLifetime;
    private readonly float _maxParticleLifetime;
    private readonly float _randomVelocityMagnitude;
    private readonly Random _random;
    
    public bool IsAlive => _lifetime > 0;
    public float PosX => _posX;
    public float PosY => _posY;
    public float VelX => _velX;
    public float VelY => _velY;
    
    public Emitter(
        float posX, float posY, 
        float velX, float velY, 
        float lifetime,
        float emissionRate,
        int particleColor,
        float minParticleLifetime,
        float maxParticleLifetime,
        float randomVelocityMagnitude,
        int screenWidth, int screenHeight,
        float gravity = 200.0f, float drag = 0.999f,
        int? randomSeed = null)
    {
        _posX = posX;
        _posY = posY;
        _velX = velX;
        _velY = velY;
        _lifetime = lifetime;
        _emissionTimer = 0f;
        
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _gravity = gravity;
        _drag = drag;
        
        _emissionRate = emissionRate;
        _particleColor = particleColor;
        _minParticleLifetime = minParticleLifetime;
        _maxParticleLifetime = maxParticleLifetime;
        _randomVelocityMagnitude = randomVelocityMagnitude;
        
        _random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();
    }
    
    public void Update(float deltaTime)
    {
        if (!IsAlive) return;
        
        // Update lifetime
        _lifetime -= deltaTime;
        
        // Update physics (same as particles)
        float gDt = _gravity * deltaTime;
        _velY += gDt;
        _velX *= _drag;
        _velY *= _drag;
        
        _posX += _velX * deltaTime;
        _posY += _velY * deltaTime;
        
        // Boundary collisions with damping
        if (_posX < 0) { _posX = 0; _velX = -_velX * 0.8f; }
        else if (_posX > _screenWidth - 1) { _posX = _screenWidth - 1; _velX = -_velX * 0.8f; }
        
        if (_posY < 0) { _posY = 0; _velY = -_velY * 0.8f; }
        else if (_posY > _screenHeight - 1) { _posY = _screenHeight - 1; _velY = -_velY * 0.8f; }
        
        // Update emission timer
        _emissionTimer += deltaTime;
    }
    
    public List<Particle> EmitParticles(ParticleManager particleManager)
    {
        if (!IsAlive) return new List<Particle>();
        
        var emittedParticles = new List<Particle>();
        
        // Calculate how many particles to emit based on emission rate and elapsed time
        float particlesToEmit = _emissionRate * _emissionTimer;
        int particleCount = (int)particlesToEmit;
        
        if (particleCount > 0)
        {
            // Reset timer, keeping the fractional part for next frame
            _emissionTimer -= particleCount / _emissionRate;
            
            // Emit particles
            for (int i = 0; i < particleCount; i++)
            {
                // Generate random velocity
                float randomAngle = (float)(_random.NextDouble() * 2.0 * Math.PI);
                float randomSpeed = (float)(_random.NextDouble() * _randomVelocityMagnitude);
                float randomVelX = (float)(Math.Cos(randomAngle) * randomSpeed);
                float randomVelY = (float)(Math.Sin(randomAngle) * randomSpeed);
                
                // Combine emitter velocity with random velocity
                float finalVelX = _velX + randomVelX;
                float finalVelY = _velY + randomVelY;
                
                // Generate random lifetime within range
                float particleLifetime = _minParticleLifetime + 
                    (float)(_random.NextDouble() * (_maxParticleLifetime - _minParticleLifetime));
                
                // Create particle at emitter position with combined velocity
                var createdParticles = particleManager.CreateParticles(
                    1, _posX, _posY, finalVelX, finalVelY, _particleColor, particleLifetime);
                
                emittedParticles.AddRange(createdParticles);
            }
        }
        
        return emittedParticles;
    }
    
    public void Kill()
    {
        _lifetime = 0;
    }
}