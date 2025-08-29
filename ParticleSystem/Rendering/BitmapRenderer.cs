using ParticleSystem.Particles;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ParticleSystem.Rendering;

// Handles drawing particles to WriteableBitmap
public unsafe class BitmapRenderer
{
    private readonly WriteableBitmap _backBuffer;
    private readonly int _screenWidth;
    private readonly int _screenHeight;
    private readonly Random _random = new();
    
    // Glow intensity arrays for different particle sizes (1, 3, 5, 7, 9)
    private static readonly Dictionary<int, float[,]> _glowIntensityArrays = new()
    {
        [1] = new float[1, 1] { { 1.4f } },
        
        [3] = new float[3, 3]
        {
            { 0.25f, 0.45f, 0.25f },
            { 0.45f, 1.0f, 0.45f },
            { 0.25f, 0.45f, 0.25f }
        },
        
        [5] = new float[5, 5]
        {
            { 0.51f, 0.31f, 0.51f, 0.31f, 0.21f },
            { 0.31f, 0.99f, 0.99f, 0.70f, 0.31f },
            { 0.51f, 0.99f, 1.0f, 0.99f, 0.51f },
            { 0.31f, 0.70f, 0.99f, 0.99f, 0.31f },
            { 0.21f, 0.31f, 0.51f, 0.31f, 0.51f }
        },
        
        [7] = new float[7, 7]
        {
            { 0.30f, 0.20f, 0.30f, 0.40f, 0.30f, 0.20f, 0.10f },
            { 0.20f, 0.60f, 0.60f, 0.70f, 0.60f, 0.35f, 0.20f },
            { 0.30f, 0.60f, 0.99f, 1.00f, 0.99f, 0.60f, 0.30f },
            { 0.40f, 0.70f, 1.00f, 1.00f, 1.00f, 0.70f, 0.40f },
            { 0.30f, 0.60f, 0.99f, 1.00f, 0.99f, 0.60f, 0.30f },
            { 0.20f, 0.35f, 0.60f, 0.70f, 0.60f, 0.60f, 0.20f },
            { 0.10f, 0.20f, 0.30f, 0.40f, 0.30f, 0.20f, 0.30f }
        },

        [9] = new float[9, 9]
        {
            { 0.30f, 0.10f, 0.21f, 0.31f, 0.40f, 0.31f, 0.21f, 0.10f, 0.05f },
            { 0.10f, 0.61f, 0.61f, 0.71f, 0.81f, 0.71f, 0.61f, 0.31f, 0.10f },
            { 0.21f, 0.99f, 0.99f, 1.00f, 1.00f, 1.00f, 0.99f, 0.61f, 0.21f },
            { 0.31f, 0.71f, 1.00f, 1.00f, 1.00f, 1.00f, 1.00f, 0.71f, 0.31f },
            { 0.40f, 0.81f, 1.00f, 1.00f, 1.00f, 1.00f, 1.00f, 0.81f, 0.40f },
            { 0.31f, 0.71f, 1.00f, 1.00f, 1.00f, 1.00f, 1.00f, 0.71f, 0.31f },
            { 0.21f, 0.61f, 0.99f, 1.00f, 1.00f, 1.00f, 0.99f, 0.61f, 0.21f },
            { 0.10f, 0.31f, 0.61f, 0.71f, 0.81f, 0.71f, 0.61f, 0.99f, 0.10f },
            { 0.05f, 0.10f, 0.21f, 0.31f, 0.40f, 0.31f, 0.21f, 0.10f, 0.30f }
        }
    };
    
    public WriteableBitmap BackBuffer => _backBuffer;
    
    public BitmapRenderer(int screenWidth, int screenHeight)
    {
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _backBuffer = new WriteableBitmap(screenWidth, screenHeight, 96, 96, PixelFormats.Pbgra32, null);
    }
    
