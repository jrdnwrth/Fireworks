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
    private bool _hasCreatedNewFirework = false; // Track if we've created a new firework after emitters died

    public Particle_Window()
    {
        InitializeComponent();

        const int width = 800;
        const int height = 900;
        const int particleCount = 50000;

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

        //// Load emitter configurations from the dedicated configuration file
        //var emitters = EmitterConfigurations.CreateFireworkEmitters(width, height);
        //foreach (var emitter in emitters)
        //{
        //    EmitterManager.AddEmitter(emitter);
        //}
        ChainedFireworkConfigurations.CreateChainedFirework(width, height);

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

    private void OnRender(object? sender, EventArgs e)
    {
        long now = _stopwatch.ElapsedTicks;
        float dt = (now - _lastTicks) / (float)Stopwatch.Frequency;
        _lastTicks = now;
        if (dt > 0.05f) dt = 0.05f;

        // Update all emitters via EmitterManager
        EmitterManager.Update(dt);

        // Check if there are no more alive emitters and create a new chained firework
        int activeEmitterCount = EmitterManager.GetActiveEmitterCount();
        if (activeEmitterCount == 0 && !_hasCreatedNewFirework)
        {
            ChainedFireworkConfigurations.CreateChainedFirework(800, 900);
            _hasCreatedNewFirework = true;
        }
        else if (activeEmitterCount > 0)
        {
            // Reset the flag when emitters are active again
            _hasCreatedNewFirework = false;
        }

        ParticleManager.Update(dt);
        
        ParticleManager.GetParticleData(out float[] posX, out float[] posY, out int[] colorArgb);
        _renderer.DrawParticles(posX, posY, colorArgb);
    }
}

