// Represents a single particle

namespace ParticleSystem.Particles;

public class Particle
{
    public int Index { get; }
    
    /// <summary>
    /// Callback function to invoke when the particle completes its lifetime
    /// </summary>
    public OnCompleteCallback OnComplete { get; set; }
    
    public Particle(int index, OnCompleteCallback onComplete = null)
    {
        Index = index;
        OnComplete = onComplete;
    }
}


