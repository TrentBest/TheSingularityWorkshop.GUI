using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui.Svg;
using Xunit;

namespace TheSingularityWorkshop.GUI.SVG.Tests;

public sealed class SvgDirectedGraphRendererTests
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";
    private readonly SvgDirectedGraphRenderer _renderer = new();

    [Fact]
    public void Output_is_deterministic_when_input_collections_are_reordered()
    {
        var first = CreateGraph();
        var second = first with
        {
            Nodes = first.Nodes.Reverse().ToArray(),
            Edges = first.Edges.Reverse().ToArray(),
            Clusters = first.Clusters!.Reverse().ToArray(),
            Legend = first.Legend!.Reverse().ToArray()
        };

        Assert.Equal(_renderer.Render(first), _renderer.Render(second));
    }

    [Fact]
    public void Renders_accessible_metadata_directional_edges_and_explicit_nested_clusters()
    {
        var document = XDocument.Parse(_renderer.Render(CreateGraph()));
        var root = document.Root!;

        Assert.Equal("img", (string?)root.Attribute("role"));
        Assert.Equal("graph-title graph-description", (string?)root.Attribute("aria-labelledby"));
        Assert.Contains("Repository dependency graph", root.Element(Svg + "title")!.Value);
        Assert.Contains("Read-only configuration projection", root.Element(Svg + "desc")!.Value);
        Assert.Equal(2, root.Descendants(Svg + "g").Count(x => x.Attribute("data-cluster-id") is not null));
        Assert.Contains(root.Descendants(Svg + "path"), x => ((string?)x.Attribute("d"))?.Contains(" V ", StringComparison.Ordinal) == true);
        Assert.Contains(root.Descendants(Svg + "path"), x => (string?)x.Attribute("marker-end") == "url(#graph-arrow)");
        Assert.Contains(root.Descendants(Svg + "text"), x => x.Value == "depends on");
        Assert.Contains(root.Descendants(Svg + "text"), x => x.Value == "ADP");
    }

    [Fact]
    public void Escapes_text_and_emits_only_validated_https_links()
    {
        var graph = new SvgDirectedGraph(
            new[] { new SvgGraphNode("repo-a", "A < B & C", HttpsLink: "https://example.com/repo-a") },
            Array.Empty<SvgGraphEdge>(),
            Title: "Graph <safe>");
        var markup = _renderer.Render(graph);
        var document = XDocument.Parse(markup);

        Assert.Contains("A &lt; B &amp; C", markup);
        Assert.Contains("Graph &lt;safe&gt;", markup);
        Assert.Equal("https://example.com/repo-a",
            (string?)document.Descendants(Svg + "a").Single().Attribute("href"));
        Assert.Throws<ArgumentException>(() => _renderer.Render(graph with
        {
            Nodes = new[] { new SvgGraphNode("repo-a", "Unsafe", HttpsLink: "javascript:alert(1)") }
        }));
    }

    [Fact]
    public void Rejects_duplicate_ids_dangling_edges_and_cluster_cycles()
    {
        var duplicate = new SvgDirectedGraph(
            new[] { new SvgGraphNode("same", "One"), new SvgGraphNode("same", "Two") },
            Array.Empty<SvgGraphEdge>());
        Assert.Throws<ArgumentException>(() => _renderer.Render(duplicate));

        var dangling = new SvgDirectedGraph(
            new[] { new SvgGraphNode("present", "Present") },
            new[] { new SvgGraphEdge("edge", "present", "missing") });
        Assert.Throws<ArgumentException>(() => _renderer.Render(dangling));

        var cyclic = new SvgDirectedGraph(
            new[] { new SvgGraphNode("node", "Node") },
            Array.Empty<SvgGraphEdge>(),
            new[]
            {
                new SvgGraphCluster("cluster-a", "A", new[] { "node" }, new[] { "cluster-b" }),
                new SvgGraphCluster("cluster-b", "B", new[] { "node" }, new[] { "cluster-a" })
            });
        Assert.Throws<ArgumentException>(() => _renderer.Render(cyclic));
    }

    [Fact]
    public void Rejects_unbounded_graphs_and_non_https_links()
    {
        var nodes = Enumerable.Range(0, 251).Select(i => new SvgGraphNode($"node-{i}", $"Node {i}")).ToArray();
        Assert.Throws<ArgumentException>(() => _renderer.Render(new SvgDirectedGraph(nodes, Array.Empty<SvgGraphEdge>())));
        Assert.Throws<ArgumentException>(() => _renderer.Render(new SvgDirectedGraph(
            new[] { new SvgGraphNode("node", "Node", HttpsLink: "http://example.com") },
            Array.Empty<SvgGraphEdge>())));
    }

    private static SvgDirectedGraph CreateGraph() => new(
        new[]
        {
            new SvgGraphNode("repo-a", "Repository A", "Focal repository", "repository"),
            new SvgGraphNode("repo-b", "Repository B", "Shared dependency", "repository"),
            new SvgGraphNode("repo-c", "Repository C", "Dependent", "repository")
        },
        new[]
        {
            new SvgGraphEdge("edge-a-b", "repo-a", "repo-b", "depends on"),
            new SvgGraphEdge("edge-c-a", "repo-c", "repo-a", "depends on")
        },
        new[]
        {
            new SvgGraphCluster("cluster-adp", "ADP", new[] { "repo-a" }, new[] { "cluster-member" }),
            new SvgGraphCluster("cluster-member", "Member set", new[] { "repo-a", "repo-b" })
        },
        new[] { new SvgGraphLegendEntry("repository", "Repository") },
        "Repository dependency graph",
        "Read-only configuration projection",
        "Source: exact commit SHA; evidence is read-only",
        "repo-a");
}
