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
    private const int MaximumTreeDepth = 256;
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

    private static readonly string[] PresentationAttributes =
    [
        "class", "transform", "opacity", "fill-opacity", "stroke-opacity",
        "stroke", "stroke-width", "stroke-dasharray", "stroke-dashoffset",
        "stroke-linecap", "stroke-linejoin", "marker-start", "marker-mid", "marker-end",
        "fill-rule", "clip-rule", "color", "vector-effect", "display", "visibility", "pointer-events",
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
        ValidateUniqueIds(root);

        var viewHeight = 100d * height / width;
        var label = Value(root, "aria-label", root.Text ?? root.Id);
        var hasDescription = root.Properties.TryGetValue("description", out var description) &&
            !string.IsNullOrWhiteSpace(description);
        var labelledBy = hasDescription
            ? "gui-svg-title gui-svg-description"
            : "gui-svg-title";
        var svg = new XElement(Svg + "svg",
            new XAttribute("width", width),
            new XAttribute("height", height),
            new XAttribute("viewBox", Value(root, "viewBox", $"0 0 100 {F(viewHeight)}")),
            new XAttribute("preserveAspectRatio", Value(root, "preserveAspectRatio", "xMidYMid meet")),
            new XAttribute("role", Value(root, "role", "img")),
            new XAttribute("aria-label", label));

        AddPresentationAttributes(svg, root);
        svg.SetAttributeValue("aria-labelledby", labelledBy);
        svg.Add(new XElement(Svg + "title", new XAttribute("id", "gui-svg-title"), label));
        if (hasDescription)
            svg.Add(new XElement(Svg + "desc", new XAttribute("id", "gui-svg-description"), description));

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
                    new XAttribute("y2", F(Number(node, "y2", 100))),
                    new XAttribute("stroke", Value(node, "stroke", "#7ee0bd")),
                    new XAttribute("stroke-width", F(NonNegative(node, "stroke-width", 0.6))));
                break;
            case "svg-rect":
                element = new XElement(Svg + "rect",
                    new XAttribute("x", F(Number(node, "x", 0))),
                    new XAttribute("y", F(Number(node, "y", 0))),
                    new XAttribute("width", F(NonNegative(node, "width", 10))),
                    new XAttribute("height", F(NonNegative(node, "height", 10))),
                    new XAttribute("fill", Value(node, "fill", "none")));
                if (node.Properties.ContainsKey("rx")) element.SetAttributeValue("rx", F(NonNegative(node, "rx", 0)));
                if (node.Properties.ContainsKey("ry")) element.SetAttributeValue("ry", F(NonNegative(node, "ry", 0)));
                break;
            case "svg-polyline":
                element = new XElement(Svg + "polyline",
                    new XAttribute("points", Value(node, "points", string.Empty)),
                    new XAttribute("fill", Value(node, "fill", "none")),
                    new XAttribute("stroke", Value(node, "stroke", "#7ee0bd")),
                    new XAttribute("stroke-width", F(NonNegative(node, "stroke-width", 0.6))));
                break;
            case "svg-polygon":
                element = new XElement(Svg + "polygon",
                    new XAttribute("points", Value(node, "points", string.Empty)),
                    new XAttribute("fill", Value(node, "fill", "#7ee0bd")));
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
            case "svg-a":
            {
                var href = Value(node, "href", string.Empty);
                if (string.IsNullOrWhiteSpace(href))
                    throw new ArgumentException($"SVG link '{node.Id}' requires a non-empty href.", nameof(node));
                element = new XElement(Svg + "a", new XAttribute("href", href));
                CopyProperties(node, element, "target", "rel", "download", "hreflang", "type");
                break;
            }
            case "svg-desc":
                element = new XElement(Svg + "desc", node.Text ?? string.Empty);
                break;
            case "svg-metadata":
                element = new XElement(Svg + "metadata", node.Text ?? string.Empty);
                break;
            case "svg-view":
                element = new XElement(Svg + "view");
                CopyProperties(node, element, "viewBox", "preserveAspectRatio", "zoomAndPan");
                break;
            case "svg-switch":
            case "svg-g":
                element = new XElement(Svg + (node.Kind == "svg-switch" ? "switch" : "g"));
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
            case "svg-defs":
                element = new XElement(Svg + "defs");
                break;
            case "svg-linear-gradient":
                element = new XElement(Svg + "linearGradient");
                CopyProperties(node, element, "x1", "y1", "x2", "y2", "gradientUnits", "spreadMethod", "gradientTransform", "href");
                break;
            case "svg-radial-gradient":
                element = new XElement(Svg + "radialGradient");
                CopyProperties(node, element, "cx", "cy", "r", "fx", "fy", "fr", "gradientUnits", "spreadMethod", "gradientTransform", "href");
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
                CopyProperties(node, element, "x", "y", "width", "height", "maskUnits", "maskContentUnits", "mask-type");
                break;
            case "svg-use":
                element = new XElement(Svg + "use",
                    new XAttribute("href", Value(node, "href", string.Empty)));
                CopyProperties(node, element, "x", "y", "width", "height");
                break;
            case "svg-symbol":
                element = new XElement(Svg + "symbol");
                CopyProperties(node, element, "viewBox", "preserveAspectRatio", "refX", "refY");
                break;
            case "svg-pattern":
                element = new XElement(Svg + "pattern");
                CopyProperties(node, element, "x", "y", "width", "height", "patternUnits", "patternContentUnits", "patternTransform", "viewBox", "preserveAspectRatio", "href");
                break;
            case "svg-marker":
                element = new XElement(Svg + "marker");
                CopyProperties(node, element, "refX", "refY", "markerWidth", "markerHeight", "markerUnits", "orient", "viewBox", "preserveAspectRatio");
                break;
            case "svg-filter":
                element = new XElement(Svg + "filter");
                CopyProperties(node, element, "x", "y", "width", "height", "filterUnits", "primitiveUnits", "filterRes", "color-interpolation-filters");
                break;
            case "svg-fe-gaussian-blur":
                element = new XElement(Svg + "feGaussianBlur");
                CopyProperties(node, element, "in", "stdDeviation", "edgeMode");
                break;
            case "svg-fe-offset":
                element = new XElement(Svg + "feOffset");
                CopyProperties(node, element, "in", "dx", "dy");
                break;
            case "svg-fe-color-matrix":
                element = new XElement(Svg + "feColorMatrix");
                CopyProperties(node, element, "in", "type", "values");
                break;
            case "svg-fe-blend":
                element = new XElement(Svg + "feBlend");
                CopyProperties(node, element, "in", "in2", "mode");
                break;
            case "svg-fe-flood":
                element = new XElement(Svg + "feFlood");
                CopyProperties(node, element, "flood-color", "flood-opacity");
                break;
            case "svg-fe-composite":
                element = new XElement(Svg + "feComposite");
                CopyProperties(node, element, "in", "in2", "operator", "k1", "k2", "k3", "k4");
                break;
            case "svg-fe-turbulence":
                element = new XElement(Svg + "feTurbulence");
                CopyProperties(node, element, "baseFrequency", "numOctaves", "seed", "stitchTiles", "type");
                break;
            case "svg-fe-displacement-map":
                element = new XElement(Svg + "feDisplacementMap");
                CopyProperties(node, element, "in", "in2", "scale", "xChannelSelector", "yChannelSelector");
                break;
            case "svg-fe-morphology":
                element = new XElement(Svg + "feMorphology");
                CopyProperties(node, element, "in", "operator", "radius");
                break;
            case "svg-fe-tile":
                element = new XElement(Svg + "feTile");
                CopyProperties(node, element, "in");
                break;
            case "svg-fe-convolve-matrix":
                element = new XElement(Svg + "feConvolveMatrix");
                CopyProperties(node, element, "in", "order", "kernelMatrix", "divisor", "bias", "targetX", "targetY", "edgeMode", "kernelUnitLength", "preserveAlpha");
                break;
            case "svg-fe-component-transfer":
                element = new XElement(Svg + "feComponentTransfer");
                CopyProperties(node, element, "in");
                break;
            case "svg-fe-func-r":
            case "svg-fe-func-g":
            case "svg-fe-func-b":
            case "svg-fe-func-a":
            {
                var functionName = node.Kind switch
                {
                    "svg-fe-func-r" => "feFuncR",
                    "svg-fe-func-g" => "feFuncG",
                    "svg-fe-func-b" => "feFuncB",
                    _ => "feFuncA"
                };
                element = new XElement(Svg + functionName);
                CopyProperties(node, element, "type", "tableValues", "slope", "intercept", "amplitude", "exponent", "offset");
                break;
            }
            case "svg-fe-diffuse-lighting":
                element = new XElement(Svg + "feDiffuseLighting");
                CopyProperties(node, element, "in", "surfaceScale", "diffuseConstant", "kernelUnitLength", "lighting-color");
                break;
            case "svg-fe-specular-lighting":
                element = new XElement(Svg + "feSpecularLighting");
                CopyProperties(node, element, "in", "surfaceScale", "specularConstant", "specularExponent", "kernelUnitLength", "lighting-color");
                break;
            case "svg-fe-distant-light":
                element = new XElement(Svg + "feDistantLight");
                CopyProperties(node, element, "azimuth", "elevation");
                break;
            case "svg-fe-point-light":
                element = new XElement(Svg + "fePointLight");
                CopyProperties(node, element, "x", "y", "z");
                break;
            case "svg-fe-spot-light":
                element = new XElement(Svg + "feSpotLight");
                CopyProperties(node, element, "x", "y", "z", "pointsAtX", "pointsAtY", "pointsAtZ", "specularExponent", "limitingConeAngle");
                break;
            case "svg-fe-image":
                element = new XElement(Svg + "feImage");
                CopyProperties(node, element, "href", "preserveAspectRatio");
                break;
            case "svg-fe-merge":
                element = new XElement(Svg + "feMerge");
                break;
            case "svg-fe-merge-node":
                element = new XElement(Svg + "feMergeNode");
                CopyProperties(node, element, "in");
                break;
            case "svg-set":
                element = new XElement(Svg + "set");
                CopyProperties(node, element, "attributeName", "attributeType", "to", "begin", "dur", "end", "repeatCount", "repeatDur", "fill", "restart", "min", "max");
                break;
            case "svg-mpath":
            {
                var href = Value(node, "href", string.Empty);
                if (string.IsNullOrWhiteSpace(href))
                    throw new ArgumentException($"SVG motion path '{node.Id}' requires a non-empty href.", nameof(node));
                element = new XElement(Svg + "mpath", new XAttribute("href", href));
                break;
            }
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

    private static void ValidateUniqueIds(GuiNode root)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var pending = new Stack<(GuiNode Node, int Depth)>();
        pending.Push((root, 0));

        // Validate depth before rendering. This traversal is iterative so a hostile or
        // accidentally pathological tree cannot overflow the call stack before the
        // renderer has a chance to reject it.
        while (pending.Count > 0)
        {
            var (node, depth) = pending.Pop();
            if (depth > MaximumTreeDepth)
                throw new InvalidOperationException(
                    $"GUI tree exceeds the SVG renderer's maximum depth ({MaximumTreeDepth}).");

            if (!ids.Add(node.Id))
                throw new ArgumentException(
                    $"SVG node id '{node.Id}' is duplicated. SVG ids must be unique within a document.",
                    nameof(root));

            foreach (var child in node.Children)
                pending.Push((child, depth + 1));
        }

        // Coordinate-system overlays create real SVG elements with synthesized ids.
        if (root.Kind == "coordinate-system")
        {
            foreach (var (property, suffix) in new[]
            {
                ("grid", "grid"), ("axes", "axes"), ("origin", "origin"), ("bounds", "bounds")
            })
            {
                if (Value(root, property, "hidden") == "visible" &&
                    !ids.Add(root.Id + "-" + suffix))
                    throw new ArgumentException(
                        $"SVG node id '{root.Id}-{suffix}' collides with a synthesized coordinate overlay id.",
                        nameof(root));
            }
        }
    }

    private static CoordinateRange ReadCoordinateRange(GuiNode node, double viewHeight)
    {
        var minimum = Number(node, "minimum", 0);
        var maximum = Number(node, "maximum", 100);
        var span = maximum - minimum;
        if (!double.IsFinite(span) || span <= 0)
            throw new ArgumentException("Coordinate-system maximum must be greater than minimum with a finite range.", nameof(node));
        return new CoordinateRange(minimum, maximum, viewHeight);
    }

    private static void CopyProperties(GuiNode node, XElement element, params string[] allowedNames)
    {
        foreach (var name in allowedNames)
            if (node.Properties.TryGetValue(name, out var value))
                element.SetAttributeValue(name, value);
    }

    private static double Map(double value, double minimum, double maximum)
    {
        var mapped = (value - minimum) / (maximum - minimum) * 100;
        if (!double.IsFinite(mapped))
            throw new ArgumentOutOfRangeException(nameof(value), value, "Coordinate mapping produced a non-finite SVG position.");
        return mapped;
    }

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
