using System.Globalization;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Gui.Svg;

/// <summary>Renders a source-grounded UML type view as nested, static semantic data cards.</summary>
/// <remarks>
/// This is a static SVG projection, not an interactive viewer. The SVG therefore renders all
/// modeled member signatures and accessibility groups instead of implying click-to-expand behavior.
/// </remarks>
public sealed class SvgUmlClassDiagramRenderer
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private const double Left = 24;
    private const double Top = 24;
    private const double HeaderHeight = 54;
    private const double GroupHeaderHeight = 30;
    private const double MemberHeaderHeight = 28;
    private const double LineHeight = 19;
    private const double Gap = 8;
    private const double Padding = 12;

    // Shared Workshop palette tokens: stable semantic meaning, not depth-based decoration.
    private const string NamespaceStroke = "#6da99b";
    private const string TypeStroke = "#ff823d";
    private const string PropertyStroke = "#d6b82e";
    private const string MethodStroke = "#2e9d70";
    private const string FieldStroke = "#4a90e2";
    private const string PublicStroke = "#2e9d70";
    private const string ProtectedStroke = "#d6b82e";
    private const string PrivateStroke = "#8b7ac8";
    private const string Ink = "#172b3a";
    private const string Paper = "#f7fafc";

    /// <summary>Creates an accessible UML type diagram with nested member-category and visibility cards.</summary>
    public string Render(UmlClassDiagram model, int width = 960)
    {
        ArgumentNullException.ThrowIfNull(model);
        if (width < 480)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be at least 480 pixels.");

        var sections = new[]
        {
            (Title: "Properties", Kind: "property", Stroke: PropertyStroke, Members: model.Properties),
            (Title: "Methods", Kind: "method", Stroke: MethodStroke, Members: model.Operations),
            (Title: "Fields", Kind: "field", Stroke: FieldStroke, Members: model.Fields)
        };
        var height = Top * 2 + HeaderHeight + sections.Sum(section =>
            GroupHeight(section.Members, section.Stroke)) +
            (model.DeclaredRelationships.Count == 0 ? 0 : GroupHeaderHeight + model.DeclaredRelationships.Count * LineHeight + Padding);

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
            new XElement(Svg + "desc", new XAttribute("id", "uml-desc"),
                $"{model.Description} Static nested-card view. Member categories are ordered Properties, Methods, Fields; each is grouped by exact accessibility in Public, Protected, Private order. All represented signatures are visible in this SVG."));

        var x = Left;
        var y = Top;
        var panelWidth = width - Left * 2;
        AddCard(svg, x, y, panelWidth, HeaderHeight, TypeStroke, "#fff0e5", "type-card", "4");
        svg.Add(Text(x + Padding, y + 21, $"«{model.Kind}»", "#6b2e0c", "12", "normal"));
        var heading = string.IsNullOrWhiteSpace(model.Namespace) ? model.Name : model.Namespace + "." + model.Name;
        svg.Add(Text(x + Padding, y + 43, heading, "#6b2e0c", "18", "700"));
        y += HeaderHeight + Gap;

        foreach (var section in sections)
            y = DrawCategory(svg, x, y, panelWidth, section.Title, section.Kind, section.Stroke, section.Members);

        if (model.DeclaredRelationships.Count > 0)
        {
            AddCard(svg, x, y, panelWidth, GroupHeaderHeight, NamespaceStroke, "#e8f4f1", "unresolved-references", "2");
            svg.Add(Text(x + Padding, y + 20, "Declared type references — unresolved", Ink, "13", "700"));
            y += GroupHeaderHeight;
            foreach (var relationship in model.DeclaredRelationships)
            {
                svg.Add(Text(x + Padding * 2, y + 15, relationship, Ink, "12", "normal"));
                y += LineHeight;
            }
        }

        return new XDocument(new XDeclaration("1.0", "utf-8", null), svg).ToString(SaveOptions.DisableFormatting);
    }

    private static double GroupHeight(IReadOnlyList<UmlMember> members, string stroke)
    {
        var groups = AccessibilityGroups(members);
        var height = Padding;
        foreach (var group in groups)
            height += GroupHeaderHeight + group.Members.Sum(member => MemberHeight(member)) + Gap;
        return Math.Max(GroupHeaderHeight + LineHeight + Padding, height);
    }

    private static double DrawCategory(XElement svg, double x, double y, double width, string title,
        string kind, string stroke, IReadOnlyList<UmlMember> members)
    {
        var categoryHeight = GroupHeight(members, stroke);
        AddCard(svg, x, y, width, categoryHeight, stroke, Paper, "member-category " + kind, "3");
        svg.Add(Text(x + Padding, y + 20, $"{title} ({members.Count})", Ink, "14", "700"));
        var innerX = x + Padding;
        var innerWidth = width - Padding * 2;
        var innerY = y + GroupHeaderHeight + Gap;
        var groups = AccessibilityGroups(members);
        if (groups.Count == 0)
        {
            svg.Add(Text(innerX + Padding, innerY + 14, "No members", "#506779", "12", "normal"));
            return y + categoryHeight + Gap;
        }

        foreach (var group in groups)
        {
            var groupHeight = GroupHeaderHeight + group.Members.Sum(MemberHeight);
            AddCard(svg, innerX, innerY, innerWidth, groupHeight, group.Stroke, group.Fill,
                "accessibility-group " + group.Name.ToLowerInvariant(), "2");
            svg.Add(Text(innerX + Padding, innerY + 20, $"{group.Name} ({group.Members.Count})", Ink, "12", "700"));
            var memberY = innerY + GroupHeaderHeight;
            foreach (var member in group.Members)
            {
                var memberHeight = MemberHeight(member);
                AddCard(svg, innerX + Padding, memberY, innerWidth - Padding * 2, memberHeight,
                    stroke, "#ffffff", "member " + kind, "1");
                var marker = member.IsStatic ? "  {static}" : string.Empty;
                var signature = string.IsNullOrWhiteSpace(member.Signature) ? member.Name : member.Signature;
                var lines = signature.Split('\n');
                for (var line = 0; line < lines.Length; line++)
                    svg.Add(Text(innerX + Padding * 2, memberY + 16 + line * LineHeight,
                        (line == 0 ? VisibilityMarker(member.Visibility) + " " : "  ") + lines[line].TrimEnd('\r') +
                        (line == lines.Length - 1 ? marker : string.Empty), Ink, "12", "normal"));
                memberY += memberHeight;
            }
            innerY += groupHeight + Gap;
        }
        return y + categoryHeight + Gap;
    }

    private static List<(string Name, string Stroke, string Fill, List<UmlMember> Members)> AccessibilityGroups(
        IReadOnlyList<UmlMember> members)
    {
        var result = new List<(string, string, string, List<UmlMember>)>();
        Add("Public", PublicStroke, "#eaf7f0", "public");
        Add("Protected", ProtectedStroke, "#fff9df", "protected");
        Add("Private", PrivateStroke, "#f1edfb", "private");
        Add("Internal / other", NamespaceStroke, "#e8f4f1", "other");
        return result.Where(group => group.Item4.Count > 0).ToList();

        void Add(string name, string stroke, string fill, string visibility)
        {
            var groupMembers = members.Where(member => NormalizeVisibility(member.Visibility) == visibility).ToList();
            if (groupMembers.Count > 0)
                result.Add((name, stroke, fill, groupMembers));
        }
    }

    private static string NormalizeVisibility(string visibility) => visibility switch
    {
        "public" => "public",
        "protected" => "protected",
        "private" => "private",
        _ => "other"
    };

    private static string VisibilityMarker(string visibility) => visibility switch
    {
        "public" => "+",
        "protected" => "#",
        "private" => "-",
        _ => "~"
    };

    private static double MemberHeight(UmlMember member) =>
        MemberHeaderHeight + Math.Max(1, (string.IsNullOrWhiteSpace(member.Signature) ? member.Name : member.Signature)
            .Split('\n').Length) * LineHeight;

    private static void AddCard(XElement svg, double x, double y, double width, double height,
        string stroke, string fill, string kind, string strokeWidth)
    {
        svg.Add(new XElement(Svg + "rect",
            new XAttribute("x", F(x)), new XAttribute("y", F(y)),
            new XAttribute("width", F(width)), new XAttribute("height", F(height)),
            new XAttribute("rx", "5"), new XAttribute("data-card-kind", kind),
            new XAttribute("fill", fill), new XAttribute("stroke", stroke),
            new XAttribute("stroke-width", strokeWidth)));
    }

    private static XElement Text(double x, double y, string value, string fill, string size, string weight) =>
        new(Svg + "text", new XAttribute("x", F(x)), new XAttribute("y", F(y)),
            new XAttribute("fill", fill), new XAttribute("font-size", size),
            new XAttribute("font-weight", weight), value);

    private static string F(double value) => value.ToString("0.##", CultureInfo.InvariantCulture);
}
