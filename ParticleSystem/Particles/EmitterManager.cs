using ParticleSystem.Particles;

namespace ParticleSystem.Particles;

public static class EmitterManager
{
    private static readonly Emitter?[] _emitters = new Emitter?[5000];

    /// <summary>
    /// Adds an emitter to the global emitter array. Finds the next available slot
    /// where the emitter is null or has a lifetime <= 0.
    /// </summary>
    /// <param name="emitter">The emitter to add</param>
    /// <returns>True if the emitter was added successfully, false if no slot was available</returns>
    public static bool AddEmitter(Emitter emitter)
    {
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
    public static void Update(float deltaTime)
    {
        for (int i = 0; i < _emitters.Length; i++)
        {
            var emitter = _emitters[i];
            if (emitter == null) continue;

            emitter.Update(deltaTime);
            
            if (emitter.IsAlive)
                emitter.EmitParticles();
        }
    }

    /// <summary>
    /// Gets the number of active emitters
    /// </summary>
    /// <returns>Count of active emitters</returns>
    public static int GetActiveEmitterCount()
    {
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

    /// <summary>
    /// Clears all emitters from the array
    /// </summary>
    public static void ClearAllEmitters()
    {
        for (int i = 0; i < _emitters.Length; i++)
        {
            _emitters[i] = null;
        }
    }
}