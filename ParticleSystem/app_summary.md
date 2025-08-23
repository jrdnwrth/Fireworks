# Objective

Create a particle system that simulates several different firework effects with visually realistic results in 2D.
Keep the code simple.  We don't have lots of time to spend on this.  It is just a hobby project.
Keep the code modular.  We want to be able to easily add new firework effects in the future.

# Technology

This particle system is implemented in C# using WPF on .NET 9, rendering particles directly to a 
WriteableBitmap for high-performance CPU-based simulation and visualization. Particle physics and 
rendering are handled manually, leveraging unsafe code for efficient pixel manipulation.

