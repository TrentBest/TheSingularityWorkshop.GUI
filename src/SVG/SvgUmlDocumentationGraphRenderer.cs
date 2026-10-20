using System.Globalization;
using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Gui.Svg;

/// <summary>
/// Renders a focused neighborhood from the canonical documentation model.
/// Type identities and relationship endpoints come from the model, not display-name matching.
/// </summary>
public sealed class SvgUmlDocumentationGraphRenderer
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private const double Margin = 28;
    private const double NodeWidth = 310;
    private const double NodeHeight = 190;
    private const double HorizontalGap = 84;
    private const double VerticalGap = 140;

    /// <summary>Renders one type and its directly connected semantic neighbors as an accessible SVG graph.</summary>
    public string Render(UmlDocumentationModel model, string focusTypeId, int width = 1200)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentException.ThrowIfNullOrWhiteSpace(focusTypeId);
        if (width < 640)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be at least 640 pixels.");

        var types = model.Types.ToDictionary(type => type.Id, StringComparer.Ordinal);
        if (!types.TryGetValue(focusTypeId, out var focus))
            throw new ArgumentException("The focused type ID is not present in the UML model.", nameof(focusTypeId));

        var edges = model.Relationships
            .Where(edge => edge.SourceTypeId == focusTypeId || edge.TargetTypeId == focusTypeId)
            .OrderBy(edge => edge.Kind)
            .ThenBy(edge => edge.SourceTypeId, StringComparer.Ordinal)
            .ThenBy(edge => edge.TargetDisplayName, StringComparer.Ordinal)
            .ThenBy(edge => edge.Label, StringComparer.Ordinal)
            .ToArray();

        var graphNodes = new Dictionary<string, GraphNode>(StringComparer.Ordinal)
        {
            [focus.Id] = FromType(focus, isFocus: true)
        };
        foreach (var edge in edges)
        {
            if (types.TryGetValue(edge.SourceTypeId, out var source))
                graphNodes.TryAdd(source.Id, FromType(source, isFocus: source.Id == focusTypeId));
            if (edge.TargetTypeId is not null && types.TryGetValue(edge.TargetTypeId, out var target))
                graphNodes.TryAdd(target.Id, FromType(target, isFocus: target.Id == focusTypeId));
            else if (edge.SourceTypeId == focusTypeId &&
                     edge.Resolution is UmlRelationshipResolution.External or UmlRelationshipResolution.Unresolved)
            {
                var externalId = "external:" + (edge.TargetAssemblyIdentity ?? "unresolved") + ":" + edge.TargetDisplayName;
                graphNodes.TryAdd(externalId, new GraphNode(
                    externalId, edge.TargetDisplayName, edge.Resolution == UmlRelationshipResolution.External ? "external type" : "unresolved type",
                    string.Empty, Array.Empty<UmlDocumentedMember>(), false, true));
            }
        }

        var neighbors = graphNodes.Values.Where(node => !node.IsFocus)
            .OrderBy(node => node.Name, StringComparer.Ordinal)
            .ThenBy(node => node.Id, StringComparer.Ordinal)
            .ToArray();
        var columns = Math.Max(1, Math.Min(3, neighbors.Length));
        var canvasWidth = Math.Max(width, Margin * 2 + columns * NodeWidth + (columns - 1) * HorizontalGap);
        var hasSelfEdge = edges.Any(edge => edge.SourceTypeId == focusTypeId && edge.TargetTypeId == focusTypeId);
        if (hasSelfEdge)
            canvasWidth += 100;
        var focusX = (canvasWidth - NodeWidth) / 2;
        var focusY = Margin;
        var positions = new Dictionary<string, NodePosition>(StringComparer.Ordinal)
        {
            [focus.Id] = new NodePosition(focusX, focusY)
        };

        for (var index = 0; index < neighbors.Length; index++)
        {
            var row = index / columns;
            var column = index % columns;
            var rowCount = Math.Min(columns, neighbors.Length - row * columns);
            var rowWidth = rowCount * NodeWidth + (rowCount - 1) * HorizontalGap;
            var rowStart = (canvasWidth - rowWidth) / 2;
            positions[neighbors[index].Id] = new NodePosition(
                rowStart + column * (NodeWidth + HorizontalGap),
                focusY + NodeHeight + VerticalGap + row * (NodeHeight + VerticalGap));
        }

        var rows = neighbors.Length == 0 ? 0 : (neighbors.Length + columns - 1) / columns;
        var canvasHeight = Margin * 2 + NodeHeight + (rows == 0 ? 0 : VerticalGap + rows * NodeHeight + (rows - 1) * VerticalGap);
        if (hasSelfEdge)
            canvasHeight = Math.Max(canvasHeight, focusY + NodeHeight + 90 + Margin);

        var title = "Source-derived UML neighborhood: " + DisplayName(focus);
        var description = $"Compiler-model neighborhood for {DisplayName(focus)}. " +
            $"{graphNodes.Count} type nodes and {edges.Length} typed relationships are shown. " +
            "Solid and dashed connectors distinguish relationship kinds; external and unresolved targets are explicitly labelled.";

        var svg = new XElement(Svg + "svg",
            new XAttribute("xmlns", Svg.NamespaceName),
            new XAttribute("width", F(canvasWidth)),
            new XAttribute("height", F(canvasHeight)),
            new XAttribute("viewBox", $"0 0 {F(canvasWidth)} {F(canvasHeight)}"),
            new XAttribute("role", "img"),
            new XAttribute("aria-labelledby", "uml-graph-title uml-graph-desc"),
            new XAttribute("font-family", "Segoe UI, Arial, sans-serif"),
            new XElement(Svg + "title", new XAttribute("id", "uml-graph-title"), title),
            new XElement(Svg + "desc", new XAttribute("id", "uml-graph-desc"), description),
            new XElement(Svg + "defs",
                new XElement(Svg + "marker",
                    new XAttribute("id", "uml-arrow"), new XAttribute("markerWidth", "10"),
                    new XAttribute("markerHeight", "10"), new XAttribute("refX", "9"),
                    new XAttribute("refY", "5"), new XAttribute("orient", "auto"),
                    new XAttribute("markerUnits", "strokeWidth"),
                    new XElement(Svg + "path", new XAttribute("d", "M0,0 L10,5 L0,10 z"), new XAttribute("fill", "#397d75"))),
                new XElement(Svg + "marker",
                    new XAttribute("id", "uml-triangle"), new XAttribute("markerWidth", "12"),
                    new XAttribute("markerHeight", "12"), new XAttribute("refX", "11"),
                    new XAttribute("refY", "6"), new XAttribute("orient", "auto"),
                    new XAttribute("markerUnits", "strokeWidth"),
                    new XElement(Svg + "path", new XAttribute("d", "M0,0 L12,6 L0,12 z"),
                        new XAttribute("fill", "#ffffff"), new XAttribute("stroke", "#397d75"), new XAttribute("stroke-width", "1.2")))));

        foreach (var edge in edges)
        {
            var sourceId = edge.SourceTypeId;
            var targetId = edge.TargetTypeId;
            if (sourceId == focusTypeId && targetId is null)
                targetId = "external:" + (edge.TargetAssemblyIdentity ?? "unresolved") + ":" + edge.TargetDisplayName;
            if (targetId is null || !positions.TryGetValue(sourceId, out var sourcePosition) ||
                !positions.TryGetValue(targetId, out var targetPosition))
                continue;

            var isSelf = sourceId == targetId;
            var stroke = "#397d75";
            var dashed = edge.Kind is UmlRelationshipKind.Dependency or UmlRelationshipKind.InterfaceRealization;
            var marker = edge.Kind is UmlRelationshipKind.Inheritance or UmlRelationshipKind.InterfaceRealization
                ? "url(#uml-triangle)"
                : "url(#uml-arrow)";
            XElement connector;
            double labelX;
            double labelY;
            if (isSelf)
            {
                var startX = sourcePosition.X + NodeWidth - 42;
                var endX = sourcePosition.X + NodeWidth - 42;
                var startY = sourcePosition.Y + 36;
                var endY = sourcePosition.Y + 92;
                var loopX = sourcePosition.X + NodeWidth + 62;
                connector = new XElement(Svg + "path",
                    new XAttribute("d", $"M {F(startX)} {F(startY)} C {F(loopX)} {F(startY)}, {F(loopX)} {F(endY)}, {F(endX)} {F(endY)}"),
                    new XAttribute("fill", "none"), new XAttribute("stroke", stroke),
                    new XAttribute("stroke-width", "1.8"), new XAttribute("marker-end", marker));
                labelX = loopX;
                labelY = sourcePosition.Y + 68;
            }
            else
            {
                var sourceIsFocus = sourceId == focusTypeId;
                var fromX = sourcePosition.X + NodeWidth / 2;
                var toX = targetPosition.X + NodeWidth / 2;
                var fromY = sourceIsFocus ? sourcePosition.Y + NodeHeight : sourcePosition.Y;
                var toY = sourceIsFocus ? targetPosition.Y : targetPosition.Y + NodeHeight;
                connector = new XElement(Svg + "line",
                    new XAttribute("x1", F(fromX)), new XAttribute("y1", F(fromY)),
                    new XAttribute("x2", F(toX)), new XAttribute("y2", F(toY)),
                    new XAttribute("stroke", stroke), new XAttribute("stroke-width", "1.8"),
                    new XAttribute("marker-end", marker));
                if (dashed)
                    connector.SetAttributeValue("stroke-dasharray", "6 4");
                labelX = (fromX + toX) / 2;
                labelY = (fromY + toY) / 2 - 6;
            }

            svg.Add(connector);
            svg.Add(new XElement(Svg + "text",
                new XAttribute("x", F(labelX)), new XAttribute("y", F(labelY)),
                new XAttribute("text-anchor", "middle"), new XAttribute("font-size", "11"),
                new XAttribute("fill", "#243b4a"),
                edge.Kind.ToString().ToLowerInvariant() + " · " + edge.Label));
        }

        var nodeIndex = 0;
        foreach (var node in graphNodes.Values.OrderBy(node => node.IsFocus ? 0 : 1)
                     .ThenBy(node => node.Name, StringComparer.Ordinal).ThenBy(node => node.Id, StringComparer.Ordinal))
        {
            var position = positions[node.Id];
            var fill = node.IsFocus ? "#172b3a" : node.IsExternal ? "#f4f6f8" : "#f7fafc";
            var headerText = node.IsFocus ? "#f0f6fb" : "#172b3a";
            var border = node.IsExternal ? "#8b98a5" : "#397d75";
            var group = new XElement(Svg + "g",
                new XAttribute("id", "uml-node-" + nodeIndex++),
                new XAttribute("data-type-id", node.Id),
                new XElement(Svg + "title", DisplayName(node) + ": " + node.Kind),
                new XElement(Svg + "rect",
                    new XAttribute("x", F(position.X)), new XAttribute("y", F(position.Y)),
                    new XAttribute("width", F(NodeWidth)), new XAttribute("height", F(NodeHeight)),
                    new XAttribute("rx", "6"), new XAttribute("fill", fill),
                    new XAttribute("stroke", border), new XAttribute("stroke-width", node.IsFocus ? "2.5" : "1.5")),
                new XElement(Svg + "text",
                    new XAttribute("x", F(position.X + 14)), new XAttribute("y", F(position.Y + 24)),
                    new XAttribute("font-size", "12"), new XAttribute("fill", node.IsFocus ? "#b7c8d5" : "#506779"),
                    "«" + node.Kind + "»"),
                new XElement(Svg + "text",
                    new XAttribute("x", F(position.X + 14)), new XAttribute("y", F(position.Y + 48)),
                    new XAttribute("font-size", "15"), new XAttribute("font-weight", "700"),
                    new XAttribute("fill", headerText), DisplayName(node)));

            var members = node.Members.Take(4).ToArray();
            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                var signature = member.Signature.Length > 39 ? member.Signature[..36] + "…" : member.Signature;
                group.Add(new XElement(Svg + "text",
                    new XAttribute("x", F(position.X + 14)),
                    new XAttribute("y", F(position.Y + 76 + index * 23)),
                    new XAttribute("font-size", "11"),
                    new XAttribute("fill", node.IsFocus ? "#f0f6fb" : "#243b4a"),
                    member.Kind + ": " + signature));
            }
            if (node.Members.Count > members.Length)
                group.Add(new XElement(Svg + "text",
                    new XAttribute("x", F(position.X + 14)), new XAttribute("y", F(position.Y + 172)),
                    new XAttribute("font-size", "11"), new XAttribute("fill", headerText),
                    $"+{node.Members.Count - members.Length} more members"));
            svg.Add(group);
        }

        return new XDocument(new XDeclaration("1.0", "utf-8", null), svg).ToString(SaveOptions.DisableFormatting);
    }

    private static GraphNode FromType(UmlDocumentedType type, bool isFocus) =>
        new(type.Id, DisplayName(type), type.Identity.Kind, type.Identity.Namespace, type.Members, isFocus, false);

    private static string DisplayName(UmlDocumentedType type)
    {
        var containing = type.Identity.ContainingTypes.Select(StripArity);
        var name = string.Join(".", containing.Append(StripArity(type.Identity.MetadataName)));
        if (type.Identity.GenericArity > 0)
            name += "<" + string.Join(", ", Enumerable.Range(1, type.Identity.GenericArity).Select(index => "T" + index)) + ">";
        return string.IsNullOrWhiteSpace(type.Identity.Namespace) ? name : type.Identity.Namespace + "." + name;
    }

    private static string DisplayName(GraphNode node) => node.Name;

    private static string StripArity(string name)
    {
        var index = name.IndexOf('`');
        return index < 0 ? name : name[..index];
    }

    private static string F(double value) => value.ToString("0.##", CultureInfo.InvariantCulture);

    private sealed record GraphNode(
        string Id,
        string Name,
        string Kind,
        string Namespace,
        IReadOnlyList<UmlDocumentedMember> Members,
        bool IsFocus,
        bool IsExternal);

    private sealed record NodePosition(double X, double Y);
}
