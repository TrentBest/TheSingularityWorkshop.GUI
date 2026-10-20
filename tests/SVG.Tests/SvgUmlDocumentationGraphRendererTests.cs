using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;
using Xunit;

namespace TheSingularityWorkshop.GUI.SVG.Tests;

public sealed class SvgUmlDocumentationGraphRendererTests
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

    [Fact]
    public void Renders_focused_semantic_neighborhood_with_accessible_relationships()
    {
        var source = new UmlSourceDeclaration("Fixture/A.cs", new UmlSourceSpan(0, 20, 1, 1, 1, 21));
        var a = new UmlDocumentedType(
            "type:Fixture:A",
            new UmlTypeIdentity("Fixture", "Fixture", [], "A", 0, "class"),
            [source],
            [new UmlDocumentedMember("member:A:Link", "Property", "Link", "Fixture.B Link", [source])]);
        var b = new UmlDocumentedType(
            "type:Fixture:B",
            new UmlTypeIdentity("Fixture", "Fixture", [], "B", 0, "class"),
            [new UmlSourceDeclaration("Fixture/B.cs", new UmlSourceSpan(0, 12, 1, 1, 1, 13))],
            []);
        var external = new UmlDocumentedRelationship(
            "edge:A:external",
            UmlRelationshipKind.Dependency,
            a.Id,
            null,
            "global::System.IO.Stream",
            UmlRelationshipResolution.External,
            "System.Runtime, Version=8.0.0.0",
            "source-to-target",
            "parameter type stream",
            null,
            null,
            new UmlRelationshipProvenance("roslyn-member-symbol", source.Path, source.Span, "A.Open(Stream stream)"));
        var model = new UmlDocumentationModel(
            "1.0",
            new UmlModelProvenance("owner/repo", "abc123", "Fixture/Fixture.csproj", "Fixture", "net8.0", "Release", "project-semantic", null),
            [a, b],
            [
                new UmlDocumentedRelationship(
                    "edge:A:B",
                    UmlRelationshipKind.Association,
                    a.Id,
                    b.Id,
                    "global::Fixture.B",
                    UmlRelationshipResolution.Resolved,
                    null,
                    "source-to-target",
                    "property type",
                    null,
                    null,
                    new UmlRelationshipProvenance("roslyn-member-symbol", source.Path, source.Span, "Fixture.B Link")),
                external
            ]);

        var document = XDocument.Parse(new SvgUmlDocumentationGraphRenderer().Render(model, a.Id));
        Assert.Equal("Source-derived UML neighborhood: Fixture.A", document.Root!.Element(Svg + "title")!.Value);
        Assert.Contains("2 type nodes and 2 typed relationships", document.Root.Element(Svg + "desc")!.Value);
        var text = string.Join(" ", document.Descendants(Svg + "text").Select(element => element.Value));
        Assert.Contains("Fixture.B", text);
        Assert.Contains("global::System.IO.Stream", text);
        Assert.Contains("association · property type", text);
        Assert.Contains("dependency · parameter type stream", text);

        var ids = document.Descendants().Attributes("id").Select(attribute => attribute.Value).ToArray();
        Assert.Equal(ids.Length, ids.Distinct(StringComparer.Ordinal).Count());
        Assert.Contains(document.Descendants(Svg + "line"), line =>
            ((string?)line.Attribute("marker-end")) == "url(#uml-arrow)");
    }

    [Fact]
    public void Rejects_unknown_focus_type_id()
    {
        var model = new UmlDocumentationModel(
            "1.0",
            new UmlModelProvenance("owner/repo", "abc123", "Fixture.csproj", "Fixture", "net8.0", "Release", "project-semantic", null),
            [],
            []);

        Assert.Throws<ArgumentException>(() => new SvgUmlDocumentationGraphRenderer().Render(model, "missing"));
    }

    [Fact]
    public void Rejects_too_narrow_graph_width()
    {
        var model = new UmlDocumentationModel(
            "1.0",
            new UmlModelProvenance("owner/repo", "abc123", "Fixture.csproj", "Fixture", "net8.0", "Release", "project-semantic", null),
            [],
            []);

        Assert.Throws<ArgumentOutOfRangeException>(() => new SvgUmlDocumentationGraphRenderer().Render(model, "missing", 400));
    }
}
