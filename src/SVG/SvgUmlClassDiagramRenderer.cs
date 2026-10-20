using System.Globalization;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Gui.Svg;

/// <summary>Renders a renderer-neutral UML class projection into a self-contained SVG class diagram.</summary>
public sealed class SvgUmlClassDiagramRenderer
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private const double Left = 24;
    private const double Top = 24;
    private const double LineHeight = 22;
    private const double Padding = 14;
    private const double HeaderHeight = 66;
    private const double SectionHeadingHeight = 26;

    /// <summary>Creates an accessible UML class diagram with separate attribute/property/operation compartments.</summary>
    public string Render(UmlClassDiagram model, int width = 960)
    {
        ArgumentNullException.ThrowIfNull(model);
        if (width < 320)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be at least 320 pixels.");

        var sections = new[]
        {
            (Title: "Fields", Members: model.Fields),
            (Title: "Properties", Members: model.Properties),
            (Title: "Operations", Members: model.Operations),
            (Title: "Declared type references (unresolved)", Members: (IReadOnlyList<UmlMember>)Array.Empty<UmlMember>())
        };
        var relationshipLines = model.DeclaredRelationships;
        var height = Top * 2 + HeaderHeight +
            sections.Take(3).Sum(s => SectionHeadingHeight + (s.Members.Count == 0
                ? LineHeight
                : s.Members.Sum(member => Math.Max(1, member.Signature.Split('\\n').Length) * LineHeight))) +
            (relationshipLines.Count == 0 ? 0 : SectionHeadingHeight + relationshipLines.Count * LineHeight);

        var svg = new XElement(Svg + "svg",
            new XAttribute("xmlns", Svg.NamespaceName),
            new XAttribute("width", width),
            new XAttribute("height", Math.Ceiling(height)),
            new XAttribute("viewBox", $"0 0 {width} {F(height)}"),
            new XAttribute("role", "img"),
            new XAttribute("aria-labelledby", "uml-title uml-desc"),
            new XAttribute("font-family", "Segoe UI, Arial, sans-serif"),
            new XAttribute("font-size", "14"),
            new XElement(Svg + "title", new XAttribute("id", "uml-title"), $"UML class diagram: {model.Name}"),
            new XElement(Svg + "desc", new XAttribute("id", "uml-desc"), model.Description));

        var x = Left;
        var y = Top;
        var panelWidth = width - Left * 2;
        svg.Add(new XElement(Svg + "rect",
            new XAttribute("x", F(x)), new XAttribute("y", F(y)),
            new XAttribute("width", F(panelWidth)), new XAttribute("height", F(height - Top)),
            new XAttribute("rx", "6"), new XAttribute("fill", "#f7fafc"),
            new XAttribute("stroke", "#397d75"), new XAttribute("stroke-width", "2")));

        svg.Add(new XElement(Svg + "rect",
            new XAttribute("x", F(x)), new XAttribute("y", F(y)),
            new XAttribute("width", F(panelWidth)), new XAttribute("height", F(HeaderHeight)),
            new XAttribute("rx", "6"), new XAttribute("fill", "#172b3a")));
        svg.Add(new XElement(Svg + "rect",
            new XAttribute("x", F(x)), new XAttribute("y", F(y + HeaderHeight - 8)),
            new XAttribute("width", F(panelWidth)), new XAttribute("height", "8"),
            new XAttribute("fill", "#172b3a")));

        var heading = string.IsNullOrWhiteSpace(model.Namespace) ? model.Name : model.Namespace + "." + model.Name;
        svg.Add(Text(x + panelWidth / 2, y + 25, $"«{model.Kind}»", "#b7c8d5", "12", "normal", "middle"));
        svg.Add(Text(x + panelWidth / 2, y + 49, heading, "#f0f6fb", "18", "700", "middle"));
        y += HeaderHeight;
        y = DrawSection(svg, x, y, panelWidth, "Fields", model.Fields);
        y = DrawSection(svg, x, y, panelWidth, "Properties", model.Properties);
        y = DrawSection(svg, x, y, panelWidth, "Operations", model.Operations);
        if (relationshipLines.Count > 0)
        {
            y = DrawSectionHeading(svg, x, y, panelWidth, "Declared type references (not semantically resolved)");
            foreach (var relationship in relationshipLines)
            {
                svg.Add(Text(x + Padding, y + 16, relationship, "#172b3a", "13", "normal"));
                y += LineHeight;
            }
        }

        return new XDocument(new XDeclaration("1.0", "utf-8", null), svg).ToString(SaveOptions.DisableFormatting);
    }

    private static double DrawSection(XElement svg, double x, double y, double width, string title, IReadOnlyList<UmlMember> members)
    {
        y = DrawSectionHeading(svg, x, y, width, title);
        if (members.Count == 0)
        {
            svg.Add(Text(x + Padding, y + 16, "—", "#506779", "13", "normal"));
            return y + LineHeight;
        }

        foreach (var member in members)
        {
            var visibility = member.Visibility switch
            {
                "public" => "+",
                "protected" => "#",
                "private" => "-",
                "internal" or "package" => "~",
                _ => "~"
            };
            var staticMarker = member.IsStatic ? " {static}" : string.Empty;
            var signature = string.IsNullOrWhiteSpace(member.Signature) ? member.Name : member.Signature;
            var lines = signature.Split('\\n');
            for (var index = 0; index < lines.Length; index++)
            {
                var prefix = index == 0 ? visibility + " " : "  ";
                var suffix = index == lines.Length - 1 ? staticMarker : string.Empty;
                svg.Add(Text(x + Padding, y + 16, prefix + lines[index].TrimEnd('\\r') + suffix, "#172b3a", "13", "normal"));
                y += LineHeight;
            }
        }
        return y;
    }

    private static double DrawSectionHeading(XElement svg, double x, double y, double width, string title)
    {
        svg.Add(new XElement(Svg + "line",
            new XAttribute("x1", F(x)), new XAttribute("y1", F(y)),
            new XAttribute("x2", F(x + width)), new XAttribute("y2", F(y)),
            new XAttribute("stroke", "#397d75"), new XAttribute("stroke-width", "1.2")));
        svg.Add(Text(x + Padding, y + 18, title, "#172b3a", "13", "700"));
        return y + SectionHeadingHeight;
    }

    private static XElement Text(double x, double y, string value, string fill, string size, string weight, string? anchor = null) =>
        new(Svg + "text",
            new XAttribute("x", F(x)), new XAttribute("y", F(y)),
            new XAttribute("fill", fill), new XAttribute("font-size", size),
            new XAttribute("font-weight", weight),
            anchor is null ? null : new XAttribute("text-anchor", anchor),
            value);

    private static string F(double value) => value.ToString("0.##", CultureInfo.InvariantCulture);
}
