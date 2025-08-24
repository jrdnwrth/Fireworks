using System.Collections.Generic;
using System;

namespace ParticleSystem.Particles
{
    public static class ParticleManager
    {
        private static float[] _posX = null!;
        private static float[] _posY = null!;
        private static float[] _velX = null!;
        private static float[] _velY = null!;
        private static int[] _colorArgb = null!;
        private static float[] _lifetime = null!;
        private static float[] _drag = null!; // Array for individual drag values
        private static float[] _size = null!; // Array for individual particle sizes
        private static byte[] _type = null!; // Array for individual particle types
        private static OnCompleteCallback[] _callbacks = null!; // Array to store particle callbacks
        
        private static int _screenWidth;
        private static int _screenHeight;
        private static float _gravity;
        private static float _defaultDrag; // Renamed for clarity
        private static float _defaultSize = 5.0f; // Default particle size
        
        public static int ParticleCount { get; private set; }
        public static bool IsInitialized { get; private set; }
        
        public static void Initialize(int particleCount, int screenWidth, int screenHeight, float gravity = 200.0f, float drag = 0.9f)
        {
            ParticleCount = particleCount;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _gravity = gravity;
            _defaultDrag = drag;
            
            _posX = new float[particleCount];
            _posY = new float[particleCount];
            _velX = new float[particleCount];
            _velY = new float[particleCount];
            _colorArgb = new int[particleCount];
            _lifetime = new float[particleCount];
            _drag = new float[particleCount]; // Initialize drag array
            _size = new float[particleCount]; // Initialize size array
            _type = new byte[particleCount]; // Initialize type array
            _callbacks = new OnCompleteCallback[particleCount]; // Initialize callback array
            
            IsInitialized = true;
        }
        
        public static void Update(float deltaTime)
        {
            if (!IsInitialized) return;
            
            float gDt = _gravity * deltaTime;
            for (int i = 0; i < ParticleCount; i++)
            {
                // Only update particles that are alive
                if (_lifetime[i] > 0)
                {
                    float previousLifetime = _lifetime[i];
                    
                    // Update lifetime
                    _lifetime[i] -= deltaTime;
                    
                    // Check if particle just died and invoke callback if available
                    if (previousLifetime > 0 && _lifetime[i] <= 0 && _callbacks[i] != null)
                    {
                        _callbacks[i](_posX[i], _posY[i], _velX[i], _velY[i]);
                        _callbacks[i] = null; // Clear callback to prevent multiple invocations
                    }
                    
                    if (_lifetime[i] > 0) // Only continue physics if still alive
                    {
                        // Update physics using individual drag values
                        _velY[i] += gDt;
                        _velX[i] *= _drag[i];
                        _velY[i] *= _drag[i];

                        _posX[i] += _velX[i] * deltaTime;
                        _posY[i] += _velY[i] * deltaTime;

                        if (_posX[i] < 0) { _posX[i] = 0; _velX[i] = -_velX[i] * 0.8f; }
                        else if (_posX[i] > _screenWidth - 1) { _posX[i] = _screenWidth - 1; _velX[i] = -_velX[i] * 0.8f; }

                        if (_posY[i] < 0) { _posY[i] = 0; _velY[i] = -_velY[i] * 0.8f; }
                        else if (_posY[i] > _screenHeight - 1) { _posY[i] = _screenHeight - 1; _velY[i] = -_velY[i] * 0.8f; }
                    }
                }
            }
        }
        
        public static void GetParticleData(out float[] posX, out float[] posY, out int[] colorArgb)
        {
            posX = _posX;
            posY = _posY;
            colorArgb = _colorArgb;
        }
        
        public static void GetParticleData(out float[] posX, out float[] posY, out int[] colorArgb, out float[] size)
        {
            posX = _posX;
            posY = _posY;
            colorArgb = _colorArgb;
            size = _size;
        }
        
        public static void GetParticleData(out float[] posX, out float[] posY, out int[] colorArgb, out float[] size, out byte[] type)
        {
            posX = _posX;
            posY = _posY;
            colorArgb = _colorArgb;
            size = _size;
            type = _type;
        }
        
        public static bool IsParticleAlive(int index)
        {
            return IsInitialized && index >= 0 && index < ParticleCount && _lifetime[index] > 0;
        }
        
        public static List<Particle> CreateParticles(int count, float posX, float posY, float velX, float velY, int colorArgb, float lifetime, float drag = -1f, float size = -1f, ParticleType particleType = ParticleType.Decay, OnCompleteCallback? onComplete = null)
        {
            if (!IsInitialized) return new List<Particle>();
            
            var createdParticles = new List<Particle>();
            int created = 0;
            
            // Use default drag if not specified
            float particleDrag = drag < 0 ? _defaultDrag : drag;
            // Use default size if not specified
            float particleSize = size < 0 ? _defaultSize : size;
            
            for (int i = 0; i < ParticleCount && created < count; i++)
            {
                if (_lifetime[i] <= 0) // Found a dead particle slot
                {
                    _posX[i] = posX;
                    _posY[i] = posY;
                    _velX[i] = velX;
                    _velY[i] = velY;
                    _colorArgb[i] = colorArgb;
                    _lifetime[i] = lifetime;
                    _drag[i] = particleDrag; // Set individual drag value
                    _size[i] = particleSize; // Set individual size value
                    _type[i] = (byte)particleType; // Set individual particle type
                    _callbacks[i] = onComplete; // Set callback for this particle
                    
                    createdParticles.Add(new Particle(i, onComplete));
                    created++;
                }
            }
            
            return createdParticles;
        }
        
        public static void DeleteParticle(Particle? particle)
        {
            if (!IsInitialized || particle == null || particle.Index < 0 || particle.Index >= ParticleCount)
                return;
                
            _lifetime[particle.Index] = 0;
            _callbacks[particle.Index] = null; // Clear callback
        }
        
        public static int GetAliveParticleCount()
        {
            if (!IsInitialized) return 0;
            
            int count = 0;
            for (int i = 0; i < ParticleCount; i++)
            {
                if (_lifetime[i] > 0)
                    count++;
            }
            return count;
        }
        
        public static void Reset()
        {
            if (!IsInitialized) return;
            
            for (int i = 0; i < ParticleCount; i++)
            {
                _lifetime[i] = 0;
                _callbacks[i] = null;
            }
        }
    }
}
