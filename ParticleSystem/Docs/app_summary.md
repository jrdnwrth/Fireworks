# Objective

Create a particle system that simulates several different firework effects with visually realistic results in 2D.
Keep the code simple.  We don't have lots of time to spend on this.  It is just a hobby project.
Keep the code modular.  We want to be able to easily add new firework effects in the future.

# Technology

This particle system is implemented in C# using WPF on .NET 9, rendering particles directly to a 
WriteableBitmap for high-performance CPU-based simulation and visualization. Particle physics and 
rendering are handled manually, leveraging unsafe code for efficient pixel manipulation.

# File Structure

```
ParticleSystem/
│
├── App.xaml
├── App.xaml.cs
├── app_summary.md
│
├── Windows/
│   └── Particle_Window.xaml
│   └── Particle_Window.xaml.cs
│
├── Particles/
│   ├── Particle.cs                # Represents a single particle (optional, if you refactor to use objects)
│   ├── ParticleManager.cs         # Handles simulation and updates for all particles
│   ├── Effects/
│   │   ├── FireworkEffect.cs      # Example firework effect
│   │   └── [OtherEffect].cs       # Additional effects can be added here
│
├── Rendering/
│   └── BitmapRenderer.cs          # Handles drawing particles to WriteableBitmap
│
├── Resources/
│   └── [Images, Sounds, etc.]
│
└── Utils/
    └── [Helper classes, extensions, etc.]
```