using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace TheSingularityWorkshop.Workshop.Gui.Svg;

/// <summary>A renderer-neutral graph node. Domain meaning remains with the caller.</summary>
public sealed record SvgGraphNode(
    string Id,
    string Label,
    string? Subtitle = null,
    string? Category = null,
    string? HttpsLink = null);

/// <summary>A directed edge from SourceNodeId to TargetNodeId.</summary>
public sealed record SvgGraphEdge(
    string Id,
    string SourceNodeId,
    string TargetNodeId,
    string? Label = null,
    string? Category = null,
    string? HttpsLink = null);

/// <summary>
/// An explicit visual container. A node may belong to multiple clusters (overlap);
/// a cluster may contain child clusters (nesting). Membership is never inferred.
/// </summary>
public sealed record SvgGraphCluster(
    string Id,
    string Title,
    IReadOnlyList<string> MemberNodeIds,
    IReadOnlyList<string>? MemberClusterIds = null,
    string? Category = null);

/// <summary>A caller-defined legend entry; category keys are opaque to the renderer.</summary>
public sealed record SvgGraphLegendEntry(string Category, string Label, string? Shape = null);

/// <summary>Immutable input to the generic directed-graph renderer.</summary>
public sealed record SvgDirectedGraph(
    IReadOnlyList<SvgGraphNode> Nodes,
    IReadOnlyList<SvgGraphEdge> Edges,
    IReadOnlyList<SvgGraphCluster>? Clusters = null,
    IReadOnlyList<SvgGraphLegendEntry>? Legend = null,
    string? Title = null,
    string? Description = null,
    string? ProvenanceCaption = null,
    string? FocalNodeId = null);

/// <summary>Bounded deterministic layout and presentation settings.</summary>
public sealed record SvgDirectedGraphRenderOptions(
    int NodeWidth = 220,
    int NodeHeight = 76,
    int HorizontalGap = 90,
    int VerticalGap = 86,
    int Margin = 48,
    int MaximumNodes = 250,
    int MaximumEdges = 1000,
    int MaximumClusters = 100);

