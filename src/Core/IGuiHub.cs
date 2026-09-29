using System;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Platform-neutral boundary for the default GUI hub.
/// </summary>
public interface IGuiHub
{
    /// <summary>Gets the stable identity of this hub instance.</summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the semantic root surface. It contains no renderer-specific types.
    /// </summary>
    GuiNode Root { get; }
}
