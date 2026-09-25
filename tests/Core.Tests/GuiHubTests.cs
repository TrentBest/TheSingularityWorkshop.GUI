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
    public void Hub_is_only_an_intrinsic_boundary()
    {
        Assert.IsAssignableFrom<IGuiHub>(Gui.Hub);
        Assert.IsType<GuiHub>(Gui.Hub);
    }
}
