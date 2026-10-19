using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace TheSingularityWorkshop.Gui;

/// <summary>Provides the WorkshopGui API.</summary>
public static class WorkshopGui
{
    /// <summary>Creates a builder for an HTML element.</summary>
    public static ElementBuilder Element(object receiver,string tagName)=>new(receiver,tagName);
    /// <summary>Creates a builder for a panel.</summary>
    public static PanelGuiBuilder Panel(object receiver)=>new(receiver);
    /// <summary>Creates a builder for a multi-panel layout.</summary>
    public static MultiPanelGuiBuilder MultiPanel(object receiver)=>new(receiver);
    /// <summary>Creates a button node or button builder.</summary>
    public static ButtonGuiBuilder Button(object receiver)=>new(receiver);
    /// <summary>Sets an image source reference.</summary>
    public static ElementBuilder Image(object receiver)=>Element(receiver,"img");
}

/// <summary>Provides the ElementBuilder API.</summary>
public class ElementBuilder
{
    private readonly object _receiver;
    private readonly string _tagName;
    private readonly RenderFragment? _rawFragment;
    private readonly List<(string Name,object? Value)> _attributes=new();
    private readonly List<RenderFragment> _children=new();
    private readonly Dictionary<string,string> _styles=new(StringComparer.Ordinal);
    internal ElementBuilder(object receiver,string tagName){_receiver=receiver;_tagName=tagName;}
    private ElementBuilder(RenderFragment rawFragment){_receiver=new object();_tagName=string.Empty;_rawFragment=rawFragment;}
    /// <summary>Wraps a Blazor render fragment in an element builder.</summary>
    public static implicit operator ElementBuilder(RenderFragment fragment){ArgumentNullException.ThrowIfNull(fragment);return new ElementBuilder(fragment);}
    /// <summary>Sets an HTML attribute.</summary>
    public ElementBuilder Attribute(string name,object? value){_attributes.Add((name,value));return this;}
    /// <summary>Sets the accessible label.</summary>
    public ElementBuilder AriaLabel(string value)=>Attribute("aria-label",value);
    /// <summary>Sets the element title.</summary>
    public ElementBuilder Title(string value)=>Attribute("title",value);
    /// <summary>Sets the element's keyboard tab index.</summary>
    public ElementBuilder TabIndex(int value)=>Attribute("tabindex",value);
    /// <summary>Sets the element's CSS class.</summary>
    public ElementBuilder Class(string className)=>Attribute("class",className);
    /// <summary>Sets a CSS style property.</summary>
    public ElementBuilder Style(string name,string value){_styles[name]=value;return this;}
    /// <summary>Sets absolute positioning using normalized coordinates.</summary>
    public ElementBuilder Position(double x,double y)=>Style("position","absolute").Style("left",$"{x:0.##}%").Style("top",$"{y:0.##}%").Style("transform","translate(-50%, -50%)");
    /// <summary>Sets or adds text content.</summary>
    public ElementBuilder Text(string text){_children.Add(b=>b.AddContent(0,text));return this;}
    /// <summary>Adds a child element or render fragment.</summary>
    public ElementBuilder Child(ElementBuilder child){_children.Add(child.Build());return this;}
    /// <summary>Adds a child element or render fragment.</summary>
    public ElementBuilder Child(RenderFragment child){_children.Add(child);return this;}
    /// <summary>Adds an element builder as content.</summary>
    public ElementBuilder Content(ElementBuilder child)=>Child(child);
    /// <summary>Registers a click handler.</summary>
    public ElementBuilder OnClick(Action action)=>Attribute("onclick",EventCallback.Factory.Create(_receiver,action));
    /// <summary>Registers a click handler.</summary>
    public ElementBuilder OnClick(Func<Task> action)=>Attribute("onclick",EventCallback.Factory.Create(_receiver,action));
    /// <summary>Registers a click handler.</summary>
    public ElementBuilder OnClick(Action<MouseEventArgs> action)=>Attribute("onclick",EventCallback.Factory.Create<MouseEventArgs>(_receiver,action));
    /// <summary>Registers a keyboard event handler.</summary>
    public ElementBuilder OnKeyDown(Action<KeyboardEventArgs> action)=>Attribute("onkeydown",EventCallback.Factory.Create(_receiver,action));
    /// <summary>Registers a mouse-enter handler.</summary>
    public ElementBuilder OnMouseEnter(Action action)=>Attribute("onmouseenter",EventCallback.Factory.Create(_receiver,action));
    /// <summary>Registers a mouse-leave handler.</summary>
    public ElementBuilder OnMouseLeave(Action action)=>Attribute("onmouseleave",EventCallback.Factory.Create(_receiver,action));
    /// <summary>Registers a mouse-wheel handler.</summary>
    public ElementBuilder OnWheel(Action<WheelEventArgs> action)=>Attribute("onwheel",EventCallback.Factory.Create<WheelEventArgs>(_receiver,action));
    /// <summary>Requests event propagation to stop for the named event.</summary>
    public ElementBuilder StopPropagation(string eventName = "onclick")=>Attribute($"{eventName}:stopPropagation",true);
    /// <summary>Requests the browser's default action be prevented for the named event.</summary>
    public ElementBuilder PreventDefault(string eventName)=>Attribute($"{eventName}:preventDefault",true);
    /// <summary>Adds multiple child builders in order.</summary>
    public ElementBuilder BuildChildren(params ElementBuilder[] children){foreach(var child in children)Child(child);return this;}
    /// <summary>Builds and returns the configured GUI node or render fragment.</summary>
    public RenderFragment Build()
    {
        if(_rawFragment is not null)return _rawFragment;
        return builder=>{builder.OpenElement(0,_tagName);var sequence=1;foreach(var a in _attributes)builder.AddAttribute(sequence++,a.Name,a.Value);if(_styles.Count>0)builder.AddAttribute(sequence++,"style",string.Join(";",_styles.Select(x=>$"{x.Key}:{x.Value}")));foreach(var child in _children)builder.AddContent(sequence++,child);builder.CloseElement();};
    }
}