    public void DrawParticles(float[] posX, float[] posY, int[] colorArgb)
    {
        // Get particle data with sizes and types
        ParticleManager.GetParticleData(out float[] x, out float[] y, out int[] colors, out float[] sizes, out byte[] types);
        
        _backBuffer.Lock();
        try
        {
            ClearBuffer();
            RenderParticles(x, y, colors, sizes, types);
            _backBuffer.AddDirtyRect(new Int32Rect(0, 0, _screenWidth, _screenHeight));
        }
        finally
        {
            _backBuffer.Unlock();
        }
    }
    
    private void ClearBuffer()
    {
        int stride = _backBuffer.BackBufferStride;
        int totalBytes = stride * _screenHeight;
        new Span<byte>((void*)_backBuffer.BackBuffer, totalBytes).Clear();
    }
    
    private static int RoundToOddSize(float size)
    {
        // Allow size 0 for invisible particles (strobe effects)
        if (size <= 0) return 0;
        
        // Round to nearest odd integer, clamped between 1 and 9
        int rounded = (int)Math.Round(size);
        if (rounded <= 1) return 1;
        if (rounded >= 9) return 9;
        
        // Make sure it's odd
        if (rounded % 2 == 0)
        {
            // If even, round to nearest odd (prefer smaller for sizes < 5, larger for sizes >= 5)
            return rounded < 5 ? rounded - 1 : rounded + 1;
        }
        
        return rounded;
    }
    
    /// <summary>
    /// Calculates the effective size for a particle based on its type and lifetime progress
    /// </summary>
    private float CalculateEffectiveSize(int particleIndex, float originalSize, ParticleType particleType)
    {
        // Get particle lifetime progress from ParticleManager
        float lifetimeProgress = ParticleManager.GetParticleLifetimeProgress(particleIndex);
        
        return particleType switch
        {
            ParticleType.FixedSize => originalSize,
            ParticleType.Decay => CalculateDecaySize(originalSize, lifetimeProgress),
            ParticleType.DecayHemisphere => CalculateDecayHemisphereSize(originalSize, lifetimeProgress),
            ParticleType.Strobe => CalculateStrobeSize(originalSize, lifetimeProgress),
            ParticleType.Flicker => CalculateFlickerSize(originalSize, lifetimeProgress),
            ParticleType.Flash => CalculateFlashSize(originalSize, lifetimeProgress),
            _ => originalSize
        };
    }
    
    private static float CalculateDecaySize(float originalSize, float lifetimeProgress)
    {
        // Decay: Standard particle that decreases size over its lifetime
        // lifetimeProgress goes from 1.0 (birth) to 0.0 (death)
        // We want size to decrease linearly with remaining lifetime
        return originalSize * lifetimeProgress;
    }
    private static float CalculateDecayHemisphereSize(float originalSize, float lifetimeProgress)
    {
        return originalSize * (float)Math.Cos(1f-lifetimeProgress);
    }

    private float CalculateStrobeSize(float originalSize, float lifetimeProgress)
    {
        // Strobe: Particle that oscillates between size 0 and its initial size
        // Use sine wave for smooth oscillation, with higher frequency for more strobing
        float strobeFrequency = 12.0f; // Oscillations over lifetime
        float phase = (1.0f - lifetimeProgress) * strobeFrequency * MathF.PI * 2;
        float strobeValue = (MathF.Sin(phase) + 1.0f) * 0.5f; // Normalize to 0-1
        
        return originalSize * strobeValue;
    }
    
    private float CalculateFlickerSize(float originalSize, float lifetimeProgress)
    {
        // Flicker: Particle that flickers with random size variations above size 0
        // Base size decreases with lifetime, but with random variations added
        float baseSize = originalSize * lifetimeProgress * 0.4f; // Minimum size (40% of original)
        float flickerAmount = originalSize * 0.6f; // Maximum additional size
        float randomFactor = (float)_random.NextDouble();
        
        return baseSize + (flickerAmount * randomFactor);
    }
    
