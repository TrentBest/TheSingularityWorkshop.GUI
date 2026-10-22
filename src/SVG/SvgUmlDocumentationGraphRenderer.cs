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

        // Spatial roles are derived from directed source relationships, not alphabetical order.
        // Incoming edges identify consumers above the focus; outgoing edges identify dependencies below.
        var externalNodes = graphNodes.Values.Where(node => node.IsExternal)
            .OrderBy(node => node.Name, StringComparer.Ordinal).ThenBy(node => node.Id, StringComparer.Ordinal).ToArray();
        var consumerIds = edges.Where(edge => edge.TargetTypeId == focusTypeId && edge.SourceTypeId != focusTypeId)
            .Select(edge => edge.SourceTypeId).ToHashSet(StringComparer.Ordinal);
        var dependencyIds = edges.Where(edge => edge.SourceTypeId == focusTypeId && edge.TargetTypeId is not null && edge.TargetTypeId != focusTypeId)
            .Select(edge => edge.TargetTypeId!).ToHashSet(StringComparer.Ordinal);
        var consumers = graphNodes.Values.Where(node => !node.IsFocus && !node.IsExternal && consumerIds.Contains(node.Id))
            .OrderBy(node => node.Name, StringComparer.Ordinal).ThenBy(node => node.Id, StringComparer.Ordinal).ToArray();
        // If a type both consumes and is consumed by the focus, keep one card above it and retain
        // the outgoing relationship; the relationship label still states the direction explicitly.
        var dependencies = graphNodes.Values.Where(node => !node.IsFocus && !node.IsExternal &&
                dependencyIds.Contains(node.Id) && !consumerIds.Contains(node.Id))
            .OrderBy(node => node.Name, StringComparer.Ordinal).ThenBy(node => node.Id, StringComparer.Ordinal).ToArray();
        var assignedIds = consumers.Concat(dependencies).Select(node => node.Id).ToHashSet(StringComparer.Ordinal);
        var otherNeighbors = graphNodes.Values.Where(node => !node.IsFocus && !node.IsExternal && !assignedIds.Contains(node.Id))
            .OrderBy(node => node.Name, StringComparer.Ordinal).ThenBy(node => node.Id, StringComparer.Ordinal).ToArray();
        dependencies = dependencies.Concat(otherNeighbors).ToArray();

        var columns = Math.Max(1, Math.Min(3, Math.Max(consumers.Length, dependencies.Length)));
        var centralWidth = Math.Max(NodeWidth, columns * NodeWidth + (columns - 1) * HorizontalGap);
        var canvasWidth = Math.Max(width, Margin * 2 + centralWidth +
            (externalNodes.Length == 0 ? 0 : HorizontalGap + NodeWidth));
        var hasSelfEdge = edges.Any(edge => edge.SourceTypeId == focusTypeId && edge.TargetTypeId == focusTypeId);
        if (hasSelfEdge)
            canvasWidth += 70;
        var focusX = Margin + (centralWidth - NodeWidth) / 2;
        var consumerRows = consumers.Length == 0 ? 0 : (consumers.Length + columns - 1) / columns;
        var focusY = Margin + (consumerRows == 0 ? 0 : consumerRows * (NodeHeight + VerticalGap));
        var positions = new Dictionary<string, NodePosition>(StringComparer.Ordinal)
        {
            [focus.Id] = new NodePosition(focusX, focusY)
        };

        void PlaceRows(IReadOnlyList<GraphNode> nodes, double startY)
        {
            for (var index = 0; index < nodes.Count; index++)
            {
                var row = index / columns;
                var column = index % columns;
                var rowCount = Math.Min(columns, nodes.Count - row * columns);
                var rowWidth = rowCount * NodeWidth + (rowCount - 1) * HorizontalGap;
                var rowStart = Margin + (centralWidth - rowWidth) / 2;
                positions[nodes[index].Id] = new NodePosition(
                    rowStart + column * (NodeWidth + HorizontalGap),
                    startY + row * (NodeHeight + VerticalGap));
            }
        }

        PlaceRows(consumers, Margin);
        PlaceRows(dependencies, focusY + NodeHeight + VerticalGap);
        var externalX = Margin + centralWidth + HorizontalGap;
        for (var index = 0; index < externalNodes.Length; index++)
            positions[externalNodes[index].Id] = new NodePosition(externalX, focusY + index * (NodeHeight + VerticalGap));

        var dependencyRows = dependencies.Length == 0 ? 0 : (dependencies.Length + columns - 1) / columns;
        var bottom = focusY + NodeHeight;
        if (dependencyRows > 0)
            bottom = Math.Max(bottom, focusY + NodeHeight + VerticalGap +
                (dependencyRows - 1) * (NodeHeight + VerticalGap) + NodeHeight);
        if (externalNodes.Length > 0)
            bottom = Math.Max(bottom, focusY + (externalNodes.Length - 1) * (NodeHeight + VerticalGap) + NodeHeight);
        var canvasHeight = bottom + Margin + (hasSelfEdge ? 55 : 0);

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

        var edgeGroups = edges.GroupBy(edge => new
            {
                Source = edge.SourceTypeId,
                Target = edge.TargetTypeId ?? (edge.SourceTypeId == focusTypeId
                    ? "external:" + (edge.TargetAssemblyIdentity ?? "unresolved") + ":" + edge.TargetDisplayName
                    : string.Empty)
            })
            .OrderBy(group => group.Key.Source, StringComparer.Ordinal)
            .ThenBy(group => group.Key.Target, StringComparer.Ordinal);

        var renderedPorts = new HashSet<string>(StringComparer.Ordinal);
        foreach (var group in edgeGroups)
        {
            var edge = group.First();
            var sourceId = group.Key.Source;
            var targetId = group.Key.Target;
            if (string.IsNullOrEmpty(targetId) || !positions.TryGetValue(sourceId, out var sourcePosition) ||
                !positions.TryGetValue(targetId, out var targetPosition))
                continue;

            var isSelf = sourceId == targetId;
            var typeIdentity = targetId;
            var stroke = TypeColor(typeIdentity);
            var marker = edge.Kind is UmlRelationshipKind.Inheritance or UmlRelationshipKind.InterfaceRealization
                ? "url(#uml-triangle)"
                : "url(#uml-arrow)";
            XElement connector;
            double labelX;
            double labelY;
            double sourcePortX;
            double sourcePortY;
            double targetPortX;
            double targetPortY;
            if (isSelf)
            {
                var startX = sourcePosition.X + NodeWidth - 42;
                var endX = sourcePosition.X + NodeWidth - 42;
                var startY = sourcePosition.Y + 36;
                var endY = sourcePosition.Y + 92;
                var loopX = sourcePosition.X + NodeWidth + 52;
                connector = new XElement(Svg + "path",
                    new XAttribute("d", $"M {F(startX)} {F(startY)} C {F(loopX)} {F(startY)}, {F(loopX)} {F(endY)}, {F(endX)} {F(endY)}"),
                    new XAttribute("fill", "none"), new XAttribute("stroke", stroke),
                    new XAttribute("stroke-width", "2"), new XAttribute("marker-end", marker));
                labelX = loopX;
                labelY = sourcePosition.Y + 68;
                sourcePortX = sourcePosition.X + NodeWidth;
                sourcePortY = sourcePosition.Y + 62;
                targetPortX = sourcePortX;
                targetPortY = sourcePosition.Y + 92;
            }
            else
            {
                var sourceCenterX = sourcePosition.X + NodeWidth / 2;
                var targetCenterX = targetPosition.X + NodeWidth / 2;
                var sourceCenterY = sourcePosition.Y + NodeHeight / 2;
                var targetCenterY = targetPosition.Y + NodeHeight / 2;
                var dx = targetCenterX - sourceCenterX;
                var dy = targetCenterY - sourceCenterY;
                if (Math.Abs(dx) > Math.Abs(dy))
                {
                    var sourceOnLeft = dx > 0;
                    sourcePortX = sourceOnLeft ? sourcePosition.X + NodeWidth : sourcePosition.X;
                    targetPortX = sourceOnLeft ? targetPosition.X : targetPosition.X + NodeWidth;
                    sourcePortY = sourceCenterY;
                    targetPortY = targetCenterY;
                    connector = new XElement(Svg + "line",
                        new XAttribute("x1", F(sourcePortX)), new XAttribute("y1", F(sourcePortY)),
                        new XAttribute("x2", F(targetPortX)), new XAttribute("y2", F(targetPortY)),
                        new XAttribute("stroke", stroke), new XAttribute("stroke-width", "2"),
                        new XAttribute("marker-end", marker));
                }
                else
                {
                    var targetBelow = dy > 0;
                    sourcePortX = sourceCenterX;
                    targetPortX = targetCenterX;
                    sourcePortY = targetBelow ? sourcePosition.Y + NodeHeight : sourcePosition.Y;
                    targetPortY = targetBelow ? targetPosition.Y : targetPosition.Y + NodeHeight;
                    connector = new XElement(Svg + "line",
                        new XAttribute("x1", F(sourcePortX)), new XAttribute("y1", F(sourcePortY)),
                        new XAttribute("x2", F(targetPortX)), new XAttribute("y2", F(targetPortY)),
                        new XAttribute("stroke", stroke), new XAttribute("stroke-width", "2"),
                        new XAttribute("marker-end", marker));
                }
                if (edge.Kind is UmlRelationshipKind.Dependency or UmlRelationshipKind.InterfaceRealization)
                    connector.SetAttributeValue("stroke-dasharray", "6 4");
                labelX = (sourcePortX + targetPortX) / 2;
                labelY = (sourcePortY + targetPortY) / 2 - 6;
            }

            svg.Add(connector);
            var labels = group.Select(item => RelationshipLabel(item.Kind) + " · " + item.Label)
                .Distinct(StringComparer.Ordinal).ToArray();
            var relationshipText = string.Join(" ; ", labels);
            svg.Add(new XElement(Svg + "text",
                new XAttribute("x", F(labelX)), new XAttribute("y", F(labelY)),
                new XAttribute("text-anchor", "middle"), new XAttribute("font-size", "11"),
                new XAttribute("fill", "#243b4a"), relationshipText));

            var sourcePortKey = $"source:{sourceId}:{typeIdentity}:{F(sourcePortX)}:{F(sourcePortY)}";
            if (renderedPorts.Add(sourcePortKey))
                AddTypePort(svg, sourceId, typeIdentity, sourcePortX, sourcePortY, isReceiving: false);
            var targetPortKey = $"target:{targetId}:{typeIdentity}:{F(targetPortX)}:{F(targetPortY)}";
            if (renderedPorts.Add(targetPortKey))
                AddTypePort(svg, targetId, typeIdentity, targetPortX, targetPortY, isReceiving: true);
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
                    new XAttribute("font-size", "16"), new XAttribute("font-weight", "700"),
                    new XAttribute("fill", headerText), ShortDisplayName(node)),
                new XElement(Svg + "text",
                    new XAttribute("x", F(position.X + 14)), new XAttribute("y", F(position.Y + 65)),
                    new XAttribute("font-size", "10"), new XAttribute("fill", node.IsFocus ? "#b7c8d5" : "#506779"),
                    node.Namespace);

            var members = node.Members.Take(4).ToArray();
            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                var signature = member.Signature.Length > 39 ? member.Signature[..36] + "…" : member.Signature;
                group.Add(new XElement(Svg + "text",
                    new XAttribute("x", F(position.X + 14)),
                    new XAttribute("y", F(position.Y + 91 + index * 20)),
                    new XAttribute("font-size", "11"),
                    new XAttribute("fill", node.IsFocus ? "#f0f6fb" : "#243b4a"),
                    member.Kind + ": " + signature));
            }
            if (node.Members.Count > members.Length)
                group.Add(new XElement(Svg + "text",
                    new XAttribute("x", F(position.X + 14)), new XAttribute("y", F(position.Y + 176)),
                    new XAttribute("font-size", "11"), new XAttribute("fill", headerText),
                    $"+{node.Members.Count - members.Length} more members"));
            svg.Add(group);
        }

        return new XDocument(new XDeclaration("1.0", "utf-8", null), svg).ToString(SaveOptions.DisableFormatting);
    }

    private static readonly string[] TypePalette =
    [
        "#52e05a", "#00a8ff", "#ff2cff", "#ff3030", "#ff7a00", "#ffd34d"
    ];

    private static string TypeColor(string identity)
    {
        // Stable FNV-1a hashing makes a semantic identity retain its color across diagrams.
        // Labels and data-type IDs remain authoritative because palette collisions are possible.
        uint hash = 2166136261;
        foreach (var character in identity)
            hash = (hash ^ character) * 16777619;
        return TypePalette[hash % (uint)TypePalette.Length];
    }

    private static void AddTypePort(XElement svg, string ownerId, string typeIdentity, double x, double y, bool isReceiving)
    {
        var group = new XElement(Svg + "g",
            new XAttribute("data-owner-type-id", ownerId),
            new XAttribute("data-type-id", typeIdentity),
            new XAttribute("transform", isReceiving ? $"rotate(180 {F(x)} {F(y)})" : "rotate(0)"),
            new XElement(Svg + "title", $"Type conduit for {typeIdentity}; " +
                (isReceiving ? "receiving endpoint, rotated 180 degrees." : "source endpoint.")),
            new XElement(Svg + "line",
                new XAttribute("x1", F(x - 8)), new XAttribute("y1", F(y)),
                new XAttribute("x2", F(x + 8)), new XAttribute("y2", F(y)),
                new XAttribute("stroke", TypeColor(typeIdentity)),
                new XAttribute("stroke-width", "5"),
                new XAttribute("stroke-linecap", "round")));
        svg.Add(group);
    }

    private static string RelationshipLabel(UmlRelationshipKind kind) => kind switch
    {
        UmlRelationshipKind.Inheritance => "inheritance",
        UmlRelationshipKind.InterfaceRealization => "interface realization",
        UmlRelationshipKind.Association => "association",
        UmlRelationshipKind.Dependency => "dependency",
        _ => kind.ToString().ToLowerInvariant()
    };

    private static GraphNode FromType(UmlDocumentedType type, bool isFocus) =>
        new(type.Id, DisplayName(type), type.Identity.Kind, type.Identity.Namespace, type.Members, isFocus, false);

    private static string ShortDisplayName(GraphNode node)
    {
        var name = node.Name;
        var lastDot = name.LastIndexOf('.');
        return lastDot < 0 ? name : name[(lastDot + 1)..];
    }

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
