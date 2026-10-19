using System;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>Provides normalization, clamping, and distance helpers for coordinates.</summary>
public static class CoordinateSystemUtilities
{
    /// <summary>Clamps a finite value to the inclusive supplied range.</summary>
    public static double Clamp(double value, double minimum = 0, double maximum = 100)
    {
        if (minimum > maximum)
            throw new ArgumentException("Minimum cannot exceed maximum.", nameof(minimum));

        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value), "Coordinate must be finite.");

        return Math.Clamp(value, minimum, maximum);
    }

    /// <summary>Maps a value from a source range to the normalized 0–100 range.</summary>
    public static double Normalize(double value, double minimum, double maximum)
    {
        ValidateFiniteRange(minimum, maximum);

        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value), "Coordinate must be finite.");

        return (value - minimum) / (maximum - minimum) * 100d;
    }

    /// <summary>Maps a normalized 0–100 value to a destination range.</summary>
    public static double Denormalize(double normalized, double minimum, double maximum)
    {
        ValidateFiniteRange(minimum, maximum);

        if (!double.IsFinite(normalized))
            throw new ArgumentOutOfRangeException(nameof(normalized), "Coordinate must be finite.");

        return minimum + normalized / 100d * (maximum - minimum);
    }

    /// <summary>Calculates Euclidean distance between two points.</summary>
    public static double Distance(double x1, double y1, double x2, double y2) =>
        Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));

    private static void ValidateFiniteRange(double minimum, double maximum)
    {
        if (!double.IsFinite(minimum))
            throw new ArgumentOutOfRangeException(nameof(minimum), "Range endpoint must be finite.");

        if (!double.IsFinite(maximum))
            throw new ArgumentOutOfRangeException(nameof(maximum), "Range endpoint must be finite.");

        if (minimum >= maximum)
            throw new ArgumentException("Minimum must be less than maximum.", nameof(minimum));
    }
}