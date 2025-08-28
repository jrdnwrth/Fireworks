using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using ParticleSystem.Particles;
using ParticleSystem.Rendering;
using ParticleSystem.Utils;

namespace ParticleSystem;

public partial class Particle_Window : Window
{
    private readonly BitmapRenderer _renderer;
    private readonly System.Windows.Controls.Image _image;
    private readonly Stopwatch _stopwatch = new Stopwatch();
    private long _lastTicks;
    const int width = 800;
    const int height = 900;
    const int particleCount = 50000;

    public Particle_Window()
    {
        InitializeComponent();

        InitializeWindow(width, height);
        
        // Initialize the static ParticleManager
        ParticleManager.Initialize(particleCount, width, height);
        
        _renderer = new BitmapRenderer(width, height);
        
        _image = new System.Windows.Controls.Image 
        { 
            Source = _renderer.BackBuffer, 
            Stretch = Stretch.None 
        };
        Content = _image;

        StartSimulation();
    }

    private void InitializeWindow(int width, int height)
    {
        Title = "WPF CPU particles (WriteableBitmap)";
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Width = width;
        Height = height;
        ResizeMode = ResizeMode.NoResize;
        SnapsToDevicePixels = true;
        Background = Brushes.Black;
    }

    private void StartSimulation()
    {
        CompositionTarget.Rendering += OnRender;
        _stopwatch.Start();
        _lastTicks = _stopwatch.ElapsedTicks;
    }
    bool toggle = false;
    private void OnRender(object? sender, EventArgs e)
    {
        long now = _stopwatch.ElapsedTicks;
        float dt = (now - _lastTicks) / (float)Stopwatch.Frequency;
        _lastTicks = now;
        if (dt > 0.05f) dt = 0.05f;

        // Update all emitters via EmitterManager
        EmitterManager.Update(dt, height);

        // Check if there are no more alive emitters and create a new chained firework
        int activeEmitterCount = EmitterManager.GetActiveEmitterCount();
        if (activeEmitterCount == 0)
        {
            var ground = 870f;
            
            //Willow.Create(FireworkColors.Gold, 250f, ground, toggle);
            //Chrysanthemum.Create(FireworkColors.BrightBlue, 350f, ground);
            //Chrysanthemum.Create(FireworkColors.DeepRed, 400f, ground);
            //Chrysanthemum.Create(FireworkColors.BrightGreen, 500f, ground);
            Palm.Create(FireworkColors.Gold, 450f, ground);
            toggle = !toggle;

        }

        ParticleManager.Update(dt);
        
        ParticleManager.GetParticleData(out float[] posX, out float[] posY, out int[] colorArgb);
        _renderer.DrawParticles(posX, posY, colorArgb);
    }
}

