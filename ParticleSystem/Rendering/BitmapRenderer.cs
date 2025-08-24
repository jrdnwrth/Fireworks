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
    
    // Glow intensity arrays for different particle sizes (1, 3, 5, 7, 9)
    private static readonly Dictionary<int, float[,]> _glowIntensityArrays = new()
    {
        [1] = new float[1, 1] { { 1.0f } },
        
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
        // Get particle data with sizes
        ParticleManager.GetParticleData(out float[] x, out float[] y, out int[] colors, out float[] sizes);
        
        _backBuffer.Lock();
        try
        {
            ClearBuffer();
            RenderParticles(x, y, colors, sizes);
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
    
    private void RenderParticles(float[] posX, float[] posY, int[] colorArgb, float[] sizes)
    {
        int stride = _backBuffer.BackBufferStride;
        byte* basePtr = (byte*)_backBuffer.BackBuffer;

        for (int i = 0; i < ParticleManager.ParticleCount; i++)
        {
            // Only render particles that are alive
            if (!ParticleManager.IsParticleAlive(i))
                continue;
            
            // Get particle size and round to odd integer
            int particleSize = RoundToOddSize(sizes[i]);
            
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
                    if (intensity >= 1.0f)
                    {
                        glowR = glowG = glowB = 255; // Full white center
                    }
                    else
                    {
                        // Apply glow intensity to original color
                        glowR = (byte)(srcR * intensity);
                        glowG = (byte)(srcG * intensity);
                        glowB = (byte)(srcB * intensity);
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
