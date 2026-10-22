using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>
/// Fluent, platform-neutral builder for a semantic GUI tree. Child builders compose
/// recursively; Build creates an immutable snapshot without recursive traversal.
/// </summary>
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

    /// <summary>
    /// Declares the stable, host-neutral command identity dispatched when this node is activated.
    /// The host must explicitly map the command to an allowed operation; this does not execute it.
    /// </summary>
    public GuiBuilder WithOnClick(string commandId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandId);
        _root.Properties["onClickCommand"] = commandId;
        return this;
    }

    /// <summary>Adds and configures a child using a fluent callback.</summary>
    public GuiBuilder Child(string kind, string id, Action<GuiBuilder>? configure = null)
    {
        var child = new GuiBuilder(new NodeBuilder(kind, id));
        configure?.Invoke(child);
        _root.Children.Add(child._root);
        return this;
    }

    /// <summary>Adds another builder as a recursively composed child.</summary>
    public GuiBuilder Child(GuiBuilder child)
    {
        ArgumentNullException.ThrowIfNull(child);
        _root.Children.Add(child._root);
        return this;
    }

    /// <summary>Adds an existing immutable node as a child snapshot.</summary>
    public GuiBuilder Child(GuiNode child)
    {
        ArgumentNullException.ThrowIfNull(child);
        _root.Children.Add(FromNode(child));
        return this;
    }

    /// <summary>
    /// Builds an immutable snapshot iteratively. Cyclic builder composition is rejected
    /// with a controlled exception rather than overflowing the process call stack.
    /// </summary>
    public GuiNode Build()
    {
        var built = new Dictionary<NodeBuilder, GuiNode>();
        var active = new HashSet<NodeBuilder>();
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
                active.Remove(builder);
                continue;
            }

            if (active.Contains(builder))
                throw new InvalidOperationException("A GUI builder cannot contain itself, directly or indirectly.");

            if (built.ContainsKey(builder))
                continue;

            active.Add(builder);
            pending.Push((builder, true));
            for (var i = builder.Children.Count - 1; i >= 0; i--)
                pending.Push((builder.Children[i], false));
        }

        return built[_root];
    }

    private static NodeBuilder FromNode(GuiNode node)
    {
        var clones = new Dictionary<GuiNode, NodeBuilder>();
        var pending = new Stack<(GuiNode Node, bool Expanded)>();
        pending.Push((node, false));

        while (pending.Count > 0)
        {
            var (current, expanded) = pending.Pop();
            if (expanded)
            {
                var builder = new NodeBuilder(current.Kind, current.Id)
                {
                    Text = current.Text,
                    Source = current.Source
                };
                foreach (var property in current.Properties)
                    builder.Properties.Add(property.Key, property.Value);
                foreach (var child in current.Children)
                    builder.Children.Add(clones[child]);
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

    private sealed class NodeBuilder
    {
        public NodeBuilder(string kind, string id)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(kind);
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            Kind = kind;
            Id = id;
        }

        public string Kind { get; }
        public string Id { get; }
        public string? Text { get; set; }
        public string? Source { get; set; }
        public Dictionary<string, string> Properties { get; } = new(StringComparer.Ordinal);
        public List<NodeBuilder> Children { get; } = new();
    }
}
