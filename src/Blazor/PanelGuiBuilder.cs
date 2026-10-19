namespace TheSingularityWorkshop.Gui;
/// <summary>Provides the PanelGuiBuilder API.</summary>
public sealed class PanelGuiBuilder:ElementBuilder
{
    internal PanelGuiBuilder(object receiver):base(receiver,"section"){Style("display","flex");Style("flex-direction","column");Style("box-sizing","border-box");}
    /// <summary>Adds a heading with the requested level.</summary>
    public PanelGuiBuilder Heading(string text,int level=2){Child(WorkshopGui.Element(this,$"h{Math.Clamp(level,1,6)}").Text(text));return this;}
    /// <summary>Adds a paragraph of text.</summary>
    public PanelGuiBuilder Paragraph(string text){Child(WorkshopGui.Element(this,"p").Text(text));return this;}
    /// <summary>Adds an element builder as content.</summary>
    public new PanelGuiBuilder Content(ElementBuilder child){Child(child);return this;}
    /// <summary>Sets an HTML attribute.</summary>
    public new PanelGuiBuilder Attribute(string name,object? value){base.Attribute(name,value);return this;}
    /// <summary>Sets the accessible label.</summary>
    public new PanelGuiBuilder AriaLabel(string value){base.AriaLabel(value);return this;}
    /// <summary>Sets a CSS style property.</summary>
    public new PanelGuiBuilder Style(string name,string value){base.Style(name,value);return this;}
}