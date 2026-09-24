using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Core.Tests;

public sealed class CoordinateSystemUtilitiesTests
{
    [Theory]
    [InlineData(-1, 0)]
    [InlineData(50, 50)]
    [InlineData(101, 100)]
    public void Clamp_enforces_bounds(double value, double expected)
    {
        Assert.Equal(expected, CoordinateSystemUtilities.Clamp(value));
    }

    [Fact]
    public void Normalize_and_denormalize_are_inverse_operations()
    {
        var normalized = CoordinateSystemUtilities.Normalize(25, 0, 200);
        var restored = CoordinateSystemUtilities.Denormalize(normalized, 0, 200);

        Assert.Equal(12.5, normalized);
        Assert.Equal(25, restored);
    }

    [Fact]
    public void Distance_uses_cartesian_distance()
    {
        Assert.Equal(5, CoordinateSystemUtilities.Distance(0, 0, 3, 4));
    }

    [Fact]
    public void Invalid_ranges_are_rejected()
    {
        Assert.Throws<ArgumentException>(() => CoordinateSystemUtilities.Normalize(1, 10, 10));
        Assert.Throws<ArgumentException>(() => CoordinateSystemUtilities.Denormalize(1, 10, 10));
    }
}
