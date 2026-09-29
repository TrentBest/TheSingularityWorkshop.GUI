namespace TheSingularityWorkshop.Workshop.Gui.Tests;

public sealed class GuiHubTests
{
    [Fact]
    public void Gui_exposes_a_stable_intrinsic_hub()
    {
        var first = Gui.Hub;
        var second = Gui.Hub;

        Assert.NotNull(first);
        Assert.Same(first, second);
        Assert.NotEqual(Guid.Empty, first.Id);
    }

    [Fact]
    public void Hub_defines_a_platform_neutral_root_surface()
    {
        var hub = Assert.IsType<GuiHub>(Gui.Hub);

        Assert.NotNull(hub.Root);
        Assert.Equal("Panel", hub.Root.Kind);
        Assert.Equal("gui-hub", hub.Root.Id);
        Assert.Equal("Hub", hub.Root.Properties["surface"]);
        Assert.Equal("application", hub.Root.Properties["role"]);
        Assert.Contains(hub.Root.Children, x => x.Id == "gui-hub-header");
        Assert.Contains(hub.Root.Children, x => x.Id == "gui-hub-content");
    }
}
