#pragma warning disable BL0006

using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.Blazor.Tests;

public sealed class BlazorGuiHubRendererTests
{
    [Fact]
    public void Renderer_manifests_the_core_hub_root()
    {
        var hub = Gui.Hub;
        var fragment = BlazorGuiHubRenderer.Render(hub);
        var builder = new Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder();

        fragment(builder);

        var frames = builder.GetFrames().Array;
        Assert.Equal("div", frames[0].ElementName);
        Assert.Equal("gui-hub", FindAttribute(frames, "id", "gui-hub"));
        Assert.Equal("Hub", FindAttribute(frames, "surface", "Hub"));
    }

    private static string? FindAttribute(
        Microsoft.AspNetCore.Components.RenderTree.RenderTreeFrame[] frames,
        string name,
        string expectedValue)
    {
        foreach (var frame in frames)
        {
            if (frame.FrameType == Microsoft.AspNetCore.Components.RenderTree.RenderTreeFrameType.Attribute &&
                frame.AttributeName == name &&
                Equals(frame.AttributeValue, expectedValue))
            {
                return expectedValue;
            }
        }

        return null;
    }
}
