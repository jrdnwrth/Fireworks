using System;

namespace ParticleSystem.Particles
{
    /// <summary>
    /// Delegate for handling completion callbacks when effects finish
    /// </summary>
    /// <param name="posX">Final X position of the completed effect</param>
    /// <param name="posY">Final Y position of the completed effect</param>
    /// <param name="velX">Final X velocity of the completed effect</param>
    /// <param name="velY">Final Y velocity of the completed effect</param>
    /// <param name="particleManager">Reference to the particle manager for creating new effects</param>
    public delegate void OnCompleteCallback(float posX, float posY, float velX, float velY, ParticleManager particleManager);
}