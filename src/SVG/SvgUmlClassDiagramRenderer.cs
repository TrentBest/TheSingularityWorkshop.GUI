using System.Globalization;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Gui.Svg;

/// <summary>Renders a source-grounded UML type view as nested, static semantic data cards.</summary>
/// <remarks>
/// The SVG projection is static, so all modeled tiers and signatures are visible. The outer
/// hierarchy is accessibility-first; each tier then repeats the Properties, Methods, Fields order.
/// </remarks>
public sealed class SvgUmlClassDiagramRenderer
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private const double Left = 24;
    private const double Top = 24;
    private const double HeaderHeight = 54;
    private const double NamespaceHeaderHeight = 32;
    private const double GroupHeaderHeight = 30;
    private const double MemberHeaderHeight = 28;
    private const double LineHeight = 19;
    private const double Gap = 8;
    private const double Padding = 12;

    // Shared Workshop palette tokens: visibility identifies the outer tier; member kind identifies its subgroup.
    private const string NamespaceStroke = "#6da99b";
    private const string TypeStroke = "#ff823d";
    private const string PropertyStroke = "#d6b82e";
    private const string MethodStroke = "#2e9d70";
    private const string FieldStroke = "#4a90e2";
    private const string PublicStroke = "#2e9d70";
    private const string ProtectedStroke = "#d6b82e";
    private const string InternalStroke = "#6da99b";
    private const string PrivateStroke = "#8b7ac8";
    private const string Ink = "#172b3a";
    private const string Paper = "#f7fafc";

    private sealed record MemberCategory(string Title, string Kind, string Stroke, IReadOnlyList<UmlMember> Members);
    private sealed record VisibilityTier(string Name, string Stroke, string Fill, string Key, IReadOnlyList<UmlMember> Members);

    /// <summary>Creates an accessible UML type diagram with nested visibility and member-category cards.</summary>
    public string Render(UmlClassDiagram model, int width = 960)
    {
        ArgumentNullException.ThrowIfNull(model);
        if (width < 480)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be at least 480 pixels.");

        var categories = new[]
        {
            new MemberCategory("Properties", "property", PropertyStroke, model.Properties),
            new MemberCategory("Methods", "method", MethodStroke, model.Operations),
            new MemberCategory("Fields", "field", FieldStroke, model.Fields)
        };
        var tiers = AccessibilityTiers(model.Properties.Concat(model.Operations).Concat(model.Fields).ToArray());
        var tierHeight = tiers.Sum(tier => TierHeight(tier, categories)) + Gap * tiers.Count;
        var relationshipHeight = model.DeclaredRelationships.Count == 0
            ? 0
            : GroupHeaderHeight + model.DeclaredRelationships.Count * LineHeight + Gap;
        var typeCardHeight = Padding * 2 + HeaderHeight + Gap + tierHeight + relationshipHeight;
        var namespaceCardHeight = Padding * 2 + NamespaceHeaderHeight + Gap + typeCardHeight;
        var height = Top * 2 + namespaceCardHeight;

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
                $"{model.Description} Static nested-card view. Visibility tiers are ordered Public, Protected, Internal / assembly-visible, then Private / implementation-only. Within each tier, member categories are ordered Properties, Methods, Fields. All represented signatures are visible in this SVG."));

        var namespaceX = Left;
        var namespaceY = Top;
        var namespaceWidth = width - Left * 2;
        AddCard(svg, namespaceX, namespaceY, namespaceWidth, namespaceCardHeight,
            NamespaceStroke, "#e8f4f1", "namespace-container", "3");
        var namespaceName = string.IsNullOrWhiteSpace(model.Namespace) ? "(global namespace)" : model.Namespace;
        svg.Add(Text(namespaceX + Padding, namespaceY + 21, namespaceName, Ink, "13", "700"));

        var x = namespaceX + Padding;
        var y = namespaceY + Padding + NamespaceHeaderHeight + Gap;
        var panelWidth = namespaceWidth - Padding * 2;
        AddCard(svg, x, y, panelWidth, typeCardHeight, TypeStroke, "#fff0e5", "type-container", "4");

        var contentX = x + Padding;
        var contentWidth = panelWidth - Padding * 2;
        var headerY = y + Padding;
        AddCard(svg, contentX, headerY, contentWidth, HeaderHeight, TypeStroke, "#ff823d", "type-header", "2");
        svg.Add(Text(contentX + Padding, headerY + 21, $"«{model.Kind}»", "#6b2e0c", "12", "normal"));
        var heading = string.IsNullOrWhiteSpace(model.Namespace) ? model.Name : model.Namespace + "." + model.Name;
        svg.Add(Text(contentX + Padding, headerY + 43, heading, "#6b2e0c", "18", "700"));
        y = headerY + HeaderHeight + Gap;

        foreach (var tier in tiers)
            y = DrawTier(svg, contentX, y, contentWidth, tier, categories);

        if (model.DeclaredRelationships.Count > 0)
        {
            AddCard(svg, contentX, y, contentWidth, GroupHeaderHeight, NamespaceStroke, "#e8f4f1", "unresolved-references", "2");
            svg.Add(Text(contentX + Padding, y + 20, "Declared type references — unresolved", Ink, "13", "700"));
            y += GroupHeaderHeight;
            foreach (var relationship in model.DeclaredRelationships)
            {
                svg.Add(Text(contentX + Padding * 2, y + 15, relationship, Ink, "12", "normal"));
                y += LineHeight;
            }
        }

        return new XDocument(new XDeclaration("1.0", "utf-8", null), svg).ToString(SaveOptions.DisableFormatting);
    }

    private static List<VisibilityTier> AccessibilityTiers(IReadOnlyList<UmlMember> members)
    {
        var definitions = new[]
        {
            (Name: "Public", Stroke: PublicStroke, Fill: "#e5f7ed", Key: "public"),
            (Name: "Protected", Stroke: ProtectedStroke, Fill: "#fff7d5", Key: "protected"),
            (Name: "Internal / assembly-visible", Stroke: InternalStroke, Fill: "#e5f2f0", Key: "internal"),
            (Name: "Private / implementation-only", Stroke: PrivateStroke, Fill: "#eee8fb", Key: "private")
        };
        return definitions
            .Select(definition => new VisibilityTier(
                definition.Name, definition.Stroke, definition.Fill, definition.Key,
                members.Where(member => NormalizeVisibility(member.Visibility) == definition.Key).ToArray()))
            .Where(tier => tier.Members.Count > 0)
            .ToList();
    }

    private static string NormalizeVisibility(string visibility)
    {
        var normalized = (visibility ?? string.Empty).Trim().ToLowerInvariant();
        if (normalized == "public") return "public";
        if (normalized == "protected") return "protected";
        if (normalized is "internal" or "protected internal" or "internal protected") return "internal";
        if (normalized is "private" or "private protected" or "protected private") return "private";
        return "internal";
    }

    private static double TierHeight(VisibilityTier tier, IReadOnlyList<MemberCategory> categories)
    {
        var categoryHeight = categories.Sum(category =>
            CategoryHeight(category, tier.Members.Where(member => category.Members.Contains(member)).ToArray()));
        return GroupHeaderHeight + Gap + categoryHeight + categories.Count(category =>
            tier.Members.Any(member => category.Members.Contains(member))) * Gap;
    }

    private static double CategoryHeight(MemberCategory category, IReadOnlyList<UmlMember> members)
    {
        if (members.Count == 0) return 0;
        return GroupHeaderHeight + Padding + members.Sum(MemberHeight) + Gap * (members.Count - 1);
    }

    private static double DrawTier(XElement svg, double x, double y, double width, VisibilityTier tier,
        IReadOnlyList<MemberCategory> categories)
    {
        var tierCategories = categories
            .Select(category => new MemberCategory(category.Title, category.Kind, category.Stroke,
                tier.Members.Where(member => category.Members.Contains(member)).ToArray()))
            .Where(category => category.Members.Count > 0)
            .ToArray();
        var height = GroupHeaderHeight + Gap +
            tierCategories.Sum(category => CategoryHeight(category, category.Members)) +
            Gap * tierCategories.Length;
        AddCard(svg, x, y, width, height, tier.Stroke, tier.Fill,
            "accessibility-tier " + tier.Key, "3");
        svg.Add(Text(x + Padding, y + 20, $"{tier.Name} ({tier.Members.Count})", Ink, "14", "700"));

        var innerX = x + Padding;
        var innerWidth = width - Padding * 2;
        var innerY = y + GroupHeaderHeight + Gap;
        foreach (var category in tierCategories)
        {
            var categoryHeight = CategoryHeight(category, category.Members);
            AddCard(svg, innerX, innerY, innerWidth, categoryHeight, category.Stroke, Paper,
                "member-category " + category.Kind, "2");
            svg.Add(Text(innerX + Padding, innerY + 20, $"{category.Title} ({category.Members.Count})", Ink, "13", "700"));
            var memberY = innerY + GroupHeaderHeight + Padding;
            foreach (var member in category.Members)
            {
                var memberHeight = MemberHeight(member);
                AddCard(svg, innerX + Padding, memberY, innerWidth - Padding * 2, memberHeight,
                    category.Stroke, "#ffffff", "member " + category.Kind, "1");
                var marker = member.IsStatic ? "  {static}" : string.Empty;
                var signature = string.IsNullOrWhiteSpace(member.Signature) ? member.Name : member.Signature;
                var lines = signature.Split('\n');
                for (var line = 0; line < lines.Length; line++)
                    svg.Add(Text(innerX + Padding * 2, memberY + 16 + line * LineHeight,
                        (line == 0 ? VisibilityMarker(member.Visibility) + " " : "  ") + lines[line].TrimEnd('\r') +
                        (line == lines.Length - 1 ? marker : string.Empty), Ink, "12", "normal"));
                memberY += memberHeight + Gap;
            }
            innerY += categoryHeight + Gap;
        }
        return y + height + Gap;
    }

    private static string VisibilityMarker(string visibility) => NormalizeVisibility(visibility) switch
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
