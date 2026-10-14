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
                .Property("fill", "none").Property("stroke-width", "0.5").Property("marker-end", "url(#arrow)"))
            .Build();

        var doc = XDocument.Parse(_renderer.Render(scene));
        XNamespace svg = "http://www.w3.org/2000/svg";

        Assert.NotNull(doc.Root!.Element(svg + "title"));
        Assert.Equal("rotate(15 20 30)", (string?)doc.Descendants(svg + "ellipse").Single().Attribute("transform"));
        Assert.Equal("3", (string?)doc.Descendants(svg + "rect").Single().Attribute("rx"));
        Assert.Equal("0,0 10,15 20,5", (string?)doc.Descendants(svg + "polyline").Single().Attribute("points"));
        Assert.NotNull(doc.Descendants(svg + "polygon").Single());
        Assert.Equal("M0 0 C10 20 20 20 30 0", (string?)doc.Descendants(svg + "path").Last().Attribute("d"));
        Assert.Equal("url(#arrow)", (string?)doc.Descendants(svg + "path").Last().Attribute("marker-end"));
    }

    [Fact]
    public void Emits_definitions_gradients_clips_and_masks()
    {
        var scene = GuiBuilder.Create("svg-scene", "scene")
            .Child("svg-defs", "defs", defs => defs
                .Child("svg-linear-gradient", "mint-gradient", gradient => gradient
                    .Property("href", "#base-gradient")
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
        Assert.Equal("#base-gradient", (string?)doc.Descendants(svg + "linearGradient").Single().Attribute("href"));
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
    public void Emits_navigable_svg_links_and_semantic_metadata_nodes()
    {
        var scene = GuiBuilder.Create("svg-scene", "scene")
            .Child("svg-a", "guide-link", link => link
                .Property("href", "https://example.invalid/guide")
                .Property("target", "_blank")
                .Property("rel", "noopener")
                .Child("svg-text", "link-label", label => label.Text("Open guide")
                    .Property("x", "5").Property("y", "10")))
            .Child("svg-desc", "scene-description", description => description.Text("A navigable learning diagram."))
            .Child("svg-metadata", "scene-metadata", metadata => metadata.Text("Workshop sample metadata"))
            .Child("svg-view", "detail-view", view => view.Property("viewBox", "0 0 50 50")
                .Property("preserveAspectRatio", "xMidYMid meet"))
            .Build();

        var doc = XDocument.Parse(_renderer.Render(scene));
        XNamespace svg = "http://www.w3.org/2000/svg";

        var link = doc.Descendants(svg + "a").Single();
        Assert.Equal("https://example.invalid/guide", (string?)link.Attribute("href"));
        Assert.Equal("_blank", (string?)link.Attribute("target"));
        Assert.Equal("noopener", (string?)link.Attribute("rel"));
        Assert.Equal("Open guide", link.Descendants(svg + "text").Single().Value);
        Assert.Equal("A navigable learning diagram.", doc.Descendants(svg + "desc").Single().Value);
        Assert.Equal("Workshop sample metadata", doc.Descendants(svg + "metadata").Single().Value);
        Assert.Equal("0 0 50 50", (string?)doc.Descendants(svg + "view").Single().Attribute("viewBox"));
    }

    [Fact]
    public void Rejects_svg_links_without_a_destination()
    {
        var scene = GuiBuilder.Create("svg-scene", "scene")
            .Child("svg-a", "empty-link", _ => { })
            .Build();

        Assert.Throws<ArgumentException>(() => _renderer.Render(scene));
    }

    [Fact]
    public void Renders_reusable_symbols_patterns_markers_and_filter_primitives()
    {
        var scene = GuiBuilder.Create("svg-scene", "scene")
            .Child("svg-defs", "defs", defs => defs
                .Child("svg-symbol", "marker-symbol", symbol => symbol
                    .Property("viewBox", "0 0 10 10")
                    .Child("svg-circle", "symbol-dot", dot => dot.Property("cx", "5").Property("cy", "5").Property("r", "4")))
                .Child("svg-pattern", "stripe-pattern", pattern => pattern
                    .Property("width", "8").Property("height", "8").Property("patternUnits", "userSpaceOnUse")
                    .Child("svg-path", "stripe", path => path.Property("d", "M0 0L8 8").Property("stroke", "#7ee0bd")))
                .Child("svg-marker", "arrowhead", marker => marker
                    .Property("refX", "5").Property("refY", "5").Property("markerWidth", "6")
                    .Property("markerHeight", "6").Property("orient", "auto")
                    .Child("svg-polygon", "arrow", polygon => polygon.Property("points", "0,0 10,5 0,10")))
                .Child("svg-filter", "soft-glow", filter => filter
                    .Child("svg-fe-gaussian-blur", "blur", blur => blur.Property("in", "SourceGraphic").Property("stdDeviation", "1.5"))
                    .Child("svg-fe-offset", "offset", offset => offset.Property("in", "blur").Property("dx", "0").Property("dy", "1"))
                    .Child("svg-fe-merge", "merge", merge => merge
                        .Child("svg-fe-merge-node", "merge-shadow", node => node.Property("in", "offset"))
                        .Child("svg-fe-merge-node", "merge-source", node => node.Property("in", "SourceGraphic")))))
            .Child("svg-use", "reused-marker", use => use.Property("href", "#marker-symbol").Property("x", "10").Property("y", "10"))
            .Child("svg-rect", "patterned-panel", panel => panel.Property("width", "50").Property("height", "20")
                .Property("fill", "url(#stripe-pattern)").Property("filter", "url(#soft-glow)"))
            .Child("svg-line", "default-line", line => line.Property("x1", "0").Property("y1", "0")
                .Property("x2", "20").Property("y2", "20"))
            .Build();

        var doc = XDocument.Parse(_renderer.Render(scene));
        XNamespace svg = "http://www.w3.org/2000/svg";

        Assert.NotNull(doc.Descendants(svg + "symbol").Single());
        Assert.NotNull(doc.Descendants(svg + "pattern").Single());
        Assert.NotNull(doc.Descendants(svg + "marker").Single());
        Assert.NotNull(doc.Descendants(svg + "filter").Single());
        Assert.NotNull(doc.Descendants(svg + "feGaussianBlur").Single());
        Assert.Equal("#7ee0bd", (string?)doc.Descendants(svg + "line").Single().Attribute("stroke"));
        Assert.Equal("url(#stripe-pattern)", (string?)doc.Descendants(svg + "rect").Single().Attribute("fill"));
        Assert.Equal("#marker-symbol", (string?)doc.Descendants(svg + "use").Single().Attribute("href"));
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
                    .Property("dur", "4s").Property("repeatCount", "indefinite")
                    .Child("svg-mpath", "motion-reference", reference => reference.Property("href", "#motion-route")))
                .Child("svg-set", "reveal", set => set.Property("attributeName", "opacity")
                    .Property("to", "1").Property("begin", "1s").Property("dur", "1s")))
            .Build();

        var svg = _renderer.Render(scene);

        Assert.Contains("<animate", svg);
        Assert.Contains("<animateTransform", svg);
        Assert.Contains("<animateMotion", svg);
        Assert.Contains("<mpath href=\"#motion-route\"", svg);
        Assert.Contains("<set", svg);
        Assert.Contains("to=\"1\"", svg);
        Assert.Contains("values=\"2;4;2\"", svg);
        Assert.Contains("repeatCount=\"indefinite\"", svg);
        Assert.Contains("viewBox=\"0 0 100 56.25\"", svg);
    }

    [Fact]
    public void Emits_extended_filter_primitives_and_lighting_sources()
    {
        var scene = GuiBuilder.Create("svg-scene", "scene")
            .Child("svg-filter", "advanced-filter", filter => filter
                .Child("svg-fe-turbulence", "noise", noise => noise
                    .Property("baseFrequency", "0.03 0.08").Property("numOctaves", "2").Property("type", "fractalNoise"))
                .Child("svg-fe-displacement-map", "displace", displacement => displacement
                    .Property("in", "SourceGraphic").Property("in2", "noise").Property("scale", "3")
                    .Property("xChannelSelector", "R").Property("yChannelSelector", "G"))
                .Child("svg-fe-morphology", "erode", morphology => morphology
                    .Property("in", "SourceGraphic").Property("operator", "erode").Property("radius", "1"))
                .Child("svg-fe-convolve-matrix", "sharpen", convolution => convolution
                    .Property("order", "3").Property("kernelMatrix", "0 -1 0 -1 5 -1 0 -1 0"))
                .Child("svg-fe-component-transfer", "tone", transfer => transfer
                    .Child("svg-fe-func-r", "red-curve", curve => curve.Property("type", "gamma")
                        .Property("amplitude", "1.1").Property("exponent", "0.9").Property("offset", "0")))
                .Child("svg-fe-diffuse-lighting", "diffuse", lighting => lighting
                    .Property("surfaceScale", "2").Property("diffuseConstant", "1")
                    .Child("svg-fe-distant-light", "sun", light => light.Property("azimuth", "225").Property("elevation", "45")))
                .Child("svg-fe-specular-lighting", "specular", lighting => lighting
                    .Property("specularConstant", "0.7").Property("specularExponent", "12")
                    .Child("svg-fe-point-light", "point-source", light => light.Property("x", "10").Property("y", "10").Property("z", "20")))
                .Child("svg-fe-spot-light", "spot-source", light => light
                    .Property("x", "0").Property("y", "0").Property("z", "10")
                    .Property("pointsAtX", "20").Property("pointsAtY", "20").Property("pointsAtZ", "0"))
                .Child("svg-fe-image", "image-source", image => image.Property("href", "#texture"))).Build();

        var doc = XDocument.Parse(_renderer.Render(scene));
        XNamespace svg = "http://www.w3.org/2000/svg";

        Assert.NotNull(doc.Descendants(svg + "feTurbulence").Single());
        Assert.Equal("3", (string?)doc.Descendants(svg + "feDisplacementMap").Single().Attribute("scale"));
        Assert.NotNull(doc.Descendants(svg + "feMorphology").Single());
        Assert.Equal("0 -1 0 -1 5 -1 0 -1 0", (string?)doc.Descendants(svg + "feConvolveMatrix").Single().Attribute("kernelMatrix"));
        Assert.Equal("gamma", (string?)doc.Descendants(svg + "feFuncR").Single().Attribute("type"));
        Assert.NotNull(doc.Descendants(svg + "feDistantLight").Single());
        Assert.NotNull(doc.Descendants(svg + "fePointLight").Single());
        Assert.NotNull(doc.Descendants(svg + "feSpotLight").Single());
        Assert.Equal("#texture", (string?)doc.Descendants(svg + "feImage").Single().Attribute("href"));
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
    public void Rejects_duplicate_node_ids_before_emitting_ambiguous_svg_references()
    {
        var scene = GuiBuilder.Create("svg-scene", "scene")
            .Child("svg-circle", "marker", circle => circle.Property("r", "2"))
            .Child("svg-rect", "marker", rect => rect.Property("width", "10").Property("height", "10"))
            .Build();

        var error = Assert.Throws<ArgumentException>(() => _renderer.Render(scene));
        Assert.Contains("marker", error.Message);
        Assert.Contains("unique", error.Message);
    }

    [Fact]
    public void Rejects_collisions_with_synthesized_coordinate_overlay_ids()
    {
        var scene = GuiBuilder.Create("coordinate-system", "plot")
            .Property("grid", "visible")
            .Child("svg-circle", "plot-grid", circle => circle.Property("r", "1"))
            .Build();

        var error = Assert.Throws<ArgumentException>(() => _renderer.Render(scene));
        Assert.Contains("plot-grid", error.Message);
    }

    [Fact]
    public void Rejects_coordinate_ranges_whose_finite_endpoints_overflow_the_span()
    {
        var scene = GuiBuilder.Create("coordinate-system", "scene")
            .Property("minimum", "-1e308")
            .Property("maximum", "1e308")
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
