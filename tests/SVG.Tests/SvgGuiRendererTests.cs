using System.Xml.Linq;
using TheSingularityWorkshop.GUI.SVG;
using TheSingularityWorkshop.Workshop.Gui;
using Xunit;

namespace TheSingularityWorkshop.GUI.SVG.Tests;

public sealed class SvgGuiRendererTests
{
    [Fact]
    public void Render_WritesAccessibleSvgAndEncodesText()
    {
        var tree = new GuiNode("Text", "heading", "Hello & welcome <friend>");
        var svg = new SvgGuiRenderer().Render(tree, new SvgRenderOptions { Title = "Greeting", Description = "A greeting", Width = 320, Height = 180 });
        Assert.Contains("<svg", svg);
        Assert.Contains("viewBox=\"0 0 320 180\"", svg);
        Assert.Contains("aria-labelledby=\"svg-title\"", svg);
        Assert.Contains("<title id=\"svg-title\">Greeting</title>", svg);
        Assert.Contains("<desc id=\"svg-description\">A greeting</desc>", svg);
        Assert.Contains("Hello &amp; welcome &lt;friend&gt;", svg);
    }

    [Fact]
    public void Render_EncodesAttributeTextAndRendersNestedChildrenInOrder()
    {
        var tree = new GuiNode("Panel", "root", "Root", children: new[]
        {
            new GuiNode("Text", "first", "First"),
            new GuiNode("Button", "save", "Save \"now\" & later")
        });

        var svg = new SvgGuiRenderer().Render(tree, new SvgRenderOptions { Title = "Root \"panel\"" });

        Assert.Contains("Root &quot;panel&quot;", svg);
        Assert.Contains("aria-label=\"Button: Save &quot;now&quot; &amp; later\"", svg);
        Assert.True(svg.IndexOf("First", StringComparison.Ordinal) < svg.IndexOf("Save &quot;now&quot;", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData(0, 100)]
    [InlineData(100, 0)]
    [InlineData(-1, 10)]
    public void Render_RejectsNonPositiveDimensions(int width, int height)
    {
        var tree = new GuiNode("Text", "heading", "Hello");
        Assert.Throws<ArgumentOutOfRangeException>(() => new SvgGuiRenderer().Render(tree, new SvgRenderOptions { Width = width, Height = height }));
    }

    [Theory]
    [InlineData("Panel")]
    [InlineData("Stack")]
    [InlineData("Row")]
    [InlineData("Column")]
    [InlineData("Object")]
    [InlineData("Image")]
    [InlineData("TextBox")]
    [InlineData("IntegerSlider")]
    [InlineData("FloatSlider")]
    [InlineData("Toggle")]
    [InlineData("Warning")]
    [InlineData("Separator")]
    [InlineData("Button")]
    [InlineData("Text")]
    public void Render_ProducesSvgForEveryCanonicalKind(string kind)
    {
        var node = new GuiNode(kind, "sample", "Sample node", source: "image.png",
            properties: new Dictionary<string, string> { ["Value"] = "true" });
        var svg = new SvgGuiRenderer().Render(node);
        Assert.StartsWith("<svg ", svg);
        Assert.EndsWith("</svg>", svg);
        Assert.Contains("Sample node", svg);
        var document = XDocument.Parse(svg);
        Assert.Equal("svg", document.Root!.Name.LocalName);
    }

    [Fact]
    public void Render_RejectsUnsafeBackgroundColor()
    {
        var tree = new GuiNode("Text", "heading", "Hello");
        Assert.Throws<ArgumentException>(() => new SvgGuiRenderer().Render(tree,
            new SvgRenderOptions { Background = "url(javascript:alert(1))" }));
    }

    [Fact]
    public void Render_RejectsNullRoot() => Assert.Throws<ArgumentNullException>(() => new SvgGuiRenderer().Render(null!));

    [Fact]
    public void Render_RejectsTreesDeeperThanTheDocumentedLimit()
    {
        GuiNode tree = new("Text", "leaf", "Leaf");
        for (var i = 0; i < 65; i++) tree = new GuiNode("Panel", "node-" + i, children: new[] { tree });
        Assert.Throws<InvalidOperationException>(() => new SvgGuiRenderer().Render(tree));
    }
}