using System.Xml.Linq;
using Xunit;
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
    public void Renders_geometry_and_transform_properties_for_composed_scenes()
    {
        var scene = GuiBuilder.Create("svg-scene", "scene")
            .Child("svg-ellipse", "ellipse", e => e.Property("cx", "20").Property("cy", "30")
                .Property("rx", "8").Property("ry", "4").Property("transform", "rotate(15 20 30)"))
            .Child("svg-rect", "rounded", r => r.Property("x", "2").Property("y", "3")
                .Property("width", "40").Property("height", "20").Property("rx", "3")
                .Property("fill", "none").Property("stroke", "#7ee0bd"))
            .Child("svg-polyline", "trace", p => p.Property("points", "0,0 10,15 20,5")
                .Property("fill", "none").Property("stroke", "#ffcf86"))
            .Child("svg-polygon", "facet", p => p.Property("points", "0,0 10,0 5,10")
                .Property("fill", "#7ee0bd"))
            .Child("svg-path", "curve", p => p.Property("d", "M0 0 C10 20 20 20 30 0")
                .Property("fill", "none").Property("stroke-width", "0.5"))
            .Build();

        var doc = XDocument.Parse(_renderer.Render(scene));
        XNamespace svg = "http://www.w3.org/2000/svg";

        Assert.NotNull(doc.Root!.Element(svg + "title"));
        Assert.Equal("rotate(15 20 30)", (string?)doc.Descendants(svg + "ellipse").Single().Attribute("transform"));
        Assert.Equal("3", (string?)doc.Descendants(svg + "rect").Single().Attribute("rx"));
        Assert.Equal("0,0 10,15 20,5", (string?)doc.Descendants(svg + "polyline").Single().Attribute("points"));
        Assert.NotNull(doc.Descendants(svg + "polygon").Single());
        Assert.Equal("M0 0 C10 20 20 20 30 0", (string?)doc.Descendants(svg + "path").Last().Attribute("d"));
    }

    [Fact]
    public void Emits_definitions_gradients_clips_and_masks()
    {
        var scene = GuiBuilder.Create("svg-scene", "scene")
            .Child("svg-defs", "defs", defs => defs
                .Child("svg-linear-gradient", "mint-gradient", gradient => gradient
                    .Property("x1", "0%").Property("y1", "0%").Property("x2", "100%").Property("y2", "0%")
                    .Child("svg-stop", "start", stop => stop.Property("offset", "0%").Property("stop-color", "#7ee0bd"))
                    .Child("svg-stop", "end", stop => stop.Property("offset", "100%").Property("stop-color", "#5777ff")))
                .Child("svg-clip-path", "clip", clip => clip
                    .Child("svg-circle", "clip-circle", circle => circle.Property("cx", "10").Property("cy", "10").Property("r", "8"))))
            .Child("svg-rect", "panel", rect => rect.Property("x", "0").Property("y", "0")
                .Property("width", "100").Property("height", "100")
                .Property("fill", "url(#mint-gradient)").Property("clip-path", "url(#clip)"))
            .Child("svg-mask", "soft-mask", mask => mask.Property("maskUnits", "userSpaceOnUse")
                .Child("svg-rect", "mask-rect", rect => rect.Property("width", "100").Property("height", "100")
                    .Property("fill", "white")))
            .Build();

        var doc = XDocument.Parse(_renderer.Render(scene));
        XNamespace svg = "http://www.w3.org/2000/svg";

        Assert.NotNull(doc.Descendants(svg + "linearGradient").Single());
        Assert.Equal(2, doc.Descendants(svg + "stop").Count());
        Assert.NotNull(doc.Descendants(svg + "clipPath").Single());
        Assert.NotNull(doc.Descendants(svg + "mask").Single());
        Assert.Equal("url(#mint-gradient)", (string?)doc.Descendants(svg + "rect").First().Attribute("fill"));
    }

    [Fact]
    public void Emits_image_text_spans_and_accessible_description()
    {
        var scene = GuiBuilder.Create("svg-scene", "scene")
            .Text("Workshop map")
            .Property("description", "A map of the demonstration floor.")
            .Child("svg-image", "map", image => image.Image("https://example.invalid/map.png")
                .Text("Demonstration floor map").Property("x", "5").Property("y", "5")
                .Property("width", "90").Property("height", "50"))
            .Child("svg-text", "caption", text => text.Property("x", "5").Property("y", "70")
                .Property("font-size", "4").Property("text-anchor", "start")
                .Child("svg-tspan", "line-one", span => span.Text("Line one"))
                .Child("svg-tspan", "line-two", span => span.Text("Line two").Property("x", "5").Property("y", "76")))
            .Build();

        var doc = XDocument.Parse(_renderer.Render(scene));
        XNamespace svg = "http://www.w3.org/2000/svg";

        Assert.Equal("Workshop map", (string?)doc.Root!.Attribute("aria-label"));
        Assert.Equal("A map of the demonstration floor.", doc.Root.Element(svg + "desc")!.Value);
        Assert.Equal("https://example.invalid/map.png", (string?)doc.Descendants(svg + "image").Single().Attribute("href"));
        Assert.Equal("Demonstration floor map", (string?)doc.Descendants(svg + "image").Single().Attribute("aria-label"));
        Assert.Equal(new[] { "Line one", "Line two" }, doc.Descendants(svg + "tspan").Select(x => x.Value).ToArray());
    }

    [Fact]
    public void Emits_explicit_svg_animation_and_transform_nodes_from_gui_model()
    {
        var scene = GuiBuilder.Create("svg-scene", "scene")
            .Child("svg-circle", "marker", marker => marker
                .Property("cx", "50").Property("cy", "25").Property("r", "2")
                .Child("svg-animate", "pulse", animation => animation
                    .Property("attributeName", "r").Property("values", "2;4;2")
                    .Property("dur", "2s").Property("repeatCount", "indefinite"))
                .Child("svg-animate-transform", "spin", animation => animation
                    .Property("attributeName", "transform").Property("type", "rotate")
                    .Property("from", "0").Property("to", "360")
                    .Property("dur", "4s").Property("repeatCount", "indefinite"))
                .Child("svg-animate-motion", "motion", motion => motion
                    .Property("path", "M10 10L90 10").Property("dur", "4s")
                    .Property("repeatCount", "indefinite")))
            .Build();

        var svg = _renderer.Render(scene);

        Assert.Contains("<animate", svg);
        Assert.Contains("<animateTransform", svg);
        Assert.Contains("<animateMotion", svg);
        Assert.Contains("path=\"M10 10L90 10\"", svg);
        Assert.Contains("values=\"2;4;2\"", svg);
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

    [Fact]
    public void Rejects_negative_geometry_dimensions()
    {
        var scene = GuiBuilder.Create("svg-scene", "scene")
            .Child("svg-circle", "invalid", circle => circle.Property("r", "-1"))
            .Build();

        Assert.Throws<ArgumentOutOfRangeException>(() => _renderer.Render(scene));
    }
}
