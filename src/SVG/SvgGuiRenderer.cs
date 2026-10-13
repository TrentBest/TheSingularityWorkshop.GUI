using System.Globalization;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Gui.Svg;

/// <summary>Manifests a platform-neutral GUI tree as a self-contained SVG document.</summary>
/// <remarks>
/// The adapter emits SVG markup only. It does not execute FSM_API, run an animation
/// clock, or claim to be a GPU renderer. Animation elements are explicit scene data.
/// Node kinds use the <c>svg-</c> prefix to keep SVG manifestation details out of Core.
/// </remarks>
public sealed class SvgGuiRenderer
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

    private static readonly string[] PresentationAttributes =
    [
        "class", "transform", "opacity", "fill-opacity", "stroke-opacity",
        "stroke", "stroke-width", "stroke-dasharray", "stroke-dashoffset",
        "stroke-linecap", "stroke-linejoin", "fill-rule", "clip-rule",
        "vector-effect", "display", "visibility", "pointer-events",
        "font-family", "font-size", "font-weight", "font-style",
        "text-anchor", "dominant-baseline", "text-decoration",
        "clip-path", "mask", "filter", "style", "role", "tabindex",
        "aria-label", "aria-hidden", "focusable"
    ];

    /// <summary>Converts a GUI model snapshot into an SVG document.</summary>
    /// <param name="root">The immutable GUI model snapshot to manifest.</param>
    /// <param name="width">The intrinsic SVG width in user-agent pixels.</param>
    /// <param name="height">The intrinsic SVG height in user-agent pixels.</param>
    public string Render(GuiNode root, int width = 960, int height = 540)
    {
        ArgumentNullException.ThrowIfNull(root);
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");

        var viewHeight = 100d * height / width;
        var label = Value(root, "aria-label", root.Text ?? root.Id);
        var svg = new XElement(Svg + "svg",
            new XAttribute("width", width),
            new XAttribute("height", height),
            new XAttribute("viewBox", Value(root, "viewBox", $"0 0 100 {F(viewHeight)}")),
            new XAttribute("preserveAspectRatio", Value(root, "preserveAspectRatio", "xMidYMid meet")),
            new XAttribute("role", Value(root, "role", "img")),
            new XAttribute("aria-label", label));

        AddPresentationAttributes(svg, root);
        svg.Add(new XElement(Svg + "title", label));
        if (root.Properties.TryGetValue("description", out var description) && !string.IsNullOrWhiteSpace(description))
            svg.Add(new XElement(Svg + "desc", description));

        if (root.Kind is "svg-scene" or "svg" or "coordinate-system")
        {
            CoordinateRange? coordinates = null;
            if (root.Kind == "coordinate-system")
            {
                coordinates = ReadCoordinateRange(root, viewHeight);
                AddCoordinateSystem(svg, root, coordinates.Value);
            }
            foreach (var child in root.Children)
                svg.Add(RenderNode(child, coordinates));
        }
        else
        {
            svg.Add(RenderNode(root, null));
        }

        return new XDocument(new XDeclaration("1.0", "utf-8", null), svg)
            .ToString(SaveOptions.DisableFormatting);
    }

    private static XElement RenderNode(GuiNode node, CoordinateRange? coordinates)
    {
        XElement element;
        switch (node.Kind)
        {
            case "coordinate-point":
            {
                var x = Number(node, "x", 0);
                var y = Number(node, "y", 0);
                if (coordinates is { } range)
                {
                    x = Map(x, range.Minimum, range.Maximum);
                    y = range.ViewHeight - Map(y, range.Minimum, range.Maximum) * range.ViewHeight / 100;
                }
                element = new XElement(Svg + "circle",
                    new XAttribute("cx", F(x)), new XAttribute("cy", F(y)),
                    new XAttribute("r", F(NonNegative(node, "radius", 1.8))),
                    new XAttribute("fill", Value(node, "fill", "#7ee0bd")));
                break;
            }
            case "svg-circle":
                element = new XElement(Svg + "circle",
                    new XAttribute("cx", F(Number(node, "cx", 50))),
                    new XAttribute("cy", F(Number(node, "cy", 50))),
                    new XAttribute("r", F(NonNegative(node, "r", 2))),
                    new XAttribute("fill", Value(node, "fill", "#7ee0bd")));
                break;
            case "svg-ellipse":
                element = new XElement(Svg + "ellipse",
                    new XAttribute("cx", F(Number(node, "cx", 50))),
                    new XAttribute("cy", F(Number(node, "cy", 50))),
                    new XAttribute("rx", F(NonNegative(node, "rx", 5))),
                    new XAttribute("ry", F(NonNegative(node, "ry", 3))),
                    new XAttribute("fill", Value(node, "fill", "#7ee0bd")));
                break;
            case "svg-line":
                element = new XElement(Svg + "line",
                    new XAttribute("x1", F(Number(node, "x1", 0))),
                    new XAttribute("y1", F(Number(node, "y1", 0))),
                    new XAttribute("x2", F(Number(node, "x2", 100))),
                    new XAttribute("y2", F(Number(node, "y2", 100))));
                break;
            case "svg-rect":
                element = new XElement(Svg + "rect",
                    new XAttribute("x", F(Number(node, "x", 0))),
                    new XAttribute("y", F(Number(node, "y", 0))),
                    new XAttribute("width", F(NonNegative(node, "width", 10))),
                    new XAttribute("height", F(NonNegative(node, "height", 10))));
                if (node.Properties.ContainsKey("rx")) element.SetAttributeValue("rx", F(NonNegative(node, "rx", 0)));
                if (node.Properties.ContainsKey("ry")) element.SetAttributeValue("ry", F(NonNegative(node, "ry", 0)));
                break;
            case "svg-polyline":
            case "svg-polygon":
                element = new XElement(Svg + (node.Kind == "svg-polygon" ? "polygon" : "polyline"),
                    new XAttribute("points", Value(node, "points", string.Empty)));
                break;
            case "svg-text":
            case "text":
                element = new XElement(Svg + "text",
                    new XAttribute("x", F(Number(node, "x", 0))),
                    new XAttribute("y", F(Number(node, "y", 10))),
                    new XAttribute("fill", Value(node, "fill", "#eef6ff")),
                    node.Text ?? string.Empty);
                break;
            case "svg-tspan":
                element = new XElement(Svg + "tspan", node.Text ?? string.Empty);
                if (node.Properties.ContainsKey("x")) element.SetAttributeValue("x", F(Number(node, "x", 0)));
                if (node.Properties.ContainsKey("y")) element.SetAttributeValue("y", F(Number(node, "y", 0)));
                break;
            case "svg-path":
                element = new XElement(Svg + "path",
                    new XAttribute("d", Value(node, "d", string.Empty)),
                    new XAttribute("fill", Value(node, "fill", "none")));
                break;
            case "svg-image":
                element = new XElement(Svg + "image",
                    new XAttribute("x", F(Number(node, "x", 0))),
                    new XAttribute("y", F(Number(node, "y", 0))),
                    new XAttribute("width", F(NonNegative(node, "width", 10))),
                    new XAttribute("height", F(NonNegative(node, "height", 10))),
                    new XAttribute("href", Value(node, "href", node.Source ?? string.Empty)),
                    new XAttribute("preserveAspectRatio", Value(node, "preserveAspectRatio", "xMidYMid meet")));
                if (node.Text is not null) element.SetAttributeValue("aria-label", node.Text);
                break;
            case "svg-title":
                element = new XElement(Svg + "title", node.Text ?? string.Empty);
                break;
            case "svg-desc":
                element = new XElement(Svg + "desc", node.Text ?? string.Empty);
                break;
            case "svg-defs":
                element = new XElement(Svg + "defs");
                break;
            case "svg-linear-gradient":
                element = new XElement(Svg + "linearGradient");
                CopyProperties(node, element, "x1", "y1", "x2", "y2", "gradientUnits", "spreadMethod", "gradientTransform");
                break;
            case "svg-radial-gradient":
                element = new XElement(Svg + "radialGradient");
                CopyProperties(node, element, "cx", "cy", "r", "fx", "fy", "fr", "gradientUnits", "spreadMethod", "gradientTransform");
                break;
            case "svg-stop":
                element = new XElement(Svg + "stop");
                CopyProperties(node, element, "offset", "stop-color", "stop-opacity");
                break;
            case "svg-clip-path":
                element = new XElement(Svg + "clipPath");
                CopyProperties(node, element, "clipPathUnits", "transform");
                break;
            case "svg-mask":
                element = new XElement(Svg + "mask");
                CopyProperties(node, element, "x", "y", "width", "height", "maskUnits", "maskContentUnits");
                break;
            case "svg-animate":
                element = new XElement(Svg + "animate");
                CopyProperties(node, element, "attributeName", "attributeType", "from", "to", "by", "dur", "repeatCount", "repeatDur", "values", "keyTimes", "keySplines", "calcMode", "begin", "end", "fill", "additive", "accumulate");
                break;
            case "svg-animate-transform":
                element = new XElement(Svg + "animateTransform");
                CopyProperties(node, element, "attributeName", "type", "from", "to", "by", "dur", "repeatCount", "repeatDur", "values", "keyTimes", "keySplines", "calcMode", "begin", "end", "additive", "accumulate");
                break;
            case "svg-animate-motion":
                element = new XElement(Svg + "animateMotion");
                CopyProperties(node, element, "path", "dur", "repeatCount", "repeatDur", "rotate", "begin", "end", "keyPoints", "keyTimes", "calcMode");
                break;
            default:
                element = new XElement(Svg + "g");
                if (node.Text is not null) element.Add(new XElement(Svg + "title", node.Text));
                break;
        }

        element.SetAttributeValue("id", node.Id);
        AddPresentationAttributes(element, node);
        foreach (var child in node.Children)
            element.Add(RenderNode(child, coordinates));
        return element;
    }

    private static void AddCoordinateSystem(XElement svg, GuiNode node, CoordinateRange range)
    {
        var grid = Value(node, "grid", "hidden") == "visible";
        var axes = Value(node, "axes", "hidden") == "visible";
        var origin = Value(node, "origin", "hidden") == "visible";
        var bounds = Value(node, "bounds", "hidden") == "visible";

        if (grid)
        {
            var gridGroup = new XElement(Svg + "g",
                new XAttribute("id", node.Id + "-grid"),
                new XAttribute("stroke", "#294055"),
                new XAttribute("stroke-width", "0.2"));
            for (var p = 10; p < 100; p += 10)
            {
                var y = p * range.ViewHeight / 100;
                gridGroup.Add(new XElement(Svg + "path", new XAttribute("d", $"M{p} 0V{F(range.ViewHeight)}")));
                gridGroup.Add(new XElement(Svg + "path", new XAttribute("d", $"M0 {F(y)}H100")));
            }
            svg.Add(gridGroup);
        }

        if (axes)
        {
            var originX = Map(Number(node, "origin-x", (range.Minimum + range.Maximum) / 2), range.Minimum, range.Maximum);
            var originY = range.ViewHeight - Map(Number(node, "origin-y", (range.Minimum + range.Maximum) / 2), range.Minimum, range.Maximum) * range.ViewHeight / 100;
            svg.Add(new XElement(Svg + "path",
                new XAttribute("id", node.Id + "-axes"),
                new XAttribute("d", $"M{F(originX)} 0V{F(range.ViewHeight)}M0 {F(originY)}H100"),
                new XAttribute("stroke", "#7390a8"),
                new XAttribute("stroke-width", "0.35")));
        }

        if (origin)
        {
            var x = Map(Number(node, "origin-x", (range.Minimum + range.Maximum) / 2), range.Minimum, range.Maximum);
            var y = range.ViewHeight - Map(Number(node, "origin-y", (range.Minimum + range.Maximum) / 2), range.Minimum, range.Maximum) * range.ViewHeight / 100;
            svg.Add(new XElement(Svg + "circle",
                new XAttribute("id", node.Id + "-origin"),
                new XAttribute("cx", F(x)), new XAttribute("cy", F(y)),
                new XAttribute("r", "1.1"), new XAttribute("fill", "#ffcf86")));
        }

        if (bounds)
        {
            svg.Add(new XElement(Svg + "rect",
                new XAttribute("id", node.Id + "-bounds"),
                new XAttribute("x", "0"), new XAttribute("y", "0"),
                new XAttribute("width", "100"), new XAttribute("height", F(range.ViewHeight)),
                new XAttribute("fill", "none"), new XAttribute("stroke", "#7390a8"),
                new XAttribute("stroke-width", "0.35")));
        }
    }

    private static void AddPresentationAttributes(XElement element, GuiNode node)
    {
        foreach (var name in PresentationAttributes)
            if (node.Properties.TryGetValue(name, out var value))
                element.SetAttributeValue(name, value);

        if (node.Properties.TryGetValue("stroke", out var stroke))
            element.SetAttributeValue("stroke", stroke);
        if (node.Properties.TryGetValue("fill", out var fill))
            element.SetAttributeValue("fill", fill);
    }

    private static void CopyProperties(GuiNode node, XElement element, params string[] allowedNames)
    {
        foreach (var name in allowedNames)
            if (node.Properties.TryGetValue(name, out var value))
                element.SetAttributeValue(name, value);
    }

    private static double Map(double value, double minimum, double maximum) =>
        (value - minimum) / (maximum - minimum) * 100;

    private static double Number(GuiNode node, string name, double fallback)
    {
        if (!node.Properties.TryGetValue(name, out var value)) return fallback;
        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) ||
            !double.IsFinite(parsed))
            throw new ArgumentException($"GUI node '{node.Id}' has an invalid numeric '{name}' property.");
        return parsed;
    }

    private static double NonNegative(GuiNode node, string name, double fallback)
    {
        var value = Number(node, name, fallback);
        if (value < 0)
            throw new ArgumentOutOfRangeException(name, value, $"SVG property '{name}' must be non-negative.");
        return value;
    }

    private static string Value(GuiNode node, string name, string fallback) =>
        node.Properties.TryGetValue(name, out var value) ? value : fallback;

    private static string F(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);

    private readonly record struct CoordinateRange(double Minimum, double Maximum, double ViewHeight);
}
