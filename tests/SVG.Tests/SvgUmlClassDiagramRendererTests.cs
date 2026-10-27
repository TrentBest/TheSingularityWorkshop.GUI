using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;
using Xunit;

namespace TheSingularityWorkshop.GUI.SVG.Tests;

public sealed class SvgUmlClassDiagramRendererTests
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

    [Fact]
    public void Renders_nested_cards_in_category_and_visibility_order_with_accessible_description()
    {
        var model = new UmlClassDiagram(
            "DataCard",
            kind: "sealed class",
            @namespace: "TheSingularityWorkshop.Workshop.Gui",
            fields:
            [
                new UmlMember("_children", "_children : ReadOnlyCollection<DataCard>", "private"),
                new UmlMember("Cache", "Cache : int", "internal", IsStatic: true)
            ],
            properties: [new UmlMember("Id", "Id : string {get}", "public")],
            operations:
            [
                new UmlMember("GetCollapsedSummary", "GetCollapsedSummary(isExpanded : bool) : string?", "public"),
                new UmlMember("Validate", "Validate() : void", "protected"),
                new UmlMember("Reset", "Reset() : void", "private")
            ],
            declaredRelationships: ["DataCard → DataCard : Children (source-declared association candidate)"]);

        var document = XDocument.Parse(new SvgUmlClassDiagramRenderer().Render(model));
        Assert.Equal("UML class diagram: DataCard", document.Root!.Element(Svg + "title")!.Value);
        var description = document.Root.Element(Svg + "desc")!.Value;
        Assert.Contains("Static nested-card view", description);
        Assert.Contains("Public, Protected, Private, then Internal / other order", description);
        Assert.Contains("source-declared association candidate", description);

        var text = string.Join(" ", document.Descendants(Svg + "text").Select(element => element.Value));
        Assert.Contains("Properties (1)", text);
        Assert.Contains("Methods (3)", text);
        Assert.Contains("Fields (2)", text);
        Assert.Contains("Public (1)", text);
        Assert.Contains("Protected (1)", text);
        Assert.Contains("Private (1)", text);
        Assert.Contains("Internal / other (1)", text);
        Assert.Contains("GetCollapsedSummary", text);
        Assert.Contains("Children", text);
        Assert.Contains("TheSingularityWorkshop.Workshop.Gui", text);

        var cards = document.Descendants(Svg + "rect").ToDictionary(
            element => (string)element.Attribute("data-card-kind")!,
            StringComparer.Ordinal);
        Assert.True(Contains(cards["namespace-container"], cards["type-container"]));
        Assert.True(Contains(cards["type-container"], cards["type-header"]));
        Assert.True(Contains(cards["type-container"], cards["member-category property"]));
        Assert.True(Contains(cards["type-container"], cards["member-category method"]));
        Assert.True(Contains(cards["type-container"], cards["member-category field"]));

        var categories = document.Descendants(Svg + "rect")
            .Where(element => ((string?)element.Attribute("data-card-kind"))?.StartsWith("member-category", StringComparison.Ordinal) == true)
            .Select(element => (string)element.Attribute("data-card-kind")!)
            .ToArray();
        Assert.Equal(new[] { "member-category property", "member-category method", "member-category field" }, categories);

        var strokes = document.Descendants(Svg + "rect").Select(element => (string?)element.Attribute("stroke")).ToArray();
        Assert.Contains("#d6b82e", strokes);
        Assert.Contains("#2e9d70", strokes);
        Assert.Contains("#4a90e2", strokes);
        Assert.Contains("#8b7ac8", strokes);
    }

    private static bool Contains(XElement outer, XElement inner)
    {
        static double Number(XElement element, string attribute) =>
            double.Parse((string)element.Attribute(attribute)!, System.Globalization.CultureInfo.InvariantCulture);

        var outerX = Number(outer, "x");
        var outerY = Number(outer, "y");
        var outerRight = outerX + Number(outer, "width");
        var outerBottom = outerY + Number(outer, "height");
        var innerX = Number(inner, "x");
        var innerY = Number(inner, "y");
        var innerRight = innerX + Number(inner, "width");
        var innerBottom = innerY + Number(inner, "height");

        return innerX >= outerX && innerY >= outerY &&
            innerRight <= outerRight && innerBottom <= outerBottom;
    }

    [Fact]
    public void Rejects_too_narrow_diagram_width()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new SvgUmlClassDiagramRenderer().Render(new UmlClassDiagram("Example"), width: 200));
    }
}
