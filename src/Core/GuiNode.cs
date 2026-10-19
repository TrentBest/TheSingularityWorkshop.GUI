using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>Provides the GuiNode API.</summary>
public sealed class GuiNode
{
    private readonly ReadOnlyCollection<GuiNode> _children;

    /// <summary>Provides the GuiNode member.</summary>
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

    /// <summary>Provides the Kind member.</summary>
    public string Kind { get; }
    /// <summary>Provides the Id member.</summary>
    public string Id { get; }
    /// <summary>Sets or adds text content.</summary>
    public string? Text { get; }
    /// <summary>Provides the Source member.</summary>
    public string? Source { get; }
    /// <summary>Provides the Properties member.</summary>
    public IReadOnlyDictionary<string, string> Properties { get; }
    /// <summary>Provides the Children member.</summary>
    public IReadOnlyList<GuiNode> Children => _children;

    /// <summary>Finds and returns the first node matching an identifier.</summary>
    public GuiNode Find(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        if (TryFind(id, out var node) && node is not null)
            return node;

        throw new InvalidOperationException(
            $"GUI node '{id}' was not found beneath '{Id}'.");
    }

    /// <summary>Attempts to find a node matching an identifier.</summary>
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
