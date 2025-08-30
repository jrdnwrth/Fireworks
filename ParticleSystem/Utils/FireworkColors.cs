using System;

namespace ParticleSystem.Utils;

/// <summary>
/// Standardized firework colors that look realistic and vibrant.
/// Colors are stored as RGB integers (0xRRGGBB format) without alpha channel.
/// </summary>
public static class FireworkColors
{
    // Primary firework colors (matching your existing colors)
    public const int BrightRed = 0xFF0000;        // Bright red - classic firework color
    public const int BrightBlue = 0x0000FF;       // Bright blue - vivid sky blue
    public const int BrightGreen = 0x009900;      // Bright green - emerald green
    public const int Gold = 0x884400;             // Gold - warm golden yellow

    // Additional firework colors for variety
    public const int PurpleViolet = 0x8800FF;     // Purple violet - royal purple
    public const int Orange = 0xFF4400;           // Orange - sunset orange
    public const int HotPink = 0xFF0088;          // Hot pink - magenta pink
    public const int White = 0xCCCCCC;            // White - brilliant white
    public const int Silver = 0x666666;           // Silver - metallic silver
    public const int Copper = 0xBB4400;           // Copper - reddish copper
    
    // Specialty colors
    public const int ElectricBlue = 0x0088FF;     // Electric blue - bright cyan-blue
    public const int LimeGreen = 0x44FF00;        // Lime green - bright yellow-green
    public const int DeepRed = 0xAA0000;          // Deep red - darker red for contrast
    public const int ChampagneGold = 0xFFDD44;    // Champagne gold - lighter gold
    
    /// <summary>
    /// Gets a random firework color from the primary color palette.
    /// </summary>
    /// <param name="random">Random number generator to use</param>
    /// <returns>A random firework color as RGB integer</returns>
    public static int GetRandomPrimaryColor(Random random)
    {
        int[] primaryColors = { BrightRed, BrightBlue, BrightGreen, Gold };
        return primaryColors[random.Next(primaryColors.Length)];
    }
    
    /// <summary>
    /// Gets a random firework color from the full color palette.
    /// </summary>
    /// <param name="random">Random number generator to use</param>
    /// <returns>A random firework color as RGB integer</returns>
    public static int GetRandomColor(Random random)
    {
        int[] allColors = 
        {
            BrightRed, BrightBlue, BrightGreen,
        };
        return allColors[random.Next(allColors.Length)];
    }
    
    /// <summary>
    /// Gets colors that complement the specified color for multi-stage fireworks.
    /// </summary>
    /// <param name="baseColor">The base color to find complements for</param>
    /// <returns>Array of complementary colors</returns>
    //public static int[] GetComplementaryColors(int baseColor)
    //{
    //    return baseColor switch
    //    {
    //        BrightRed => [Gold, White, Silver],
    //        BrightBlue => [White, Silver, ChampagneGold],
    //        BrightGreen => [Gold, Copper, White],
    //        Gold => [BrightRed, DeepRed, White],
    //        PurpleViolet => [Gold, White, Silver],
    //        Orange => [BrightBlue, White, ElectricBlue],
    //        _ => [White, Silver, Gold]
    //    };
    //}
}