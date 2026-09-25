namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// The intrinsic GUI hub object.
///
/// This object is deliberately small. It is a composition boundary and
/// identity holder, not a renderer and not a collection of domain purposes.
/// </summary>
public sealed class GuiHub : IGuiHub
{
    public GuiHub()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }
}
