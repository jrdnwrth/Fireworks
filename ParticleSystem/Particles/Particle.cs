// Represents a single particle

namespace ParticleSystem.Particles;

public struct Particle
{
    public float PosX;
    public float PosY;
    public float VelX;
    public float VelY;
    public int ColorArgb;
    
    public Particle(float posX, float posY, float velX, float velY, int colorArgb)
    {
        PosX = posX;
        PosY = posY;
        VelX = velX;
        VelY = velY;
        ColorArgb = colorArgb;
    }
}
