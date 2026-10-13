using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;

namespace TheSingularityWorkshop.GUI.SVG.Tests;

public sealed class SvgGuiRendererTests
{
    private readonly SvgGuiRenderer _renderer = new();

    [Fact]
    public void Renders_coordinate_system_and_maps_cartesian_y_up_to_svg_y_down()
    {
        var scene = CoordinateSystemGuiBuilder.Create("scene")
            .Text("Coordinate proof")
            .Grid()
            .Axes()
            .Point("upper-right", 75, 80)
            .Build();

        var svg = _renderer.Render(scene);

        Assert.Contains("<svg", svg);
        Assert.Contains("scene-grid", svg);
        Assert.Contains("scene-axes", svg);
        Assert.Contains("id=\"upper-right\"", svg);
        Assert.Contains("cx=\"75\"", svg);
        Assert.Contains("cy=\"11.25\"", svg);
    }

    [Fact]
    public void Renders_svg_primitives_and_escapes_text()
    {
        var scene = GuiBuilder.Create("svg-scene", "scene")
            .Child("svg-circle", "marker", marker => marker
                .Property("cx", "50").Property("cy", "50").Property("r", "4")
                .Property("fill", "#ffcf86"))
            .Child("svg-text", "label", label => label.Text("A & B")
                .Property("x", "5").Property("y", "12"))
            .Build();

        var svg = _renderer.Render(scene);

        Assert.Contains("id=\"marker\"", svg);
        Assert.Contains("fill=\"#ffcf86\"", svg);
        Assert.Contains("A &amp; B", svg);
    }

    [Fact]
    public void Emits_explicit_svg_animation_nodes_from_gui_model()
    {
        var scene = GuiBuilder.Create("svg-scene", "scene")
            .Child("svg-circle", "marker", marker => marker
                .Property("cx", "50").Property("cy", "25").Property("r", "2")
                .Child("svg-animate-motion", "motion", motion => motion
                    .Property("path", "M10 10L90 10")
                    .Property("dur", "4s")
                    .Property("repeatCount", "indefinite")))
            .Build();

        var svg = _renderer.Render(scene);

        Assert.Contains("<animateMotion", svg);
        Assert.Contains("path=\"M10 10L90 10\"", svg);
        Assert.Contains("repeatCount=\"indefinite\"", svg);
        Assert.Contains("viewBox=\"0 0 100 56.25\"", svg);
    }

    [Fact]
    public void Rejects_non_positive_surface_dimensions()
    {
        var scene = GuiBuilder.Create("svg-scene", "scene").Build();

        Assert.Throws<ArgumentOutOfRangeException>(() => _renderer.Render(scene, width: 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => _renderer.Render(scene, height: -1));
    }

    [Fact]
    public void Rejects_invalid_coordinate_range()
    {
        var scene = GuiBuilder.Create("coordinate-system", "scene")
            .Property("minimum", "10")
            .Property("maximum", "10")
            .Build();

        Assert.Throws<ArgumentException>(() => _renderer.Render(scene));
    }
}
