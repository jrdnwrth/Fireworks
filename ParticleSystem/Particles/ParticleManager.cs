using System;

namespace ParticleSystem.Particles;

public class ParticleManager
{
    private readonly float[] _posX;
    private readonly float[] _posY;
    private readonly float[] _velX;
    private readonly float[] _velY;
    private readonly int[] _colorArgb;
    
    private readonly int _screenWidth;
    private readonly int _screenHeight;
    private readonly float _gravity;
    private readonly float _drag;
    
    public int ParticleCount { get; }
    
    public ParticleManager(int particleCount, int screenWidth, int screenHeight, float gravity = 200.0f, float drag = 0.999f)
    {
        ParticleCount = particleCount;
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _gravity = gravity;
        _drag = drag;
        
        _posX = new float[particleCount];
        _posY = new float[particleCount];
        _velX = new float[particleCount];
        _velY = new float[particleCount];
        _colorArgb = new int[particleCount];
        
        InitializeParticles();
    }
    
    private void InitializeParticles()
    {
        var rng = new Random(1234);
        for (int i = 0; i < ParticleCount; i++)
        {
            _posX[i] = (float)rng.NextDouble() * _screenWidth * 0.8f + _screenWidth * 0.1f;
            _posY[i] = (float)rng.NextDouble() * _screenHeight * 0.3f + _screenHeight * 0.1f;
            _velX[i] = (float)((rng.NextDouble() - 0.5) * 200.0);
            _velY[i] = (float)(-(rng.NextDouble() * 200.0 + 50.0));

            //int r = 160 + rng.Next(96);
            //int g = 160 + rng.Next(96);
            //int b = 160 + rng.Next(96);
            int r = 160 + rng.Next(96);
            int g = 0;
            int b = 0;
            _colorArgb[i] = unchecked((255 << 24) | (r << 16) | (g << 8) | b);
        }
    }
    
    public void Update(float deltaTime)
    {
        float gDt = _gravity * deltaTime;
        for (int i = 0; i < ParticleCount; i++)
        {
            _velY[i] += gDt;
            _velX[i] *= _drag;
            _velY[i] *= _drag;

            _posX[i] += _velX[i] * deltaTime;
            _posY[i] += _velY[i] * deltaTime;

            if (_posX[i] < 0) { _posX[i] = 0; _velX[i] = -_velX[i] * 0.8f; }
            else if (_posX[i] > _screenWidth - 1) { _posX[i] = _screenWidth - 1; _velX[i] = -_velX[i] * 0.8f; }

            if (_posY[i] < 0) { _posY[i] = 0; _velY[i] = -_velY[i] * 0.8f; }
            else if (_posY[i] > _screenHeight - 1) { _posY[i] = _screenHeight - 1; _velY[i] = -_velY[i] * 0.8f; }
        }
    }
    
    public void GetParticleData(out float[] posX, out float[] posY, out int[] colorArgb)
    {
        posX = _posX;
        posY = _posY;
        colorArgb = _colorArgb;
    }
}
