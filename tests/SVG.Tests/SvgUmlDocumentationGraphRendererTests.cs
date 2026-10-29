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
            new UmlModelProvenance("TrentBest/TheSingularityWorkshop.GUI", "0123456789abcdef0123456789abcdef01234567", "Fixture/Fixture.csproj", "Fixture", "net8.0", "Release", "project-semantic", null),
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
                    "edge:A:self",
                    UmlRelationshipKind.Association,
                    a.Id,
                    a.Id,
                    "global::Fixture.A",
                    UmlRelationshipResolution.Resolved,
                    null,
                    "source-to-target",
                    "self-reference",
                    null,
                    null,
                    new UmlRelationshipProvenance("roslyn-member-symbol", source.Path, source.Span, "A.Link -> A")),
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
        var reviewArtifactPath = Environment.GetEnvironmentVariable("GUI_UML_REVIEW_ARTIFACT");
        if (!string.IsNullOrWhiteSpace(reviewArtifactPath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(reviewArtifactPath))!);
            document.Save(reviewArtifactPath);
        }
        Assert.Equal("Source-derived UML neighborhood: Fixture.A", document.Root!.Element(Svg + "title")!.Value);
        Assert.Contains(document.Root.Elements(Svg + "rect"), rect =>
            ((string?)rect.Attribute("data-canvas-background")) == "true" &&
            ((string?)rect.Attribute("fill")) == "#ffffff");
        Assert.Contains("4 type nodes and 4 typed relationships", document.Root.Element(Svg + "desc")!.Value);
        var text = string.Join(" ", document.Descendants(Svg + "text").Select(element => element.Value));
        Assert.Contains(document.Descendants(Svg + "title"), title => title.Value.Contains("Fixture.B", StringComparison.Ordinal));
        Assert.Contains(document.Descendants(Svg + "a"), link =>
            ((string?)link.Attribute("href"))?.Contains(
                "/blob/0123456789abcdef0123456789abcdef01234567/Fixture/A.cs#L1",
                StringComparison.Ordinal) == true);
        Assert.Contains(document.Descendants(Svg + "title"), title => title.Value.Contains("global::System.IO.Stream", StringComparison.Ordinal));
        Assert.Contains(document.Descendants(Svg + "text"), label => label.Value == "Assembly: System.Runtime");
        Assert.Contains("association", text);
        Assert.Contains("dependency", text);
        Assert.Contains("declared members · see type page", text);

        var ids = document.Descendants().Attributes("id").Select(attribute => attribute.Value).ToArray();
        Assert.Equal(ids.Length, ids.Distinct(StringComparer.Ordinal).Count());
        Assert.Contains(document.Descendants(Svg + "line"), line =>
            ((string?)line.Attribute("marker-end")) == "url(#uml-arrow)");

        var focusRect = nodeGroupsForSelfEdge(document, "type:Fixture:A");
        var focusCardX = double.Parse((string)focusRect.Attribute("x")!, System.Globalization.CultureInfo.InvariantCulture);
        var focusCardWidth = double.Parse((string)focusRect.Attribute("width")!, System.Globalization.CultureInfo.InvariantCulture);
        var selfLoop = Assert.Single(document.Descendants(Svg + "path"), path =>
            ((string?)path.Attribute("d"))?.StartsWith("M ", StringComparison.Ordinal) == true &&
            ((string?)path.Attribute("marker-end")) == "url(#uml-arrow)" &&
            ((string?)path.Attribute("d"))?.Contains(" C ", StringComparison.Ordinal) == true);
        var pathData = (string)selfLoop.Attribute("d")!;
        var coordinates = System.Text.RegularExpressions.Regex.Matches(pathData, @"-?\d+(?:\.\d+)?")
            .Select(match => double.Parse(match.Value, System.Globalization.CultureInfo.InvariantCulture)).ToArray();
        Assert.Equal(focusCardX + focusCardWidth, coordinates[0], precision: 2);
        Assert.Equal(focusCardX + focusCardWidth, coordinates[^2], precision: 2);
        Assert.True(coordinates[4] > focusCardX + focusCardWidth,
            "the self-reference loop must route outside the focus card rather than through its interior");

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

    private static XElement nodeGroupsForSelfEdge(XDocument document, string typeId) =>
        document.Descendants(Svg + "g")
            .Where(group => ((string?)group.Attribute("id"))?.StartsWith("uml-node-", StringComparison.Ordinal) == true)
            .Single(group => ((string?)group.Attribute("data-type-id")) == typeId)
            .Element(Svg + "rect")!;

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

    [Fact]
    public void Routes_self_reference_from_the_focus_card_boundary()
    {
        var source = new UmlSourceDeclaration("Fixture/Self.cs", new UmlSourceSpan(0, 20, 1, 1, 1, 21));
        var self = new UmlDocumentedType(
            "type:Fixture:Self",
            new UmlTypeIdentity("Fixture", "Fixture", [], "Self", 0, "class"),
            [source],
            []);
        var model = new UmlDocumentationModel(
            "1.0",
            new UmlModelProvenance("TrentBest/TheSingularityWorkshop.GUI", "0123456789abcdef0123456789abcdef01234567", "Fixture/Fixture.csproj", "Fixture", "net8.0", "Release", "project-semantic", null),
            [self],
            [new UmlDocumentedRelationship(
                "edge:self:self",
                UmlRelationshipKind.Association,
                self.Id,
                self.Id,
                "global::Fixture.Self",
                UmlRelationshipResolution.Resolved,
                null,
                "source-to-target",
                "self reference",
                null,
                null,
                new UmlRelationshipProvenance("roslyn-member-symbol", source.Path, source.Span, "Self.Self"))]);

        var document = XDocument.Parse(new SvgUmlDocumentationGraphRenderer().Render(model, self.Id));
        var node = document.Descendants(Svg + "g")
            .Single(group => (string?)group.Attribute("data-type-id") == self.Id &&
                group.Attribute("id") is not null);
        var rect = node.Element(Svg + "rect")!;
        var right = double.Parse((string)rect.Attribute("x")!, System.Globalization.CultureInfo.InvariantCulture) +
            double.Parse((string)rect.Attribute("width")!, System.Globalization.CultureInfo.InvariantCulture);
        var loop = document.Descendants(Svg + "path")
            .Single(path => ((string?)path.Attribute("d"))?.Contains(" C ", StringComparison.Ordinal) == true);
        var coordinates = System.Text.RegularExpressions.Regex.Matches(
            (string)loop.Attribute("d")!, @"-?\d+(?:\.\d+)?")
            .Select(match => double.Parse(match.Value, System.Globalization.CultureInfo.InvariantCulture))
            .ToArray();

        Assert.Equal(right, coordinates[0], 2);
        Assert.Equal(right, coordinates[^2], 2);
        Assert.Contains(document.Descendants(Svg + "text"), label => label.Value == "self-reference");
    }

}
