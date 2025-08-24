using System;
using System.Collections.Generic;
using ParticleSystem.Utils;

namespace ParticleSystem.Particles
{
    public class Emitter
    {
        private float _posX;
        private float _posY;
        private float _velX;
        private float _velY;
        private float _lifetime;
        private float _emissionTimer;
        
        // Physics properties (similar to particles)
        private readonly float _gravity;
        private readonly float _drag;
        
        // Emission properties
        private readonly float _emissionRate; // Particles per second
        private readonly int _particleColor;
        private readonly float _minParticleLifetime;
        private readonly float _maxParticleLifetime;
        private readonly float _randomVelocityMagnitude;
        private readonly float _minParticleDrag; // New: minimum drag for particles
        private readonly float _maxParticleDrag; // New: maximum drag for particles
        private readonly ParticleType _particleType; // New: type of particles to emit
        private readonly Random _random;
        
        // Box-Muller transform state
        private bool _hasSpareNormal = false;
        private float _spareNormal;

        // Callback for chaining effects
        private bool _callbackInvoked = false;
        
        public bool IsAlive => _lifetime > 0;
        public float PosX => _posX;
        public float PosY => _posY;
        public float VelX => _velX;
        public float VelY => _velY;
        
        /// <summary>
        /// Callback function to invoke when the emitter completes its lifetime
        /// </summary>
        public OnCompleteCallback? OnComplete { get; set; }
        
        public Emitter(
            float posX, float posY, 
            float velX, float velY, 
            float lifetime,
            float emissionRate,
            int particleColor,
            float minParticleLifetime,
            float maxParticleLifetime,
            float randomVelocityMagnitude,
            float gravity = 200.0f, float drag = 0.999f,
            float minParticleDrag = 0.8f, float maxParticleDrag = 0.93f, 
            ParticleType particleType = ParticleType.Decay,
            int? randomSeed = null,
            OnCompleteCallback? onComplete = null)
        {
            _posX = posX;
            _posY = posY;
            _velX = velX;
            _velY = velY;
            _lifetime = lifetime;
            _emissionTimer = 0f;
            
            _gravity = gravity;
            _drag = drag;
            
            _emissionRate = emissionRate;
            _particleColor = particleColor;
            _minParticleLifetime = minParticleLifetime;
            _maxParticleLifetime = maxParticleLifetime;
            _randomVelocityMagnitude = randomVelocityMagnitude;
            _minParticleDrag = minParticleDrag;
            _maxParticleDrag = maxParticleDrag;
            _particleType = particleType;
            
            _random = randomSeed.HasValue ? new Random(randomSeed.Value) : new Random();
            OnComplete = onComplete;
        }

        public void Update(float deltaTime)
        {
            if (!IsAlive) return;
            
            // Store previous lifetime to detect when emitter dies
            float previousLifetime = _lifetime;
            
            // Update lifetime
            _lifetime -= deltaTime;
            
            // Check if emitter just died and invoke callback if available
            if (previousLifetime > 0 && _lifetime <= 0 && !_callbackInvoked && OnComplete != null)
            {
                _callbackInvoked = true;
                OnComplete(_posX, _posY, _velX, _velY);
            }
            
            if (!IsAlive) return;
            
            // Update physics (same as particles)
            float gDt = _gravity * deltaTime;
            _velY += gDt;
            _velX *= _drag;
            _velY *= _drag;
            
            _posX += _velX * deltaTime;
            _posY += _velY * deltaTime;
            
            if (_posY < 20) { _posY = 0; _velY = -_velY * 0.8f; }
            
            // Update emission timer
            _emissionTimer += deltaTime;
        }
        
        public List<Particle> EmitParticles()
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
                    // Generate random velocity using normal distribution
                    float randomAngle = (float)(_random.NextDouble() * 2.0 * Math.PI);
                    
                    // Use normal distribution for speed where _randomVelocityMagnitude is 2-sigma
                    // This means 95% of particles will have speed between 0 and _randomVelocityMagnitude
                    float randomSpeed = Math.Abs(MathUtils.NextGaussian(_random, ref _hasSpareNormal, ref _spareNormal) * (_randomVelocityMagnitude / 2.0f));
                    
                    float randomVelX = (float)(Math.Cos(randomAngle) * randomSpeed);
                    float randomVelY = (float)(Math.Sin(randomAngle) * randomSpeed);
                    
                    // Combine emitter velocity with random velocity
                    float finalVelX = _velX + randomVelX;
                    float finalVelY = _velY + randomVelY;
                    
                    // Generate random lifetime using Gaussian distribution
                    // Use normal distribution where _maxParticleLifetime is 2-sigma
                    // This means 95% of particles will have lifetime between _minParticleLifetime and (_minParticleLifetime + _maxParticleLifetime)
                    float randomLifetime = Math.Abs(MathUtils.NextGaussian(_random, ref _hasSpareNormal, ref _spareNormal) * (_maxParticleLifetime / 2.0f));
                    float particleLifetime = _minParticleLifetime + randomLifetime;

                    // Generate random drag within range
                    float particleDrag = _minParticleDrag + 
                        (float)(_random.NextDouble() * (_maxParticleDrag - _minParticleDrag));
                    
                    // Create particle at emitter position with combined velocity, individual drag, and specified particle type
                    var createdParticles = ParticleManager.CreateParticles(
                        1, _posX, _posY, finalVelX, finalVelY, _particleColor, particleLifetime, particleDrag,
                        size: null, _particleType);
                    
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
}