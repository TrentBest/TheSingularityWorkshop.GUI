using System;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// The intrinsic GUI hub: the platform-neutral root surface through which a
/// host can present an assembled runtime.
/// </summary>
public sealed class GuiHub : IGuiHub
{
    public GuiHub()
    {
        Id = Guid.NewGuid();
        Root = GuiBuilder.Create("Panel", "gui-hub")
            .Property("surface", "Hub")
            .Property("role", "application")
            .Child("Panel", "gui-hub-header")
            .Child("Panel", "gui-hub-content")
            .Build();
    }

    public Guid Id { get; }

    /// <summary>
    /// Gets the platform-neutral semantic root for the default GUI surface.
    /// Platform adapters decide how this tree is manifested.
    /// </summary>
    public GuiNode Root { get; }
}
