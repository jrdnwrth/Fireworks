using System;
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
    private const int _particleSize = 5; // Hard-coded to 5 pixels
    
    // Pre-calculated glow intensity multipliers for 5x5 grid based on distance from center
    private static readonly float[,] _glowIntensity = new float[5, 5]
    {
        { 0.05f, 0.10f, 0.15f, 0.10f, 0.05f },
        { 0.10f, 0.25f, 0.45f, 0.25f, 0.10f },
        { 0.15f, 0.45f, 1.0f, 0.45f, 0.15f },
        { 0.10f, 0.25f, 0.45f, 0.25f, 0.10f },
        { 0.05f, 0.10f, 0.15f, 0.10f, 0.05f }
    };
    
    public WriteableBitmap BackBuffer => _backBuffer;
    
    public BitmapRenderer(int screenWidth, int screenHeight, int particleSize = 5)
    {
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _backBuffer = new WriteableBitmap(screenWidth, screenHeight, 96, 96, PixelFormats.Pbgra32, null);
    }
    
    public void DrawParticles(float[] posX, float[] posY, int[] colorArgb, int particleCount)
    {
        _backBuffer.Lock();
        try
        {
            ClearBuffer();
            RenderParticles(posX, posY, colorArgb, particleCount);
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
    
    private void RenderParticles(float[] posX, float[] posY, int[] colorArgb, int particleCount)
    {
        int stride = _backBuffer.BackBufferStride;
        byte* basePtr = (byte*)_backBuffer.BackBuffer;

        for (int i = 0; i < particleCount; i++)
        {
            // Center the particle around the position
            int centerX = (int)posX[i] - 2; // Offset by 2 to center 5x5 grid
            int centerY = (int)posY[i] - 2;
            
            // Extract original particle color channels
            byte srcR = (byte)((colorArgb[i] >> 16) & 0xFF);
            byte srcG = (byte)((colorArgb[i] >> 8) & 0xFF);
            byte srcB = (byte)(colorArgb[i] & 0xFF);

            // Draw 5x5 glow pattern
            for (int dy = 0; dy < _particleSize; dy++)
            {
                int y = centerY + dy;
                if ((uint)y >= (uint)_screenHeight) continue;
                byte* row = basePtr + y * stride;
                
                for (int dx = 0; dx < _particleSize; dx++)
                {
                    int x = centerX + dx;
                    if ((uint)x >= (uint)_screenWidth) continue;
                    
                    int* pixelPtr = (int*)(row + (x << 2));
                    int dst = *pixelPtr;
                    
                    float intensity = _glowIntensity[dy, dx];
                    
                    // For center pixel (2,2), use full white
                    byte glowR, glowG, glowB;
                    if (dx == 2 && dy == 2)
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

                    // Additive blend with clamping
                    byte outA = (byte)Math.Min(255 + dstA, 255);
                    byte outR = (byte)Math.Min(glowR + dstR, 255);
                    byte outG = (byte)Math.Min(glowG + dstG, 255);
                    byte outB = (byte)Math.Min(glowB + dstB, 255);

                    *pixelPtr = (outA << 24) | (outR << 16) | (outG << 8) | outB;
                }
            }
        }
    }
}
