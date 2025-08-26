using System;
using ParticleSystem.Utils;

namespace ParticleSystem.Particles
{
    /// <summary>
    /// Delegate for handling completion callbacks when effects finish
    /// </summary>
    /// <param name="pos">Final position of the completed effect</param>
    /// <param name="vel">Final velocity of the completed effect</param>
    public delegate void OnCompleteCallback(Position pos, Velocity vel);
}