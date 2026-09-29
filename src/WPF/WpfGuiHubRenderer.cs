using System.Windows;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.WPF;

/// <summary>
/// WPF manifestation adapter for the platform-neutral GUI Hub.
/// </summary>
public static class WpfGuiHubRenderer
{
    public static FrameworkElement Render(IGuiHub hub)
    {
        ArgumentNullException.ThrowIfNull(hub);
        return WpfGuiRenderer.Render(hub.Root);
    }
}
