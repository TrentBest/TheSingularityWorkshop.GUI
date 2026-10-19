using Microsoft.AspNetCore.Components.Web;
namespace TheSingularityWorkshop.Gui;
/// <summary>Provides the ButtonGuiBuilder API.</summary>
public sealed class ButtonGuiBuilder:ElementBuilder
{
    internal ButtonGuiBuilder(object receiver):base(receiver,"button"){Attribute("type","button");}
    /// <summary>Sets the button label.</summary>
    public ButtonGuiBuilder Label(string text){Text(text);return this;}
    /// <summary>Positions the button at normalized coordinates.</summary>
    public ButtonGuiBuilder PositionAt(double x,double y){Position(x,y);return this;}
    /// <summary>Sets an HTML attribute.</summary>
    public new ButtonGuiBuilder Attribute(string name,object? value){base.Attribute(name,value);return this;}
    /// <summary>Sets the accessible label.</summary>
    public new ButtonGuiBuilder AriaLabel(string value){base.AriaLabel(value);return this;}
    /// <summary>Sets the element title.</summary>
    public new ButtonGuiBuilder Title(string value){base.Title(value);return this;}
    /// <summary>Sets a CSS style property.</summary>
    public new ButtonGuiBuilder Style(string name,string value){base.Style(name,value);return this;}
    /// <summary>Registers a click handler.</summary>
    public new ButtonGuiBuilder OnClick(Action action){base.OnClick(action);return this;}
    /// <summary>Registers a click handler.</summary>
    public new ButtonGuiBuilder OnClick(Func<Task> action){base.OnClick(action);return this;}
    /// <summary>Registers a click handler.</summary>
    public new ButtonGuiBuilder OnClick(Action<MouseEventArgs> action){base.OnClick(action);return this;}
}