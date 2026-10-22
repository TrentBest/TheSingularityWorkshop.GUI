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
        var consumer = new UmlDocumentedType(
            "type:Fixture:Consumer",
            new UmlTypeIdentity("Fixture", "Fixture", [], "Consumer", 0, "class"),
            [new UmlSourceDeclaration("Fixture/Consumer.cs", new UmlSourceSpan(0, 16, 1, 1, 1, 17))],
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
            [a, b, consumer],
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
                external,
                new UmlDocumentedRelationship(
                    "edge:consumer:A",
                    UmlRelationshipKind.Dependency,
                    consumer.Id,
                    a.Id,
                    "global::Fixture.A",
                    UmlRelationshipResolution.Resolved,
                    null,
                    "source-to-target",
                    "constructor parameter",
                    null,
                    null,
                    new UmlRelationshipProvenance("roslyn-member-symbol", "Fixture/Consumer.cs",
                        new UmlSourceSpan(0, 16, 1, 1, 1, 17), "Consumer(A target)"))
            ]);

        var document = XDocument.Parse(new SvgUmlDocumentationGraphRenderer().Render(model, a.Id));
        Assert.Equal("Source-derived UML neighborhood: Fixture.A", document.Root!.Element(Svg + "title")!.Value);
        Assert.Contains("4 type nodes and 3 typed relationships", document.Root.Element(Svg + "desc")!.Value);
        var text = string.Join(" ", document.Descendants(Svg + "text").Select(element => element.Value));
        Assert.Contains("Fixture.B", text);
        Assert.Contains(document.Descendants(Svg + "title"), title => title.Value.Contains("global::System.IO.Stream", StringComparison.Ordinal));
        Assert.Contains("association · property type", text);
        Assert.Contains("dependency · parameter type stream", text);

        var ids = document.Descendants().Attributes("id").Select(attribute => attribute.Value).ToArray();
        Assert.Equal(ids.Length, ids.Distinct(StringComparer.Ordinal).Count());
        Assert.Contains(document.Descendants(Svg + "line"), line =>
            ((string?)line.Attribute("marker-end")) == "url(#uml-arrow)");

        var nodeGroups = document.Descendants(Svg + "g")
            .Where(group => ((string?)group.Attribute("id"))?.StartsWith("uml-node-", StringComparison.Ordinal) == true)
            .ToDictionary(group => (string)group.Attribute("data-type-id")!,
                group => group.Element(Svg + "rect")!, StringComparer.Ordinal);
        var consumerY = double.Parse((string)nodeGroups["type:Fixture:Consumer"].Attribute("y")!, System.Globalization.CultureInfo.InvariantCulture);
        var focusY = double.Parse((string)nodeGroups["type:Fixture:A"].Attribute("y")!, System.Globalization.CultureInfo.InvariantCulture);
        var dependencyY = double.Parse((string)nodeGroups["type:Fixture:B"].Attribute("y")!, System.Globalization.CultureInfo.InvariantCulture);
        var focusX = double.Parse((string)nodeGroups["type:Fixture:A"].Attribute("x")!, System.Globalization.CultureInfo.InvariantCulture);
        var externalX = double.Parse((string)document.Descendants(Svg + "g")
            .Where(group => ((string?)group.Attribute("id"))?.StartsWith("uml-node-", StringComparison.Ordinal) == true)
            .Single(group => group.Element(Svg + "title")!.Value.StartsWith("global::System.IO.Stream", StringComparison.Ordinal))
            .Element(Svg + "rect")!.Attribute("x")!, System.Globalization.CultureInfo.InvariantCulture);
        Assert.True(consumerY < focusY, "verified consumers must appear above the focus");
        Assert.True(focusY < dependencyY, "dependencies used by the focus must appear below it");
        Assert.True(focusX > 100, "the focus should be centered within the available neighborhood, not pinned to the left");
        Assert.True(externalX > focusX, "external/platform references must appear to the right");
        var focusCenterY = focusY + double.Parse((string)nodeGroups["type:Fixture:A"].Attribute("height")!, System.Globalization.CultureInfo.InvariantCulture) / 2;
        var canvasHeight = double.Parse((string)document.Root!.Attribute("height")!, System.Globalization.CultureInfo.InvariantCulture);
        Assert.InRange(Math.Abs(focusCenterY - canvasHeight / 2), 0, 1);
        Assert.Contains(document.Descendants(Svg + "g"), group =>
            ((string?)group.Attribute("data-type-id")) == "type:Fixture:A" &&
            ((string?)group.Attribute("transform"))?.StartsWith("rotate(180", StringComparison.Ordinal) == true);
        Assert.Contains(document.Descendants(Svg + "g"), group =>
            group.Attribute("data-type-id") is not null &&
            group.Attribute("data-owner-type-id") is not null &&
            group.Element(Svg + "title")?.Value.Contains("Type conduit", StringComparison.Ordinal) == true);
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
