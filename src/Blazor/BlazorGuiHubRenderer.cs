using Microsoft.AspNetCore.Components;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Blazor manifestation adapter for the platform-neutral GUI Hub.
/// </summary>
public static class BlazorGuiHubRenderer
{
    public static RenderFragment Render(IGuiHub hub)
    {
        ArgumentNullException.ThrowIfNull(hub);
        return BlazorGuiRenderer.Render(hub.Root);
    }
}
