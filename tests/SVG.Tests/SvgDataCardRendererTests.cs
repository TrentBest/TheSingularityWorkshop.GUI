using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;
using Xunit;

namespace TheSingularityWorkshop.GUI.SVG.Tests;

public sealed class SvgDataCardRendererTests
{
    private static readonly XNamespace Svg = "http://www.w3.org/2000/svg";

    [Fact]
    public void Collapsed_card_shows_model_derived_direct_child_count_and_hides_children()
    {
        var card = new DataCard("namespace", "namespace", "Workshop",
            isExpandedByDefault: false,
            children:
            [
                new DataCard("class", "type", "ExampleClass"),
                new DataCard("struct", "type", "ExampleStruct"),
                new DataCard("interface", "type", "IExample"),
                new DataCard("enum", "type", "ExampleKind")
            ]);

        var document = XDocument.Parse(new SvgDataCardRenderer().Render(card));
        var texts = document.Descendants(Svg + "text").Select(element => element.Value).ToArray();

        Assert.Contains(texts, text => text.Contains("Workshop  ·  Collapsed (4)", StringComparison.Ordinal));
        Assert.DoesNotContain(texts, text => text == "ExampleClass");
        Assert.DoesNotContain(texts, text => text == "ExampleStruct");
    }

    [Fact]
    public void Expanded_cards_render_nested_containment_and_subtitles_without_overlap()
    {
        var card = new DataCard("namespace", "namespace", "Workshop",
            subtitle: "Namespace",
            children:
            [
                new DataCard("type", "type", "ExampleClass",
                    subtitle: "public sealed class ExampleClass")
            ]);

        var document = XDocument.Parse(new SvgDataCardRenderer().Render(card));
        var rootGroup = document.Descendants(Svg + "g")
            .Single(element => (string?)element.Attribute("data-card-id") == "namespace");
        var childGroup = rootGroup.Descendants(Svg + "g")
            .Single(element => (string?)element.Attribute("data-card-id") == "type");

        Assert.Equal("Namespace", rootGroup.Elements(Svg + "text").ElementAt(1).Value);
        Assert.Equal("public sealed class ExampleClass", childGroup.Elements(Svg + "text").ElementAt(1).Value);

        var rootRect = rootGroup.Element(Svg + "rect")!;
        var childRect = childGroup.Element(Svg + "rect")!;
        Assert.True(double.Parse((string)childRect.Attribute("y")!, System.Globalization.CultureInfo.InvariantCulture) >
                    double.Parse((string)rootRect.Attribute("y")!, System.Globalization.CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Uml_card_palette_uses_namespace_blue_class_orange_and_property_yellow_borders()
    {
        var root = new DataCard("namespace", "namespace", "Example",
            children:
            [
                new DataCard("class", "class", "SampleClass",
                    children:
                    [
                        new DataCard("property", "property", "Name")
                    ])
            ]);

        var document = XDocument.Parse(new SvgDataCardRenderer().Render(root));
        var cards = document.Descendants(Svg + "g")
            .ToDictionary(element => (string)element.Attribute("data-card-id")!);

        Assert.Equal("#4f46e5", (string)cards["namespace"].Element(Svg + "rect")!.Attribute("stroke")!);
        Assert.Equal("4", (string)cards["namespace"].Element(Svg + "rect")!.Attribute("stroke-width")!);
        Assert.Equal("#ff823d", (string)cards["class"].Element(Svg + "rect")!.Attribute("stroke")!);
        Assert.Equal("#f2e83d", (string)cards["property"].Element(Svg + "rect")!.Attribute("stroke")!);
    }

    [Fact]
    public void Uml_card_palette_uses_light_tints_and_dark_text()
    {
        var root = new DataCard("namespace", "namespace", "Example",
            children:
            [
                new DataCard("class", "class", "SampleClass",
                    children: [new DataCard("property", "property", "Name")])
            ]);

        var document = XDocument.Parse(new SvgDataCardRenderer().Render(root));
        var cards = document.Descendants(Svg + "g")
            .ToDictionary(element => (string)element.Attribute("data-card-id")!);

        var namespaceRect = cards["namespace"].Element(Svg + "rect")!;
        var classRect = cards["class"].Element(Svg + "rect")!;
        var propertyRect = cards["property"].Element(Svg + "rect")!;

        Assert.Equal("#eef0ff", (string)namespaceRect.Attribute("fill")!);
        Assert.Equal("#fff0e5", (string)classRect.Attribute("fill")!);
        Assert.Equal("#fffde0", (string)propertyRect.Attribute("fill")!);
        Assert.Equal("#20205c", (string)cards["namespace"].Element(Svg + "text")!.Attribute("fill")!);
        Assert.Equal("#6b2e0c", (string)cards["class"].Element(Svg + "text")!.Attribute("fill")!);
        Assert.Equal("#514d00", (string)cards["property"].Element(Svg + "text")!.Attribute("fill")!);
    }

    [Fact]
    public void Empty_region_has_no_collapsed_summary_and_add_affordance_is_edit_and_capability_gated()
    {
        var card = new DataCard("empty", "region", "Empty region",
            isExpandedByDefault: false,
            canRequestChildInsertion: true);
        var renderer = new SvgDataCardRenderer();

        var viewDocument = XDocument.Parse(renderer.Render(card, editMode: false));
        var inspectDocument = XDocument.Parse(renderer.Render(card, editMode: false));
        var editDocument = XDocument.Parse(renderer.Render(card, editMode: true));

        Assert.DoesNotContain(viewDocument.Descendants(Svg + "text"), element => element.Value == "+");
        Assert.DoesNotContain(inspectDocument.Descendants(Svg + "text"), element => element.Value == "+");
        Assert.Contains(editDocument.Descendants(Svg + "text"), element => element.Value == "+");
        Assert.DoesNotContain(viewDocument.Descendants(Svg + "text"),
            element => element.Value.Contains("Collapsed", StringComparison.Ordinal));
    }

    [Fact]
    public void Edit_mode_does_not_show_add_affordance_without_model_capability()
    {
        var card = new DataCard("read-only", "region", "Read-only region",
            canRequestChildInsertion: false);

        var document = XDocument.Parse(new SvgDataCardRenderer().Render(card, editMode: true));

        Assert.DoesNotContain(document.Descendants(Svg + "text"), element => element.Value == "+");
    }
}