/// <summary>
/// Produces standalone, accessible SVG for caller-owned directed graph semantics.
/// Layout order is derived only from stable node IDs; it does not interpret edges.
/// </summary>
public sealed class SvgDirectedGraphRenderer
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private static readonly Regex StableId = new("^[A-Za-z_][A-Za-z0-9_.:-]{0,95}$", RegexOptions.CultureInvariant);
    private const string DefaultNodeFill = "#f7fafc";
    private const string DefaultNodeStroke = "#315b72";

    private sealed record Point(double X, double Y);
    private sealed record Bounds(double X, double Y, double Width, double Height)
    {
        public double Right => X + Width;
        public double Bottom => Y + Height;
        public Bounds Expand(double amount) => new(X - amount, Y - amount, Width + amount * 2, Height + amount * 2);
        public static Bounds Union(Bounds a, Bounds b)
        {
            var x = Math.Min(a.X, b.X); var y = Math.Min(a.Y, b.Y);
            return new Bounds(x, y, Math.Max(a.Right, b.Right) - x, Math.Max(a.Bottom, b.Bottom) - y);
        }
    }

    /// <summary>Render a validated graph as deterministic, standalone SVG markup.</summary>
    public string Render(SvgDirectedGraph graph, SvgDirectedGraphRenderOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(graph);
        options ??= new SvgDirectedGraphRenderOptions();
        ValidateOptions(options);
        ValidateGraph(graph, options);

        var nodes = graph.Nodes.OrderBy(n => n.Id, StringComparer.Ordinal).ToArray();
        var edges = graph.Edges.OrderBy(e => e.Id, StringComparer.Ordinal).ToArray();
        var clusters = (graph.Clusters ?? Array.Empty<SvgGraphCluster>())
            .OrderBy(c => c.Id, StringComparer.Ordinal).ToArray();
        var nodeBounds = Layout(nodes, graph.FocalNodeId, options);
        var clusterBounds = BuildClusterBounds(clusters, nodeBounds, options);

        var allBounds = nodeBounds.Values.Aggregate((a, b) => Bounds.Union(a, b));
        foreach (var bounds in clusterBounds.Values)
            allBounds = Bounds.Union(allBounds, bounds);
        if (graph.FocalNodeId is not null)
        {
            var focal = nodeBounds[graph.FocalNodeId];
            var centerX = focal.X + focal.Width / 2;
            var centerY = focal.Y + focal.Height / 2;
            var halfWidth = Math.Max(centerX - allBounds.X, allBounds.Right - centerX);
            var halfHeight = Math.Max(centerY - allBounds.Y, allBounds.Bottom - centerY);
            allBounds = new Bounds(centerX - halfWidth, centerY - halfHeight, halfWidth * 2, halfHeight * 2);
        }
        var canvas = allBounds.Expand(options.Margin);
        var width = Math.Max(1, canvas.Width);
        var height = Math.Max(1, canvas.Height);
        var titleId = "graph-title";
        var descId = "graph-description";
        var title = graph.Title ?? "Directed graph";
        var description = graph.Description ?? $"{nodes.Length} nodes, {edges.Length} directed edges, {clusters.Length} explicit clusters.";

        var svg = new XElement(Svg + "svg",
            new XAttribute("xmlns", Svg.NamespaceName),
            new XAttribute("width", F(width)),
            new XAttribute("height", F(height)),
            new XAttribute("viewBox", $"0 0 {F(width)} {F(height)}"),
            new XAttribute("role", "img"),
            new XAttribute("aria-labelledby", $"{titleId} {descId}"),
            new XAttribute("font-family", "Segoe UI, Arial, sans-serif"),
            new XElement(Svg + "title", new XAttribute("id", titleId), title),
            new XElement(Svg + "desc", new XAttribute("id", descId), description),
            new XElement(Svg + "defs",
                new XElement(Svg + "marker",
                    new XAttribute("id", "graph-arrow"), new XAttribute("markerWidth", "9"),
                    new XAttribute("markerHeight", "9"), new XAttribute("refX", "8"),
                    new XAttribute("refY", "4.5"), new XAttribute("orient", "auto"),
                    new XElement(Svg + "path", new XAttribute("d", "M0,0 L9,4.5 L0,9 z"),
                        new XAttribute("fill", "#315b72")))));

        // Translate every primitive into a positive, padded viewBox.
        Bounds TranslateBounds(Bounds b) => new(b.X - canvas.X, b.Y - canvas.Y, b.Width, b.Height);

        svg.Add(new XElement(Svg + "rect", new XAttribute("width", F(width)),
            new XAttribute("height", F(height)), new XAttribute("fill", "#ffffff")));

        // Containers are painted first. A title gap is created by drawing two top-border segments.
        foreach (var cluster in clusters.OrderBy(c => ClusterDepth(c, clusters), Comparer<int>.Default)
                     .ThenBy(c => c.Id, StringComparer.Ordinal))
        {
            var b = TranslateBounds(clusterBounds[cluster.Id]);
            var titleWidth = Math.Min(b.Width - 28, Math.Max(42, cluster.Title.Length * 8 + 20));
            var gapStart = b.X + (b.Width - titleWidth) / 2;
            var gapEnd = gapStart + titleWidth;
            var group = new XElement(Svg + "g", new XAttribute("data-cluster-id", cluster.Id),
                new XElement(Svg + "path",
                    new XAttribute("d", $"M {F(b.X)} {F(b.Y + 14)} V {F(b.Bottom)} H {F(b.Right)} V {F(b.Y + 14)} M {F(b.X)} {F(b.Y + 14)} H {F(gapStart)} M {F(gapEnd)} {F(b.Y + 14)} H {F(b.Right)}"),
                    new XAttribute("fill", "none"), new XAttribute("stroke", "#687b89"),
                    new XAttribute("stroke-width", "1.6"), new XAttribute("stroke-dasharray", "7 5")),
                new XElement(Svg + "text", new XAttribute("x", F(b.X + b.Width / 2)),
                    new XAttribute("y", F(b.Y + 5)), new XAttribute("text-anchor", "middle"),
                    new XAttribute("dominant-baseline", "hanging"), new XAttribute("font-size", "13"),
                    new XAttribute("font-weight", "600"), new XAttribute("fill", "#344955"), cluster.Title));
            svg.Add(group);
        }

        // Edges are stable-sorted. Parallel edges receive deterministic offsets.
        foreach (var edgeGroup in edges.GroupBy(e => (e.SourceNodeId, e.TargetNodeId)))
        {
            var parallel = edgeGroup.OrderBy(e => e.Id, StringComparer.Ordinal).ToArray();
            for (var i = 0; i < parallel.Length; i++)
            {
                var edge = parallel[i];
                var source = TranslateBounds(nodeBounds[edge.SourceNodeId]);
                var target = TranslateBounds(nodeBounds[edge.TargetNodeId]);
                var offset = (i - (parallel.Length - 1) / 2.0) * 12;
                var start = new Point(source.X + source.Width / 2 + offset, source.Y + source.Height / 2);
                var end = new Point(target.X + target.Width / 2 + offset, target.Y + target.Height / 2);
                XElement path;
                if (edge.SourceNodeId == edge.TargetNodeId)
                {
                    path = new XElement(Svg + "path", new XAttribute("d",
                        $"M {F(source.Right)} {F(source.Y + 18 + offset)} C {F(source.Right + 55)} {F(source.Y - 8 + offset)}, {F(source.Right + 55)} {F(source.Y + 58 + offset)}, {F(source.Right)} {F(source.Y + 48 + offset)}"));
                }
                else
                {
                    var clipped = ClipToRect(start, end, source, target);
                    var midX = (clipped.Start.X + clipped.End.X) / 2;
                    var midY = (clipped.Start.Y + clipped.End.Y) / 2;
                    var bend = Math.Abs(clipped.End.Y - clipped.Start.Y) > Math.Abs(clipped.End.X - clipped.Start.X);
                    var d = bend
                        ? $"M {F(clipped.Start.X)} {F(clipped.Start.Y)} L {F(clipped.Start.X + offset)} {F(midY)} L {F(clipped.End.X + offset)} {F(midY)} L {F(clipped.End.X)} {F(clipped.End.Y)}"
                        : $"M {F(clipped.Start.X)} {F(clipped.Start.Y)} L {F(midX)} {F(clipped.Start.Y + offset)} L {F(midX)} {F(clipped.End.Y + offset)} L {F(clipped.End.X)} {F(clipped.End.Y)}";
                    path = new XElement(Svg + "path", new XAttribute("d", d));
                }
                path.SetAttributeValue("fill", "none");
                path.SetAttributeValue("stroke", "#315b72");
                path.SetAttributeValue("stroke-width", "1.8");
                path.SetAttributeValue("marker-end", "url(#graph-arrow)");
                path.SetAttributeValue("data-edge-id", edge.Id);
                if (edge.Category is not null) path.SetAttributeValue("data-category", edge.Category);
                if (edge.HttpsLink is not null)
                    svg.Add(new XElement(Svg + "a", new XAttribute("href", edge.HttpsLink), path));
                else svg.Add(path);

                if (!string.IsNullOrWhiteSpace(edge.Label))
                    svg.Add(new XElement(Svg + "text",
                        new XAttribute("x", F((TranslateBounds(nodeBounds[edge.SourceNodeId]).X + TranslateBounds(nodeBounds[edge.TargetNodeId]).X) / 2 + options.NodeWidth / 2)),
                        new XAttribute("y", F((TranslateBounds(nodeBounds[edge.SourceNodeId]).Y + TranslateBounds(nodeBounds[edge.TargetNodeId]).Y) / 2 - 7 + offset)),
                        new XAttribute("text-anchor", "middle"), new XAttribute("font-size", "11"),
                        new XAttribute("fill", "#344955"), edge.Label));
            }
        }

        foreach (var node in nodes)
        {
            var b = TranslateBounds(nodeBounds[node.Id]);
            var group = new XElement(Svg + "g", new XAttribute("data-node-id", node.Id),
                new XElement(Svg + "rect", new XAttribute("x", F(b.X)), new XAttribute("y", F(b.Y)),
                    new XAttribute("width", F(b.Width)), new XAttribute("height", F(b.Height)),
                    new XAttribute("rx", "8"), new XAttribute("fill", DefaultNodeFill),
                    new XAttribute("stroke", node.Id == graph.FocalNodeId ? "#9a5b16" : DefaultNodeStroke),
                    new XAttribute("stroke-width", node.Id == graph.FocalNodeId ? "3" : "1.6")),
                new XElement(Svg + "text", new XAttribute("x", F(b.X + b.Width / 2)),
                    new XAttribute("y", F(b.Y + 27)), new XAttribute("text-anchor", "middle"),
                    new XAttribute("font-size", "14"), new XAttribute("font-weight", "600"),
                    new XAttribute("fill", "#172b36"), node.Label));
            if (!string.IsNullOrWhiteSpace(node.Subtitle))
                group.Add(new XElement(Svg + "text", new XAttribute("x", F(b.X + b.Width / 2)),
                    new XAttribute("y", F(b.Y + 49)), new XAttribute("text-anchor", "middle"),
                    new XAttribute("font-size", "11"), new XAttribute("fill", "#405b68"), node.Subtitle));
            if (node.Category is not null) group.SetAttributeValue("data-category", node.Category);
            if (node.HttpsLink is not null)
                svg.Add(new XElement(Svg + "a", new XAttribute("href", node.HttpsLink), group));
            else svg.Add(group);
        }

        if (graph.Legend is { Count: > 0 })
        {
            var y = height - 18;
            var x = 12.0;
            foreach (var item in graph.Legend.OrderBy(i => i.Category, StringComparer.Ordinal))
            {
                svg.Add(new XElement(Svg + "rect", new XAttribute("x", F(x)), new XAttribute("y", F(y - 10)),
                    new XAttribute("width", "12"), new XAttribute("height", "12"),
                    new XAttribute("fill", "#f7fafc"), new XAttribute("stroke", "#315b72")));
                svg.Add(new XElement(Svg + "text", new XAttribute("x", F(x + 18)),
                    new XAttribute("y", F(y)), new XAttribute("font-size", "11"), item.Label));
                x += 32 + item.Label.Length * 7;
            }
        }
        if (!string.IsNullOrWhiteSpace(graph.ProvenanceCaption))
            svg.Add(new XElement(Svg + "text", new XAttribute("x", F(width - 12)),
                new XAttribute("y", F(height - 6)), new XAttribute("text-anchor", "end"),
                new XAttribute("font-size", "9"), new XAttribute("fill", "#5c6870"),
                graph.ProvenanceCaption));

        return new XDocument(new XDeclaration("1.0", "utf-8", null), svg)
            .ToString(SaveOptions.DisableFormatting);
    }

    private static Dictionary<string, Bounds> Layout(SvgGraphNode[] nodes, string? focalId, SvgDirectedGraphRenderOptions o)
    {
        var result = new Dictionary<string, Bounds>(StringComparer.Ordinal);
        var others = nodes.Where(n => n.Id != focalId).OrderBy(n => n.Id, StringComparer.Ordinal).ToArray();
        var columns = Math.Max(1, (int)Math.Ceiling(Math.Sqrt(nodes.Length)));
        var rows = Math.Max(1, (int)Math.Ceiling(nodes.Length / (double)columns));
        if (focalId is not null)
        {
            // Reserve a true central cell. Odd grid dimensions keep the focal rectangle
            // centered in the canvas without overlapping adjacent node slots.
            columns = Math.Max(3, columns);
            rows = Math.Max(1, (int)Math.Ceiling((nodes.Length + 1) / (double)columns));
            if (columns % 2 == 0) columns++;
            if (rows % 2 == 0) rows++;
        }
        var centerColumn = (columns - 1) / 2.0;
        var centerRow = (rows - 1) / 2.0;
        if (focalId is not null)
            result[focalId] = new Bounds(o.Margin + centerColumn * (o.NodeWidth + o.HorizontalGap),
                o.Margin + centerRow * (o.NodeHeight + o.VerticalGap), o.NodeWidth, o.NodeHeight);
        var slots = Enumerable.Range(0, rows * columns)
            .Select(i => (Row: i / columns, Column: i % columns))
            .Where(p => focalId is null || p.Row != (int)centerRow || p.Column != (int)centerColumn)
            .OrderBy(p => Math.Pow(p.Row - centerRow, 2) + Math.Pow(p.Column - centerColumn, 2))
            .ThenBy(p => p.Row).ThenBy(p => p.Column).ToArray();
        foreach (var (node, slot) in others.Zip(slots))
            result[node.Id] = new Bounds(o.Margin + slot.Column * (o.NodeWidth + o.HorizontalGap),
                o.Margin + slot.Row * (o.NodeHeight + o.VerticalGap), o.NodeWidth, o.NodeHeight);
        return result;
    }

    private static Dictionary<string, Bounds> BuildClusterBounds(SvgGraphCluster[] clusters,
        Dictionary<string, Bounds> nodes, SvgDirectedGraphRenderOptions o)
    {
        var result = new Dictionary<string, Bounds>(StringComparer.Ordinal);
        var byId = clusters.ToDictionary(c => c.Id, StringComparer.Ordinal);
        Bounds Resolve(string id, HashSet<string> visiting)
        {
            if (result.TryGetValue(id, out var cached)) return cached;
            if (!visiting.Add(id)) throw new ArgumentException("Cluster membership contains a cycle.", nameof(clusters));
            var cluster = byId[id];
            var members = cluster.MemberNodeIds.Select(nodeId => nodes[nodeId])
                .Concat((cluster.MemberClusterIds ?? Array.Empty<string>()).Select(child => Resolve(child, visiting)))
                .ToArray();
            var bounds = members.Aggregate(Bounds.Union).Expand(26);
            visiting.Remove(id);
            result[id] = bounds;
            return bounds;
        }
        foreach (var cluster in clusters) Resolve(cluster.Id, new HashSet<string>(StringComparer.Ordinal));
        return result;
    }

    private static (Point Start, Point End) ClipToRect(Point sourceCenter, Point targetCenter, Bounds source, Bounds target)
    {
        static Point Intersect(Point from, Point to, Bounds b)
        {
            var dx = to.X - from.X; var dy = to.Y - from.Y;
            var scaleX = Math.Abs(dx) < 0.0001 ? double.PositiveInfinity : b.Width / 2 / Math.Abs(dx);
            var scaleY = Math.Abs(dy) < 0.0001 ? double.PositiveInfinity : b.Height / 2 / Math.Abs(dy);
            var scale = Math.Min(scaleX, scaleY);
            return new Point(from.X + dx * scale, from.Y + dy * scale);
        }
        return (Intersect(sourceCenter, targetCenter, source), Intersect(targetCenter, sourceCenter, target));
    }

    private static int ClusterDepth(SvgGraphCluster cluster, SvgGraphCluster[] clusters)
    {
        var byId = clusters.ToDictionary(c => c.Id, StringComparer.Ordinal);
        var depth = 0; var current = cluster;
        var seen = new HashSet<string>(StringComparer.Ordinal);
        while (current.MemberClusterIds is { Count: > 0 } children && seen.Add(current.Id))
        {
            var parent = clusters.FirstOrDefault(c => c.MemberClusterIds?.Contains(current.Id, StringComparer.Ordinal) == true);
            if (parent is null) break;
            current = parent; depth++;
        }
        return depth;
    }

    private static void ValidateGraph(SvgDirectedGraph graph, SvgDirectedGraphRenderOptions o)
    {
        if (graph.Nodes is null || graph.Edges is null) throw new ArgumentException("Nodes and edges are required.");
        if (graph.Nodes.Count == 0 || graph.Nodes.Count > o.MaximumNodes) throw new ArgumentException("Node count is outside the configured bound.");
        if (graph.Edges.Count > o.MaximumEdges) throw new ArgumentException("Edge count exceeds the configured bound.");
        var clusters = graph.Clusters ?? Array.Empty<SvgGraphCluster>();
        if (clusters.Count > o.MaximumClusters) throw new ArgumentException("Cluster count exceeds the configured bound.");
        static void UniqueIds(IEnumerable<string> ids, string kind)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var id in ids)
                if (string.IsNullOrWhiteSpace(id) || !StableId.IsMatch(id) || !seen.Add(id))
                    throw new ArgumentException($"{kind} IDs must be unique stable XML-safe identifiers.");
        }
        UniqueIds(graph.Nodes.Select(n => n.Id), "Node");
        UniqueIds(graph.Edges.Select(e => e.Id), "Edge");
        UniqueIds(clusters.Select(c => c.Id), "Cluster");
        var nodeIds = graph.Nodes.Select(n => n.Id).ToHashSet(StringComparer.Ordinal);
        var clusterIds = clusters.Select(c => c.Id).ToHashSet(StringComparer.Ordinal);
        if (graph.FocalNodeId is not null && !nodeIds.Contains(graph.FocalNodeId)) throw new ArgumentException("Focal node does not exist.");
        foreach (var node in graph.Nodes) { _ = Required(node.Label, "Node label"); ValidateLink(node.HttpsLink); }
        foreach (var edge in graph.Edges)
        {
            if (!nodeIds.Contains(edge.SourceNodeId) || !nodeIds.Contains(edge.TargetNodeId))
                throw new ArgumentException($"Edge '{edge.Id}' has a dangling endpoint.");
            ValidateLink(edge.HttpsLink);
        }
        foreach (var cluster in clusters)
        {
            _ = Required(cluster.Title, "Cluster title");
            if (cluster.MemberNodeIds.Count == 0 && (cluster.MemberClusterIds?.Count ?? 0) == 0)
                throw new ArgumentException($"Cluster '{cluster.Id}' must contain at least one explicit member.");
            if (cluster.MemberNodeIds.Any(id => !nodeIds.Contains(id)) ||
                (cluster.MemberClusterIds ?? Array.Empty<string>()).Any(id => !clusterIds.Contains(id) || id == cluster.Id))
                throw new ArgumentException($"Cluster '{cluster.Id}' references an unknown member.");
        }
        if (graph.Legend is not null) UniqueIds(graph.Legend.Select(l => l.Category), "Legend category");
        _ = Required(graph.Title ?? "Directed graph", "Graph title");
    }

    private static void ValidateLink(string? link)
    {
        if (link is null) return;
        if (!Uri.TryCreate(link, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps ||
            string.IsNullOrEmpty(uri.Host) || !string.IsNullOrEmpty(uri.UserInfo))
            throw new ArgumentException("Graph links must be absolute HTTPS URLs without embedded credentials.");
    }

    private static string Required(string value, string field) =>
        string.IsNullOrWhiteSpace(value) ? throw new ArgumentException($"{field} cannot be empty.") : value;

    private static void ValidateOptions(SvgDirectedGraphRenderOptions o)
    {
        if (o.NodeWidth < 100 || o.NodeHeight < 50 || o.HorizontalGap < 20 || o.VerticalGap < 20 ||
            o.Margin < 12 || o.MaximumNodes < 1 || o.MaximumEdges < 0 || o.MaximumClusters < 0)
            throw new ArgumentOutOfRangeException(nameof(o), "Graph dimensions and bounds must be positive and readable.");
    }

    private static string F(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);
}
