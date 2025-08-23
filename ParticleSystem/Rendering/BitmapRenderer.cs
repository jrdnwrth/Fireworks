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
    private readonly int _particleSize;
    
    public WriteableBitmap BackBuffer => _backBuffer;
    
    public BitmapRenderer(int screenWidth, int screenHeight, int particleSize = 5)
    {
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _particleSize = particleSize;
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
            int px = (int)posX[i];
            int py = (int)posY[i];
            if ((uint)px >= (uint)_screenWidth || (uint)py >= (uint)_screenHeight) continue;

            int* pixel = (int*)(basePtr + py * stride + (px << 2));
            *pixel = colorArgb[i];

            if (_particleSize > 1)
            {
                for (int dy = 0; dy < _particleSize; dy++)
                {
                    int y = py + dy;
                    if ((uint)y >= (uint)_screenHeight) break;
                    byte* row = basePtr + y * stride;
                    for (int dx = 0; dx < _particleSize; dx++)
                    {
                        int x = px + dx;
                        if ((uint)x >= (uint)_screenWidth) break;
                        *(int*)(row + (x << 2)) = colorArgb[i];
                    }
                }
            }
        }
    }
}
