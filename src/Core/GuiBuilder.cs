using System;
using System.Collections.Generic;

namespace TheSingularityWorkshop.Workshop.Gui;

public sealed class GuiBuilder
{
    private readonly NodeBuilder _root;
    private GuiBuilder(NodeBuilder root) => _root = root;
    public static GuiBuilder Create(string kind, string id) => new(new NodeBuilder(kind, id));
    public GuiBuilder Text(string text) { _root.Text = text; return this; }
    public GuiBuilder Image(string source) { _root.Source = source; return this; }
    public GuiBuilder Property(string name, string value) { _root.Properties[name] = value; return this; }
    public GuiBuilder Child(string kind, string id, Action<GuiBuilder>? configure = null)
    {
        var child = new NodeBuilder(kind, id);
        var builder = new GuiBuilder(child);
        configure?.Invoke(builder);
        _root.Children.Add(child);
        return this;
    }
    public GuiNode Build() => _root.Build();
    private sealed class NodeBuilder
    {
        public NodeBuilder(string kind, string id) { Kind = kind; Id = id; }
        public string Kind { get; }
        public string Id { get; }
        public string? Text { get; set; }
        public string? Source { get; set; }
        public Dictionary<string,string> Properties { get; } = new(StringComparer.Ordinal);
        public List<NodeBuilder> Children { get; } = new();
        public GuiNode Build() => new(Kind, Id, Text, Source, Properties, Children.ConvertAll(x => x.Build()));
    }
}