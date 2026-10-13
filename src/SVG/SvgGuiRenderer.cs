using System.Globalization;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Gui.Svg;

/// <summary>Manifests a platform-neutral GUI tree as a self-contained SVG document.</summary>
/// <remarks>
/// The adapter emits SVG markup only. It does not execute FSM_API, run an animation
/// clock, or claim to be a GPU renderer. Temporal SVG effects must be described
/// explicitly in the input tree or added by a higher-level presentation component.
/// </remarks>
public sealed class SvgGuiRenderer
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

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
        var svg = new XElement(Svg + "svg",
            new XAttribute("width", width),
            new XAttribute("height", height),
            new XAttribute("viewBox", $"0 0 100 {F(viewHeight)}"),
            new XAttribute("preserveAspectRatio", "xMidYMid meet"),
            new XAttribute("role", "img"),
            new XAttribute("aria-label", root.Text ?? root.Id),
            new XElement(Svg + "title", root.Text ?? root.Id));

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
                    new XAttribute("cx", F(x)),
                    new XAttribute("cy", F(y)),
                    new XAttribute("r", F(Number(node, "radius", 1.8))),
                    new XAttribute("fill", Value(node, "fill", "#7ee0bd")));
                AddStroke(element, node);
                break;
            }
            case "svg-circle":
                element = new XElement(Svg + "circle",
                    new XAttribute("cx", F(Number(node, "cx", 50))),
                    new XAttribute("cy", F(Number(node, "cy", 50))),
                    new XAttribute("r", F(Number(node, "r", 2))),
                    new XAttribute("fill", Value(node, "fill", "#7ee0bd")));
                AddStroke(element, node);
                break;
            case "svg-line":
                element = new XElement(Svg + "line",
                    new XAttribute("x1", F(Number(node, "x1", 0))),
                    new XAttribute("y1", F(Number(node, "y1", 0))),
                    new XAttribute("x2", F(Number(node, "x2", 100))),
                    new XAttribute("y2", F(Number(node, "y2", 100))),
                    new XAttribute("stroke", Value(node, "stroke", "#7ee0bd")),
                    new XAttribute("stroke-width", F(Number(node, "stroke-width", 0.6))));
                break;
            case "svg-rect":
                element = new XElement(Svg + "rect",
                    new XAttribute("x", F(Number(node, "x", 0))),
                    new XAttribute("y", F(Number(node, "y", 0))),
                    new XAttribute("width", F(Number(node, "width", 10))),
                    new XAttribute("height", F(Number(node, "height", 10))),
                    new XAttribute("fill", Value(node, "fill", "none")));
                AddStroke(element, node);
                break;
            case "svg-text":
            case "text":
                element = new XElement(Svg + "text",
                    new XAttribute("x", F(Number(node, "x", 0))),
                    new XAttribute("y", F(Number(node, "y", 10))),
                    new XAttribute("fill", Value(node, "fill", "#eef6ff")),
                    new XAttribute("font-size", F(Number(node, "font-size", 3.5))),
                    node.Text ?? string.Empty);
                break;
            case "svg-path":
                element = new XElement(Svg + "path",
                    new XAttribute("d", Value(node, "d", string.Empty)),
                    new XAttribute("fill", Value(node, "fill", "none")));
                AddStroke(element, node);
                break;
            case "svg-animate":
                element = new XElement(Svg + "animate");
                CopyProperties(node, element, "attributeName", "from", "to", "dur", "repeatCount", "values", "keyTimes", "calcMode", "begin");
                break;
            case "svg-animate-motion":
                element = new XElement(Svg + "animateMotion");
                CopyProperties(node, element, "path", "dur", "repeatCount", "rotate", "begin");
                break;
            default:
                element = new XElement(Svg + "g");
                if (node.Text is not null)
                    element.Add(new XElement(Svg + "title", node.Text));
                break;
        }

        element.SetAttributeValue("id", node.Id);
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
                new XAttribute("cx", F(x)),
                new XAttribute("cy", F(y)),
                new XAttribute("r", "1.1"),
                new XAttribute("fill", "#ffcf86")));
        }

        if (bounds)
        {
            svg.Add(new XElement(Svg + "rect",
                new XAttribute("id", node.Id + "-bounds"),
                new XAttribute("x", "0"), new XAttribute("y", "0"),
                new XAttribute("width", "100"), new XAttribute("height", F(range.ViewHeight)),
                new XAttribute("fill", "none"),
                new XAttribute("stroke", "#7390a8"),
                new XAttribute("stroke-width", "0.35")));
        }
    }

    private static void AddStroke(XElement element, GuiNode node)
    {
        var stroke = Value(node, "stroke", string.Empty);
        if (stroke.Length > 0) element.SetAttributeValue("stroke", stroke);
        if (node.Properties.TryGetValue("stroke-width", out var width) &&
            double.TryParse(width, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) &&
            double.IsFinite(parsed) && parsed >= 0)
            element.SetAttributeValue("stroke-width", F(parsed));
        foreach (var name in new[] { "stroke-dasharray", "stroke-linecap", "stroke-linejoin", "opacity", "fill-opacity" })
            if (node.Properties.TryGetValue(name, out var value))
                element.SetAttributeValue(name, value);
    }

    private static CoordinateRange ReadCoordinateRange(GuiNode node, double viewHeight)
    {
        var minimum = Number(node, "minimum", 0);
        var maximum = Number(node, "maximum", 100);
        if (maximum <= minimum)
            throw new ArgumentException("Coordinate-system maximum must be greater than minimum.", nameof(node));
        return new CoordinateRange(minimum, maximum, viewHeight);
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

    private static string Value(GuiNode node, string name, string fallback) =>
        node.Properties.TryGetValue(name, out var value) ? value : fallback;

    private static string F(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);

    private readonly record struct CoordinateRange(double Minimum, double Maximum, double ViewHeight);
}
