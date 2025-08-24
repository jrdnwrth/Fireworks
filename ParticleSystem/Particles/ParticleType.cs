namespace ParticleSystem.Particles
{
    /// <summary>
    /// Defines the different types of particle behaviors
    /// </summary>
    public enum ParticleType : byte
    {
        /// <summary>
        /// Standard particle that decreases size over its lifetime.
        /// </summary>
        Decay = 0,

        /// <summary>
        /// Particle that oscilates between size 0 and its initial size.
        /// </summary>
        Strobe = 1,
        
        /// <summary>
        /// Particle that flickers with random size variations above size 0.
        /// </summary>
        Flicker = 2,
        
        /// <summary>
        /// Particle that creates a bright flash effect at the very end of its life.
        /// </summary>
        Flash = 3
    }
}