    private float CalculateFlashSize(float originalSize, float lifetime)
    {
        // Flash: Particle that creates a bright flash effect at the very last 0.04 seconds of its life
        const float flashThreshold = 0.04f; 

        // lifetimeProgress goes from 1.0 (birth) to 0.0 (death)
        if (lifetime <= flashThreshold)
        {
            // Create intense flash effect at the end
            float randomSize = (float)_random.NextDouble() + 0.1f;
            return originalSize * randomSize;
        }
        else
            return 0;
    }
   
    
    private void RenderParticles(float[] posX, float[] posY, int[] colorArgb, float[] sizes, byte[] types)
    {
        int stride = _backBuffer.BackBufferStride;
        byte* basePtr = (byte*)_backBuffer.BackBuffer;

        for (int i = 0; i < ParticleManager.ParticleCount; i++)
        {
            // Only render particles that are alive
            if (!ParticleManager.IsParticleAlive(i))
                continue;
            
            // Calculate effective size based on particle type
            ParticleType particleType = (ParticleType)types[i];
            float effectiveSize = CalculateEffectiveSize(i, sizes[i], particleType);
            
            // Get particle size and round to odd integer
            int particleSize = RoundToOddSize(effectiveSize);
            
            // Skip rendering if particle size is 0 (invisible particle for strobe effects)
            if (particleSize == 0)
                continue;
                
            float[,] glowIntensity = _glowIntensityArrays[particleSize];
            
            // Center the particle around the position
            int halfSize = particleSize / 2;
            int centerX = (int)posX[i] - halfSize;
            int centerY = (int)posY[i] - halfSize;
            
            // Extract original particle color channels
            byte srcR = (byte)((colorArgb[i] >> 16) & 0xFF);
            byte srcG = (byte)((colorArgb[i] >> 8) & 0xFF);
            byte srcB = (byte)(colorArgb[i] & 0xFF);

            // Draw particle with appropriate glow pattern
            for (int dy = 0; dy < particleSize; dy++)
            {
                int y = centerY + dy;
                if ((uint)y >= (uint)_screenHeight) continue;
                byte* row = basePtr + y * stride;
                
                for (int dx = 0; dx < particleSize; dx++)
                {
                    int x = centerX + dx;
                    if ((uint)x >= (uint)_screenWidth) continue;
                    
                    int* pixelPtr = (int*)(row + (x << 2));
                    int dst = *pixelPtr;
                    
                    float intensity = glowIntensity[dy, dx];
                    
                    // For center pixel, use full white
                    byte glowR, glowG, glowB;
                    if (intensity == 1.0f)
                    {
                        glowR = glowG = glowB = 255; // Full white center
                    }
                    else
                    {
                        // Apply glow intensity to original color
                        glowR = (byte)Math.Min(255, srcR * intensity);
                        glowG = (byte)Math.Min(255, srcG * intensity);
                        glowB = (byte)Math.Min(255, srcB * intensity);
                    }
                    
                    // Extract destination channels
                    byte dstA = (byte)((dst >> 24) & 0xFF);
                    byte dstR = (byte)((dst >> 16) & 0xFF);
                    byte dstG = (byte)((dst >> 8) & 0xFF);
                    byte dstB = (byte)(dst & 0xFF);

                    // Realistic overexposure blending - spill excess into other channels
                    int totalR = glowR + dstR;
                    int totalG = glowG + dstG;
                    int totalB = glowB + dstB;
                    
                    // Calculate overflow amounts
                    int overflowR = Math.Max(0, totalR - 255);
                    int overflowG = Math.Max(0, totalG - 255);
                    int overflowB = Math.Max(0, totalB - 255);
                    
                    // Distribute overflow to create white overexposure effect
                    // Each overflow contributes to the other two channels
                    totalG = Math.Min(255, totalG + (overflowR + overflowB) / 2);
                    totalB = Math.Min(255, totalB + (overflowR + overflowG) / 2);
                    totalR = Math.Min(255, totalR + (overflowG + overflowB) / 2);
                    
                    byte outA = (byte)Math.Min(255 + dstA, 255);
                    byte outR = (byte)totalR;
                    byte outG = (byte)totalG;
                    byte outB = (byte)totalB;

                    *pixelPtr = (outA << 24) | (outR << 16) | (outG << 8) | outB;
                }
            }
        }
    }
}
