using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Layouts;

/// <summary>Composable tab surface with semantic tab identity and deferred content construction.</summary>
public sealed class TabbedPanelGuiBuilderWPF : GuiPanelBuilderWPF<Grid, TabbedPanelGuiBuilderWPF>
{
    private readonly List<TabDefinition> _tabs = new();
    private readonly TabControl _tabsControl = new();
    private Brush _headerBrush = SystemColors.ControlBrush;

    public TabbedPanelGuiBuilderWPF(GuiConfiguration config) : base(config)
    {
        _tabsControl.Background = SystemColors.WindowBrush;
        _tabsControl.BorderBrush = SystemColors.ActiveBorderBrush;
        _tabsControl.SelectionChanged += (_, _) => SelectedTabChanged?.Invoke(SelectedKey);
    }

    public string? SelectedKey => (_tabsControl.SelectedItem as TabItem)?.Tag as string;
    public Action<string?>? SelectedTabChanged { get; set; }

    public TabbedPanelGuiBuilderWPF WithHeaderBrush(Brush brush)
    {
        _headerBrush = brush ?? throw new ArgumentNullException(nameof(brush));
        return this;
    }

    public TabbedPanelGuiBuilderWPF AddTab(string key, string header, IComponentWPF content, bool select = false)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Tab key is required.", nameof(key));
        ArgumentNullException.ThrowIfNull(content);
        _tabs.Add(new TabDefinition(key, header ?? key, content, select));
        content.SetParent(this);
        return this;
    }

    public TabbedPanelGuiBuilderWPF AddTab(string key, string header, Action<StackPanel> configure, bool select = false)
    {
        ArgumentNullException.ThrowIfNull(configure);
        var panel = new StackPanel();
        configure(panel);
        _tabs.Add(new TabDefinition(key, header ?? key, new NativeComponent(panel), select));
        return this;
    }

    public override FrameworkElement Build()
    {
        _tabsControl.Items.Clear();
        foreach (var tab in _tabs)
        {
            var item = new TabItem
            {
                Header = tab.Header,
                Tag = tab.Key,
                Background = _headerBrush,
                Content = tab.Content.Build()
            };
            _tabsControl.Items.Add(item);
            if (tab.Select) item.IsSelected = true;
        }

        Root.Children.Clear();
        Root.Children.Add(_tabsControl);
        return Root;
    }

    private sealed record TabDefinition(string Key, string Header, IComponentWPF Content, bool Select);

    private sealed class NativeComponent : IComponentWPF
    {
        private readonly FrameworkElement _element;
        public NativeComponent(FrameworkElement element) => _element = element;
        public string Id { get; } = Guid.NewGuid().ToString();
        public void SetParent(IComponentWPF? parent) { }
        public IComponentWPF? GetParent() => null;
        public void Publish(string topic, object payload) { }
        public void Receive(string topic, object payload) { }
        public FrameworkElement Build() => _element;
    }
}
