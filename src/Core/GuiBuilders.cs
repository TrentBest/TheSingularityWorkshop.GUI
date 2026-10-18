using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Convenience factories for composing semantic GUI trees without platform dependencies.
/// </summary>
public static class GuiBuilders
{
    public static GuiNodeBuilder Button(string id, string text) => new("Button", id).Text(text);
    public static GuiNodeBuilder Column(string id) => new("Column", id);
    public static GuiNodeBuilder Text(string id, string text) => new("Text", id).Text(text);
    public static GuiNodeBuilder Panel(string id) => new("Panel", id);
    public static GuiNodeBuilder Warning(string id, string text) => new("Warning", id).Text(text);
}

/// <summary>
/// Fluent builder for an immutable, platform-neutral <see cref="GuiNode"/> tree.
/// Property values remain strings so platform adapters own parsing and manifestation.
/// </summary>
public sealed class GuiNodeBuilder
{
    private readonly string _kind;
    private readonly string _id;
    private string? _text;
    private string? _source;
    private readonly Dictionary<string, string> _properties = new(StringComparer.Ordinal);
    private readonly List<GuiNodeBuilder> _children = new();

    internal GuiNodeBuilder(string kind, string id)
    {
        _kind = kind;
        _id = id;
    }

    public GuiNodeBuilder Text(string text) { _text = text; return this; }
    public GuiNodeBuilder Image(string source) { _source = source; return this; }

    public GuiNodeBuilder Property(string name, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(value);
        _properties[name] = value;
        return this;
    }

    public GuiNodeBuilder Child(GuiNodeBuilder child)
    {
        ArgumentNullException.ThrowIfNull(child);
        _children.Add(child);
        return this;
    }

    public GuiNodeBuilder Child(GuiNode child)
    {
        ArgumentNullException.ThrowIfNull(child);
        _children.Add(FromNode(child));
        return this;
    }

    public GuiNode Build() => new(
        _kind, _id, _text, _source, _properties,
        _children.ConvertAll(child => child.Build()));

    private static GuiNodeBuilder FromNode(GuiNode node)
    {
        var builder = new GuiNodeBuilder(node.Kind, node.Id)
        {
            _text = node.Text,
            _source = node.Source
        };
        foreach (var property in node.Properties)
            builder._properties.Add(property.Key, property.Value);
        foreach (var child in node.Children)
            builder._children.Add(FromNode(child));
        return builder;
    }
}
