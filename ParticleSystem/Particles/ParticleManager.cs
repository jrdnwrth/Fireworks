using System;
using System.Collections.Generic;

namespace ParticleSystem.Particles;

public class ParticleManager
{
    private readonly float[] _posX;
    private readonly float[] _posY;
    private readonly float[] _velX;
    private readonly float[] _velY;
    private readonly int[] _colorArgb;
    private readonly float[] _lifetime;
    
    private readonly int _screenWidth;
    private readonly int _screenHeight;
    private readonly float _gravity;
    private readonly float _drag;
    
    public int ParticleCount { get; }
    
    public ParticleManager(int particleCount, int screenWidth, int screenHeight, float gravity = 200.0f, float drag = 0.9f)
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
        _lifetime = new float[particleCount];
    }
    
    public void Update(float deltaTime)
    {
        float gDt = _gravity * deltaTime;
        for (int i = 0; i < ParticleCount; i++)
        {
            // Only update particles that are alive
            if (_lifetime[i] > 0)
            {
                // Update lifetime
                _lifetime[i] -= deltaTime;
                
                // Update physics
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
    }
    
    public void GetParticleData(out float[] posX, out float[] posY, out int[] colorArgb)
    {
        posX = _posX;
        posY = _posY;
        colorArgb = _colorArgb;
    }
    
    public bool IsParticleAlive(int index)
    {
        return index >= 0 && index < ParticleCount && _lifetime[index] > 0;
    }
    
    public List<Particle> CreateParticles(int count, float posX, float posY, float velX, float velY, int colorArgb, float lifetime)
    {
        var createdParticles = new List<Particle>();
        int created = 0;
        
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
                
                createdParticles.Add(new Particle(i));
                created++;
            }
        }
        
        return createdParticles;
    }
    
    public void DeleteParticle(Particle particle)
    {
        if (particle != null && particle.Index >= 0 && particle.Index < ParticleCount)
        {
            _lifetime[particle.Index] = 0;
        }
    }
    
    public int GetAliveParticleCount()
    {
        int count = 0;
        for (int i = 0; i < ParticleCount; i++)
        {
            if (_lifetime[i] > 0)
                count++;
        }
        return count;
    }
}
