# Directed graph SVG renderer

GUI.SVG can render a renderer-neutral directed graph without taking ownership of the graph's meaning. The consumer owns identities, dependency semantics, evidence, authorization, and validation. GUI.SVG owns deterministic placement, connector geometry, accessible SVG, and serialization.

## Contract

- `SvgDirectedGraph` is an immutable input snapshot of nodes, directed edges, explicit clusters, legend entries, descriptive metadata, provenance caption, and an optional focal node.
- Node and edge identifiers are stable XML-safe identifiers. Duplicate IDs and dangling endpoints are rejected.
- A cluster lists its node members and child-cluster members explicitly. The same node may be listed by more than one cluster (overlap); a cluster may contain child clusters (nesting). The renderer never derives membership from proximity, edge direction, category, or visual style.
- The optional focal node is emphasized and placed at the center of the deterministic layout. Other nodes are placed in stable identifier order. The renderer does not interpret edge meaning to decide placement.
- Optional links must be absolute HTTPS URLs without embedded credentials.
- SVG is emitted as a standalone document with `title`, `desc`, `role="img"`, stable `aria-labelledby`, a responsive `viewBox`, directed arrow markers, node/edge labels, and a visible caller-supplied legend.
- XML text and attributes are serialized through `XElement`; no scripts, external resources, or runtime interaction are added.

The renderer is deliberately not a generic diagram language. It does not decide whether an edge means dependency, execution order, authorization, or readiness. It does not claim that a partial graph is complete.

## Example

```csharp
using TheSingularityWorkshop.Workshop.Gui.Svg;

var graph = new SvgDirectedGraph(
    Nodes: new[]
    {
        new SvgGraphNode("repo-ciai", "CiAi", "Focal repository", "repository"),
        new SvgGraphNode("repo-gui", "GUI", "SVG renderer", "repository"),
        new SvgGraphNode("repo-fsm", "FSM_API", "State machinery", "repository")
    },
    Edges: new[]
    {
        new SvgGraphEdge("edge-ciai-gui", "repo-ciai", "repo-gui", "uses renderer"),
        new SvgGraphEdge("edge-ciai-fsm", "repo-ciai", "repo-fsm", "depends on")
    },
    Clusters: new[]
    {
        new SvgGraphCluster("cluster-adp", "ADP",
            new[] { "repo-ciai", "repo-gui", "repo-fsm" })
    },
    Legend: new[] { new SvgGraphLegendEntry("repository", "Repository") },
    Title: "Repository graph",
    Description: "A caller-defined repository relationship projection.",
    ProvenanceCaption: "Configuration view; not execution evidence",
    FocalNodeId: "repo-ciai");

var svg = new SvgDirectedGraphRenderer().Render(graph);
File.WriteAllText("repository-graph.svg", svg);
```

A nested cluster is expressed by listing its ID in a parent cluster's `MemberClusterIds`. Overlapping membership is expressed by listing the same node ID in multiple clusters. A cycle in cluster containment is invalid and rejected.

## Layout and bounds

The renderer uses stable IDs rather than dictionary iteration order or timestamps. Its default input bounds are 250 nodes, 1,000 edges, and 100 clusters; callers can tune the limits through `SvgDirectedGraphRenderOptions`. Layout is intentionally deterministic rather than optimized: it prioritizes repeatable artifacts and an obvious focal node over crossing minimization. Parallel edges are ordered by edge ID and receive deterministic offsets. Self-edges receive a loop.

The current implementation is an initial graph layout capability. It does not promise collision-free routing for every dense graph, optimal cluster packing for arbitrary overlapping sets, or an interactive pan/zoom experience. Those are separate improvements that must preserve the same semantic ownership boundary.

## Embedding

Write the generated SVG and a companion Markdown file from the same validated graph snapshot. Use a relative path so the pair can be consumed from a repository checkout, a downloaded artifact, or a Pages artifact:

```markdown
![Repository dependency graph](./REPOSITORY_BUILD_GRAPH.svg)
```

For CI/Pages, generate into the build output and validate the artifact pair. Do not push generated output back to a branch as a side effect. A provenance caption should identify the source revision and projection type when the consumer has that evidence.

## Ownership and coordination

The initial consumer is CiAi's repository configuration and federated ecosystem graph. CiAi owns the typed snapshot, dependency semantics, explicit ADP membership, provenance, freshness, and partial-discovery disclosure. GUI.SVG remains independent of CiAi and only renders the supplied graph.

- [GUI issue #90 — generic directed graph renderer](https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues/90)
- [CiAi issue #102 — federated ecosystem and ADP cluster graphs](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/issues/102)
- [CiAi repository build graph contract](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/blob/development/docs/REPOSITORY_BUILD_GRAPH.md)
