using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Gui;

public sealed class GuiNode
{
    private readonly ReadOnlyCollection<GuiNode> _children;

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

    public string Kind { get; }
    public string Id { get; }
    public string? Text { get; }
    public string? Source { get; }
    public IReadOnlyDictionary<string, string> Properties { get; }
    public IReadOnlyList<GuiNode> Children => _children;

    public GuiNode Find(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        if (TryFind(id, out var node) && node is not null)
            return node;

        throw new InvalidOperationException(
            $"GUI node '{id}' was not found beneath '{Id}'.");
    }

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
