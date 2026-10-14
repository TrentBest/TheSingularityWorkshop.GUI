using TheSingularityWorkshop.GUI.SVG;
using TheSingularityWorkshop.Workshop.Gui;
using Xunit;

namespace TheSingularityWorkshop.GUI.SVG.Tests;

public sealed class SvgGuiRendererTests
{
    [Fact]
    public void Render_WritesAccessibleSvgAndEncodesText()
    {
        var tree = new GuiNode("Text", "heading", "Hello & welcome");
        var svg = new SvgGuiRenderer().Render(tree, new SvgRenderOptions { Title = "Greeting", Description = "A greeting", Width = 320, Height = 180 });
        Assert.Contains("<svg", svg);
        Assert.Contains("viewBox=\"0 0 320 180\"", svg);
        Assert.Contains("<title id=\"svg-title\">Greeting</title>", svg);
        Assert.Contains("<desc id=\"svg-description\">A greeting</desc>", svg);
        Assert.Contains("Hello &amp; welcome", svg);
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

    [Fact]
    public void Render_RejectsNullRoot() => Assert.Throws<ArgumentNullException>(() => new SvgGuiRenderer().Render(null!));
}
