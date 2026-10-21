using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>Builds a platform-neutral semantic GUI node tree.</summary>
public sealed class GuiBuilder
{
    private readonly NodeBuilder _root;
    private GuiBuilder(NodeBuilder root) => _root = root;

    /// <summary>Creates a builder for the supplied semantic root kind and identifier.</summary>
    public static GuiBuilder Create(string kind, string id) => new(new NodeBuilder(kind, id));

    /// <summary>Sets or adds text content.</summary>
    public GuiBuilder Text(string text) { _root.Text = text; return this; }

    /// <summary>Sets an image source reference.</summary>
    public GuiBuilder Image(string source) { _root.Source = source; return this; }

    /// <summary>Sets a string-valued node property.</summary>
    public GuiBuilder Property(string name, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(value);
        _root.Properties[name] = value;
        return this;
    }

    /// <summary>Adds a child node and configures it with a fluent callback.</summary>
    public GuiBuilder Child(string kind, string id, Action<GuiBuilder>? configure = null)
    {
        var child = new NodeBuilder(kind, id);
        var builder = new GuiBuilder(child);
        configure?.Invoke(builder);
        _root.Children.Add(child);
        return this;
    }

    /// <summary>
    /// Builds a snapshot of the configured tree without recursive materialization,
    /// so deeply nested builder input cannot exhaust the process call stack.
    /// </summary>
    public GuiNode Build()
    {
        var built = new Dictionary<NodeBuilder, GuiNode>();
        var pending = new Stack<(NodeBuilder Builder, bool Expanded)>();
        pending.Push((_root, false));

        while (pending.Count > 0)
        {
            var (builder, expanded) = pending.Pop();
            if (expanded)
            {
                var children = new List<GuiNode>(builder.Children.Count);
                foreach (var child in builder.Children)
                    children.Add(built[child]);

                built[builder] = new GuiNode(
                    builder.Kind, builder.Id, builder.Text, builder.Source,
                    builder.Properties, children);
                continue;
            }

            if (built.ContainsKey(builder))
                continue;

            pending.Push((builder, true));
            for (var i = builder.Children.Count - 1; i >= 0; i--)
                pending.Push((builder.Children[i], false));
        }

        return built[_root];
    }

    private sealed class NodeBuilder
    {
        public NodeBuilder(string kind, string id) { Kind = kind; Id = id; }
        public string Kind { get; }
        public string Id { get; }
        public string? Text { get; set; }
        public string? Source { get; set; }
        public Dictionary<string, string> Properties { get; } = new(StringComparer.Ordinal);
        public List<NodeBuilder> Children { get; } = new();
    }
}
