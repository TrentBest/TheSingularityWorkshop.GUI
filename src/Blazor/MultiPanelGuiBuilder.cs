namespace TheSingularityWorkshop.Workshop.Gui;

public sealed class MultiPanelGuiBuilder : ElementBuilder
{
    internal MultiPanelGuiBuilder(object receiver) : base(receiver, "div")
    {
    }

    public MultiPanelGuiBuilder Panel(Action<PanelGuiBuilder> configure)
    {
        var panel = WorkshopGui.Panel(this);
        configure(panel);
        Child(panel);
        return this;
    }

    public new MultiPanelGuiBuilder Content(ElementBuilder child)
    {
        Child(child);
        return this;
    }

    public new MultiPanelGuiBuilder Style(string name, string value)
    {
        base.Style(name, value);
        return this;
    }
}
