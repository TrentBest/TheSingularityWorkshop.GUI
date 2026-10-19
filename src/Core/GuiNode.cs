using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>Represents an immutable node in a renderer-neutral GUI tree.</summary>
public sealed class GuiNode
{
    private readonly ReadOnlyCollection<GuiNode> _children;

    /// <summary>Creates a GUI node and snapshots its properties and child sequence.</summary>
    public GuiNode(
        string kind,
        string id,
        string? text = null,
        string? source = null,
        IReadOnlyDictionary<string, string>? properties = null,
        IEnumerable<GuiNode>? children = null)
    {
        if (string.IsNullOrWhiteSpace(kind))
            throw new ArgumentException("GUI node kind is required.", nameof(kind));

        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("GUI node id is required.", nameof(id));

        Kind = kind;
        Id = id;
        Text = text;
        Source = source;

        Properties = new ReadOnlyDictionary<string, string>(
            new Dictionary<string, string>(
                properties ?? new Dictionary<string, string>(),
                StringComparer.Ordinal));

        _children = new ReadOnlyCollection<GuiNode>(
            (children ?? Enumerable.Empty<GuiNode>()).ToList());
    }

    /// <summary>Gets the renderer-neutral kind identifier for this node.</summary>
    public string Kind { get; }
    /// <summary>Gets the identifier used to locate this node within the tree.</summary>
    public string Id { get; }
    /// <summary>Gets the optional text content associated with this node.</summary>
    public string? Text { get; }
    /// <summary>Gets the optional source or resource reference associated with this node.</summary>
    public string? Source { get; }
    /// <summary>Gets a read-only snapshot of this node's string-valued properties.</summary>
    public IReadOnlyDictionary<string, string> Properties { get; }
    /// <summary>Gets the read-only, ordered child nodes.</summary>
    public IReadOnlyList<GuiNode> Children => _children;

    /// <summary>Finds the first node with the specified identifier using depth-first, left-to-right traversal.</summary>
    public GuiNode Find(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        if (TryFind(id, out var node) && node is not null)
            return node;

        throw new InvalidOperationException(
            $"GUI node '{id}' was not found beneath '{Id}'.");
    }

    /// <summary>Attempts to find a node by identifier using depth-first, left-to-right traversal.</summary>
    public bool TryFind(string id, out GuiNode? node)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        var pending = new Stack<GuiNode>();
        pending.Push(this);

        while (pending.Count > 0)
        {
            var current = pending.Pop();
            if (current.Id == id)
            {
                node = current;
                return true;
            }

            // Reverse push preserves the existing depth-first, left-to-right search order.
            for (var i = current._children.Count - 1; i >= 0; i--)
                pending.Push(current._children[i]);
        }

        node = null;
        return false;
    }
}
