using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Controls;

/// <summary>Hierarchical WPF builder with stable keys, payloads, and expansion/selection events.</summary>
public sealed class TreeViewGuiBuilderWPF : GuiPanelBuilderWPF<Grid, TreeViewGuiBuilderWPF>
{
    private readonly TreeView _tree = new();
    private readonly List<Branch> _branches = new();
    private readonly List<Leaf> _leaves = new();

    public TreeViewGuiBuilderWPF(GuiConfiguration config) : base(config)
    {
        _tree.SelectedItemChanged += (_, e) => NodeSelected?.Invoke((e.NewValue as TreeViewItem)?.Tag);
        _tree.Expanded += (_, e) => { if (e.OriginalSource is TreeViewItem item && item.Tag is string key) NodeExpansionChanged?.Invoke(key, true); };
        _tree.Collapsed += (_, e) => { if (e.OriginalSource is TreeViewItem item && item.Tag is string key) NodeExpansionChanged?.Invoke(key, false); };
    }

    public Action<object?>? NodeSelected { get; set; }
    public Action<string, bool>? NodeExpansionChanged { get; set; }

    public TreeViewGuiBuilderWPF AddBranch(string key, string header, bool expanded = true)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Branch key is required.", nameof(key));
        _branches.Add(new Branch(key, header ?? key, expanded));
        return this;
    }

    public TreeViewGuiBuilderWPF AddLeaf(string branchKey, string key, object? content, object? payload = null)
    {
        if (string.IsNullOrWhiteSpace(branchKey)) throw new ArgumentException("Branch key is required.", nameof(branchKey));
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Leaf key is required.", nameof(key));
        _leaves.Add(new Leaf(branchKey, key, content ?? key, payload));
        return this;
    }

    public TreeViewGuiBuilderWPF ClearNodes()
    {
        _branches.Clear();
        _leaves.Clear();
        _tree.Items.Clear();
        return this;
    }

    public override FrameworkElement Build()
    {
        _tree.Items.Clear();
        var map = new Dictionary<string, TreeViewItem>(StringComparer.Ordinal);

        foreach (var branch in _branches)
        {
            var item = new TreeViewItem { Header = branch.Header, Tag = branch.Key, IsExpanded = branch.Expanded };
            map[branch.Key] = item;
            _tree.Items.Add(item);
        }

        foreach (var leaf in _leaves)
        {
            if (!map.TryGetValue(leaf.BranchKey, out var parent)) continue;
            parent.Items.Add(new TreeViewItem { Header = leaf.Content, Tag = leaf.Key, DataContext = leaf.Payload });
        }

        Root.Children.Clear();
        Root.Children.Add(_tree);
        return Root;
    }

    private sealed record Branch(string Key, string Header, bool Expanded);
    private sealed record Leaf(string BranchKey, string Key, object Content, object? Payload);
}
