using System;
using System.Collections.Generic;

namespace ParticleSystem.Utils;

public static class MathUtils
{
    private static Random? _random = null;
    public static Random random
    {
        get
        {
            if (_random is null)
                _random = new Random();
            return _random;
        }
    }

    public static float random_float() => random.NextSingle();
    public static float random_float(float min, float max) => min + (random_float() * (max - min));

    /// <summary>
    /// Generates a random boolean value (true or false).
    /// </summary>
    /// <returns>A random boolean value</returns>
    public static bool random_bool() => random.Next(2) == 1;

    /// <summary>
    /// Generates a normally distributed random number using the Box-Muller transform.
    /// Maintains state for spare normal value.
    /// </summary>
    public static float NextGaussian(ref bool hasSpareNormal, ref float spareNormal)
    {
        if (hasSpareNormal)
        {
            hasSpareNormal = false;
            return spareNormal;
        }

        hasSpareNormal = true;

        float u1, u2;
        do
        {
            u1 = (float)random.NextDouble();
            u2 = (float)random.NextDouble();
        }
        while (u1 <= float.Epsilon);

        float magnitude = (float)(Math.Sqrt(-2.0 * Math.Log(u1)));
        float angle = (float)(2.0 * Math.PI * u2);

        spareNormal = magnitude * (float)Math.Sin(angle);
        return magnitude * (float)Math.Cos(angle);
    }

    /// <summary>
    /// Generates a sphere of points.
    /// X axis is horizontal
    /// Y axis is vertical
    /// Z axis is coming out of the screen.
    /// </summary>
    /// <param name="layers">The sphere is built in layers of points.  How many layers should we have?  This controls how many points are returned.</param>
    /// <returns></returns>
    public static IEnumerable<(float x, float y, float z)> generate_unit_sphere(float layers = 10f)
    {
        // PI
        var PI = 3.14159265f;

        // We can't have the slices always be perfectly horizontal. So rotate the final result this much.
        var final_rotation_on_z = random_float(0f, 2f * PI);
        var final_rotation_on_x = random_float(0f, 2f * PI);

        // Iterate through angles off the vertical.
        var total_vertical_radians = PI;
        var distance_between_points = total_vertical_radians / layers;  // This is both the angle (in radians) at the equator between points, and the standard distance between points as measured along the surface of the sphere.

        // Slightly rotate every other slice so they are not perfectly aligned.
        bool stagger_slices = true;

        // Vertical (Y-Axis)
        for (var v = 0f; v < total_vertical_radians; v += distance_between_points)
        {
            // Calculate the radius of a circle at this cross section of the sphere.
            var slice_radius = (float)Math.Sin(v);

            // Calculate the circumference of a circle
            var circum = 2f * PI * slice_radius;

            // How many points will fit on this cicle?
            var total_points = circum / distance_between_points;

            // How many radians is this on a unit circle?
            var step_radians = 2f * PI / total_points;

            // Start to end rotation.
            var start =  0f;
            var finish = 2f * PI;

            // Shift half a step_radians if we are staggering slices.
            stagger_slices = !stagger_slices;
            if (stagger_slices)
            {
                start += step_radians / 2f;
                finish += step_radians / 2f;
            }

            // Rotate around the horizontal circle
            for (var h = start; h < finish; h += step_radians)
            {
                // Calculate X
                var x = (float)Math.Cos(h) * slice_radius;

                // Calculate Z
                var z = (float)Math.Sin(h) * slice_radius;

                // Calculate Y
                var y = (float)Math.Cos(v);

                // Apply final rotation around z.  (These are euler angles, not quaternions.)
                var x_rotated = (float)(x * Math.Cos(final_rotation_on_z) - y * Math.Sin(final_rotation_on_z));
                var y_rotated = (float)(x * Math.Sin(final_rotation_on_z) + y * Math.Cos(final_rotation_on_z));

                // Appliy final rotation around x.
                var y_final = (float)(y_rotated * Math.Cos(final_rotation_on_x) - z * Math.Sin(final_rotation_on_x));
                var z_final = (float)(y_rotated * Math.Sin(final_rotation_on_x) + z * Math.Cos(final_rotation_on_x));

                yield return (x_rotated, y_final, z_final);
            }
        }
    }
}


