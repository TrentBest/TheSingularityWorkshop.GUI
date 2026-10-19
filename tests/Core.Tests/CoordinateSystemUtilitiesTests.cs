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

    [Theory]
    [InlineData(double.NaN, 10)]
    [InlineData(double.PositiveInfinity, 10)]
    [InlineData(double.NegativeInfinity, 10)]
    [InlineData(0, double.NaN)]
    [InlineData(0, double.PositiveInfinity)]
    [InlineData(0, double.NegativeInfinity)]
    public void Normalize_rejects_non_finite_range_endpoints(double minimum, double maximum)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CoordinateSystemUtilities.Normalize(5, minimum, maximum));
    }

    [Theory]
    [InlineData(double.NaN, 10)]
    [InlineData(double.PositiveInfinity, 10)]
    [InlineData(double.NegativeInfinity, 10)]
    [InlineData(0, double.NaN)]
    [InlineData(0, double.PositiveInfinity)]
    [InlineData(0, double.NegativeInfinity)]
    public void Denormalize_rejects_non_finite_range_endpoints(double minimum, double maximum)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CoordinateSystemUtilities.Denormalize(5, minimum, maximum));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Normalize_rejects_non_finite_value(double value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CoordinateSystemUtilities.Normalize(value, 0, 100));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Denormalize_rejects_non_finite_normalized_value(double value)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CoordinateSystemUtilities.Denormalize(value, 0, 100));
    }
}
