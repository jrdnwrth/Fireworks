using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using ParticleSystem.Particles;
using ParticleSystem.Rendering;
using ParticleSystem.Utils;
using static ParticleSystem.Utils.MathUtils;

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

    // Firework launching control variables
    private readonly Random _random = new Random();
    private float _nextFireworkDelay = 0f;
    private float _timeSinceLastFirework = 0f;

    // Adjustable delay settings (in seconds)
    public float MinFireworkDelay = 0.4f;  // Minimum delay between fireworks
    public float MaxFireworkDelay = 2.6f;  // Maximum delay between fireworks

    // Firework types for random selection
    private readonly FireworkType[] _fireworkTypes =
    {
        FireworkType.Willow,
        FireworkType.Chrysanthemum,
        FireworkType.Chrysanthemum,
        FireworkType.Chrysanthemum2,
        FireworkType.Chrysanthemum2,
        FireworkType.Chrysanthemum3,
        FireworkType.Chrysanthemum3,
        FireworkType.Palm,
        FireworkType.Palm,
    };

    private enum FireworkType
    {
        Willow,
        Chrysanthemum,
        Chrysanthemum2,
        Chrysanthemum3,
        Palm
    }

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

        // Initialize first firework delay
        _nextFireworkDelay = _random.NextSingle() * (MaxFireworkDelay - MinFireworkDelay) + MinFireworkDelay;

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
        EmitterManager.Update(dt, height);

        // Check if there are no more alive emitters and enough time has passed
        int activeEmitterCount = EmitterManager.GetActiveEmitterCount();
        _timeSinceLastFirework += dt;

        if (_timeSinceLastFirework >= _nextFireworkDelay)
        {
            LaunchRandomFirework();

            // Reset timer and set next delay
            _timeSinceLastFirework = 0f;
            _nextFireworkDelay = _random.NextSingle() * (MaxFireworkDelay - MinFireworkDelay) + MinFireworkDelay;
        }

        ParticleManager.Update(dt);

        ParticleManager.GetParticleData(out float[] posX, out float[] posY, out int[] colorArgb);
        _renderer.DrawParticles(posX, posY, colorArgb);
    }

    private void LaunchRandomFirework()
    {
        var ground = 870f;
        var xPos = MathUtils.random_float(200f, 600f); // Random x position between 200-600
        var fireworkType = _fireworkTypes[MathUtils.random.Next(_fireworkTypes.Length)];
        var color = FireworkColors.GetRandomColor(MathUtils.random);

        switch (fireworkType)
        {
            case FireworkType.Willow:
                Willow.Create(FireworkColors.Gold, xPos, ground, random_bool());
                break;

            case FireworkType.Chrysanthemum:
                Chrysanthemum.Create(color, xPos, ground, random_bool());
                break;

            case FireworkType.Chrysanthemum2:
                Chrysanthemum2.Create(color, xPos, ground, random_bool());
                break;

            case FireworkType.Chrysanthemum3:
                Chrysanthemum3.Create(color, xPos, ground, random_bool(), random_bool());
                break;

            case FireworkType.Palm:
                Palm.Create(FireworkColors.Gold, xPos, ground, random_bool(), random_bool());
                break;
        }
    }
}

