using System;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Entry point for intrinsic GUI concepts.
///
/// Accessing <see cref="Hub"/> does not render or manifest anything. It only
/// provides the GUI's fundamental hub boundary.
/// </summary>
public static class Gui
{
    private static readonly Lazy<IGuiHub> HubInstance = new(
        static () => new GuiHub());

    /// <summary>
    /// Gets the intrinsic GUI hub. The instance is created only when accessed.
    /// </summary>
    public static IGuiHub Hub => HubInstance.Value;
}
