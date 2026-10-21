namespace TheSingularityWorkshop.GUI.SVG;

/// <summary>Controls safe, deterministic SVG serialization.</summary>
public sealed record SemanticGuiSvgRenderOptions
{
    /// <summary>Optional accessible title for the SVG document.</summary>
    public string? Title { get; init; }

    /// <summary>Optional longer accessible description.</summary>
    public string? Description { get; init; }

    /// <summary>SVG viewBox width and height in user units.</summary>
    public int Width { get; init; } = 800;

    /// <summary>SVG viewBox height in user units.</summary>
    public int Height { get; init; } = 600;

    /// <summary>Background color. Null leaves the canvas transparent.</summary>
    public string? Background { get; init; }

    /// <summary>Reject dimensions that cannot form a valid positive viewBox.</summary>
    internal void Validate()
    {
        if (Width <= 0) throw new ArgumentOutOfRangeException(nameof(Width));
        if (Height <= 0) throw new ArgumentOutOfRangeException(nameof(Height));
        if (Background is not null && !IsSafeColor(Background))
            throw new ArgumentException("Background must be a named CSS color or a hex color (#RGB, #RRGGBB, or #RRGGBBAA).", nameof(Background));
    }

    private static bool IsSafeColor(string value)
    {
        if ((value.Length is 4 or 7 or 9) && value[0] == '#')
            return value.AsSpan(1).ToString().All(Uri.IsHexDigit);
        return value.Length > 0 && value.All(char.IsAsciiLetter);
    }
}
