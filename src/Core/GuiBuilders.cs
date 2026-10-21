using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Convenience factories for composing semantic GUI trees without platform dependencies.
/// </summary>
public static class GuiBuilders
{
    /// <summary>Creates a button node or button builder.</summary>
    public static GuiNodeBuilder Button(string id, string text) => new GuiNodeBuilder("Button", id).Text(text);
    /// <summary>Creates a column node builder.</summary>
    public static GuiNodeBuilder Column(string id) => new GuiNodeBuilder("Column", id);
    /// <summary>Sets or adds text content.</summary>
    public static GuiNodeBuilder Text(string id, string text) => new GuiNodeBuilder("Text", id).Text(text);
    /// <summary>Creates a builder for a panel.</summary>
    public static GuiNodeBuilder Panel(string id) => new GuiNodeBuilder("Panel", id);
    /// <summary>Creates a warning node builder.</summary>
    public static GuiNodeBuilder Warning(string id, string text) => new GuiNodeBuilder("Warning", id).Text(text);
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

    /// <summary>Sets or adds text content.</summary>
    public GuiNodeBuilder Text(string text) { _text = text; return this; }
    /// <summary>Sets an image source reference.</summary>
    public GuiNodeBuilder Image(string source) { _source = source; return this; }

    /// <summary>Sets a string-valued node property.</summary>
    public GuiNodeBuilder Property(string name, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(value);
        _properties[name] = value;
        return this;
    }

    /// <summary>Adds a child element or render fragment.</summary>
    public GuiNodeBuilder Child(GuiNodeBuilder child)
    {
        ArgumentNullException.ThrowIfNull(child);
        _children.Add(child);
        return this;
    }

    /// <summary>Adds a child element or render fragment.</summary>
    public GuiNodeBuilder Child(GuiNode child)
    {
        ArgumentNullException.ThrowIfNull(child);
        _children.Add(FromNode(child));
        return this;
    }

    /// <summary>
    /// Builds the configured tree iteratively so deeply nested input does not consume
    /// the process call stack. A cyclic builder graph is rejected with a controlled error.
    /// </summary>
    public GuiNode Build()
    {
        var built = new Dictionary<GuiNodeBuilder, GuiNode>();
        var active = new HashSet<GuiNodeBuilder>();
        var pending = new Stack<(GuiNodeBuilder Builder, bool Expanded)>();
        pending.Push((this, false));

        while (pending.Count > 0)
        {
            var (builder, expanded) = pending.Pop();
            if (expanded)
            {
                var children = new List<GuiNode>(builder._children.Count);
                foreach (var child in builder._children)
                    children.Add(built[child]);

                built[builder] = new GuiNode(
                    builder._kind, builder._id, builder._text, builder._source,
                    builder._properties, children);
                active.Remove(builder);
                continue;
            }

            if (active.Contains(builder))
                throw new InvalidOperationException("A GUI builder cannot contain itself, directly or indirectly.");

            if (built.ContainsKey(builder))
                continue;

            active.Add(builder);
            pending.Push((builder, true));
            for (var i = builder._children.Count - 1; i >= 0; i--)
                pending.Push((builder._children[i], false));
        }

        return built[this];
    }

    private static GuiNodeBuilder FromNode(GuiNode node)
    {
        var clones = new Dictionary<GuiNode, GuiNodeBuilder>();
        var pending = new Stack<(GuiNode Node, bool Expanded)>();
        pending.Push((node, false));

        while (pending.Count > 0)
        {
            var (current, expanded) = pending.Pop();
            if (expanded)
            {
                var builder = new GuiNodeBuilder(current.Kind, current.Id)
                {
                    _text = current.Text,
                    _source = current.Source
                };
                foreach (var property in current.Properties)
                    builder._properties.Add(property.Key, property.Value);
                foreach (var child in current.Children)
                    builder._children.Add(clones[child]);
                clones[current] = builder;
                continue;
            }

            if (clones.ContainsKey(current))
                continue;

            pending.Push((current, true));
            for (var i = current.Children.Count - 1; i >= 0; i--)
                pending.Push((current.Children[i], false));
        }

        return clones[node];
    }
}
