namespace ParticleSystem.Utils;

/// <summary>
/// Represents a 2D position with X and Y coordinates
/// </summary>
public readonly record struct Position(float X, float Y);

/// <summary>
/// Represents a 2D velocity with X and Y components
/// </summary>
public readonly record struct Velocity(float X, float Y);