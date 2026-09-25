namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// The minimal intrinsic GUI hub boundary.
///
/// The hub has identity, but no domain-specific purpose. Consuming projects
/// may add purpose through their own interfaces and capabilities.
/// </summary>
public interface IGuiHub
{
    /// <summary>
    /// Gets the stable identity of this hub instance.
    /// </summary>
    Guid Id { get; }
}
