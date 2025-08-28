using ParticleSystem.Particles;
using System;

namespace ParticleSystem.Particles;

public static class EmitterManager
{
    private static readonly Emitter?[] _emitters = new Emitter?[5000];
    private static bool _initialized = false;

    /// <summary>
    /// Initializes the emitter pool by pre-creating 5000 emitter objects with lifetime 0
    /// </summary>
    private static void InitializePool()
    {
        if (_initialized) return;
        
        for (int i = 0; i < _emitters.Length; i++)
        {
            // Create emitters with default values and lifetime 0 to make them inactive
            _emitters[i] = new Emitter();
        }
        
        _initialized = true;
    }

    /// <summary>
    /// Gets the next available emitter from the pool
    /// </summary>
    /// <returns>An available emitter, or null if no emitters are available</returns>
    public static Emitter GetEmitter()
    {
        InitializePool();
        
        for (int i = 0; i < _emitters.Length; i++)
        {
            if (_emitters[i] != null && !_emitters[i]!.IsAlive)
            {
                _emitters[i]!.Clear(); // Reset emitter properties before reuse
                return _emitters[i]!;
            }
        }
        throw new Exception("No available emitter found");
    }

    /// <summary>
    /// Adds an emitter to the global emitter array. Finds the next available slot
    /// where the emitter is null or has a lifetime <= 0.
    /// </summary>
    /// <param name="emitter">The emitter to add</param>
    /// <returns>True if the emitter was added successfully, false if no slot was available</returns>
    public static bool AddEmitter(Emitter emitter)
    {
        InitializePool();
        
        for (int i = 0; i < _emitters.Length; i++)
        {
            if (_emitters[i] == null || !_emitters[i]!.IsAlive)
            {
                _emitters[i] = emitter;
                return true;
            }
        }
        
        return false; // No available slot found
    }

    /// <summary>
    /// Updates all active emitters and handles particle emission
    /// </summary>
    /// <param name="deltaTime">Time since last update</param>
    public static void Update(float deltaTime, int window_height)
    {
        InitializePool();
        
        for (int i = 0; i < _emitters.Length; i++)
        {
            var emitter = _emitters[i];
            if (emitter == null) continue;



            if (emitter.IsAlive)
            {
                emitter.Update(deltaTime, window_height);
                emitter.EmitParticles();
            }
        }
    }

    /// <summary>
    /// Gets the number of active emitters
    /// </summary>
    /// <returns>Count of active emitters</returns>
    public static int GetActiveEmitterCount()
    {
        if (!_initialized) return 0;
        
        int count = 0;
        for (int i = 0; i < _emitters.Length; i++)
        {
            if (_emitters[i] != null && _emitters[i]!.IsAlive)
            {
                count++;
            }
        }
        return count;
    }
}