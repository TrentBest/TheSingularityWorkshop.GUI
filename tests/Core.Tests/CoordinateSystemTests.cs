using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Core.Tests;

public sealed class CoordinateSystemTests
{
    [Fact]
    public void Normalized_coordinate_system_has_expected_contract()
    {
        var coordinates = CoordinateSystem.Normalized;

        Assert.Equal(0, coordinates.Minimum);
        Assert.Equal(100, coordinates.Maximum);
        Assert.Equal(50, coordinates.OriginX);
        Assert.Equal(50, coordinates.OriginY);
        Assert.True(coordinates.Contains(0, 0));
        Assert.True(coordinates.Contains(100, 100));
        Assert.False(coordinates.Contains(-1, 50));
    }

    [Fact]
    public void Coordinate_system_rejects_non_finite_values()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => CoordinateSystem.Normalized.ToPanel(double.NaN, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => CoordinateSystem.Normalized.ToPanel(10, double.PositiveInfinity));
    }

    [Fact]
    public void Coordinate_builder_preserves_coordinate_semantics()
    {
        var node = CoordinateSystemGuiBuilder.Create("canvas")
            .Axes()
            .Grid()
            .Origin()
            .Bounds()
            .Point("origin", 50, 50)
            .Build();

        Assert.Equal("coordinate-system", node.Kind);
        Assert.Equal("cartesian", node.Properties["coordinate-space"]);
        Assert.Equal("visible", node.Properties["axes"]);
        Assert.Single(node.Children);
        Assert.Equal("50", node.Find("origin").Properties["x"]);
        Assert.Equal("50", node.Find("origin").Properties["y"]);
    }
}
