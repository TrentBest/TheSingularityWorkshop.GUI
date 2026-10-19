using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>Builds a platform-neutral semantic GUI node tree.</summary>
public sealed class GuiBuilder
{
    private readonly NodeBuilder _root;
    private GuiBuilder(NodeBuilder root) => _root = root;
    /// <summary>Creates a builder or adapter for the supplied root or element.</summary>
    public static GuiBuilder Create(string kind, string id) => new(new NodeBuilder(kind, id));
    /// <summary>Sets or adds text content.</summary>
    public GuiBuilder Text(string text) { _root.Text = text; return this; }
    /// <summary>Sets an image source reference.</summary>
    public GuiBuilder Image(string source) { _root.Source = source; return this; }
    /// <summary>Sets a string-valued node property.</summary>
    public GuiBuilder Property(string name, string value) { _root.Properties[name] = value; return this; }
    /// <summary>Adds a child element or render fragment.</summary>
    public GuiBuilder Child(string kind, string id, Action<GuiBuilder>? configure = null)
    {
        var child = new NodeBuilder(kind, id);
        var builder = new GuiBuilder(child);
        configure?.Invoke(builder);
        _root.Children.Add(child);
        return this;
    }
    /// <summary>Builds and returns the configured GUI node or render fragment.</summary>
    public GuiNode Build() => _root.Build();
    private sealed class NodeBuilder
    {
        /// <summary>Provides the NodeBuilder member.</summary>
        public NodeBuilder(string kind, string id) { Kind = kind; Id = id; }
        /// <summary>Provides the Kind member.</summary>
        public string Kind { get; }
        /// <summary>Provides the Id member.</summary>
        public string Id { get; }
        /// <summary>Sets or adds text content.</summary>
        public string? Text { get; set; }
        /// <summary>Provides the Source member.</summary>
        public string? Source { get; set; }
        /// <summary>Provides the Properties member.</summary>
        public Dictionary<string,string> Properties { get; } = new(StringComparer.Ordinal);
        /// <summary>Provides the Children member.</summary>
        public List<NodeBuilder> Children { get; } = new();
        /// <summary>Builds and returns the configured GUI node or render fragment.</summary>
        public GuiNode Build() => new(Kind, Id, Text, Source, Properties, Children.ConvertAll(x => x.Build()));
    }
}