using System.Xml.Linq;
using TheSingularityWorkshop.Workshop.Gui;
using TheSingularityWorkshop.Workshop.Gui.Svg;
using Xunit;

namespace TheSingularityWorkshop.GUI.SVG.Tests;

/// <summary>
/// Makes the SVG adapter's generic fallback explicit: it preserves identity and
/// descendants, but does not pretend an unknown widget kind has native SVG semantics.
/// </summary>
public sealed class SvgGuiRendererFallbackContractTests
{
    [Fact]
    public void Unknown_kind_becomes_group_with_title_and_ordered_children()
    {
        var tree = GuiBuilder.Create("UnknownWidget", "custom-widget")
            .Text("Widget label")
            .Child("svg-circle", "first", circle => circle
                .Property("cx", "10").Property("cy", "10").Property("r", "2"))
            .Child("svg-rect", "second", rect => rect
                .Property("x", "20").Property("y", "20")
                .Property("width", "5").Property("height", "5"))
            .Build();

        var document = XDocument.Parse(new SvgGuiRenderer().Render(tree));
        XNamespace svg = "http://www.w3.org/2000/svg";

        var fallback = document.Descendants(svg + "g")
            .Single(element => (string?)element.Attribute("id") == "custom-widget");

        Assert.Equal("Widget label", fallback.Element(svg + "title")?.Value);
        Assert.Equal(
            new[] { "first", "second" },
            fallback.Elements().Where(element => element.Name != svg + "title")
                .Select(element => (string?)element.Attribute("id")));
        Assert.Equal("circle", fallback.Elements().First(element => (string?)element.Attribute("id") == "first").Name.LocalName);
        Assert.Equal("rect", fallback.Elements().First(element => (string?)element.Attribute("id") == "second").Name.LocalName);
    }
}
