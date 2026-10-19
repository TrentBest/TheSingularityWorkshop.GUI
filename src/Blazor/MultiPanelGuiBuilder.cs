namespace TheSingularityWorkshop.Gui;
/// <summary>Provides the MultiPanelGuiBuilder API.</summary>
public sealed class MultiPanelGuiBuilder:ElementBuilder
{
    internal MultiPanelGuiBuilder(object receiver):base(receiver,"div"){}
    /// <summary>Creates a builder for a panel.</summary>
    public MultiPanelGuiBuilder Panel(Action<PanelGuiBuilder> configure){var panel=WorkshopGui.Panel(this);configure(panel);Child(panel);return this;}
    /// <summary>Adds an element builder as content.</summary>
    public new MultiPanelGuiBuilder Content(ElementBuilder child){Child(child);return this;}
    /// <summary>Sets a CSS style property.</summary>
    public new MultiPanelGuiBuilder Style(string name,string value){base.Style(name,value);return this;}
}