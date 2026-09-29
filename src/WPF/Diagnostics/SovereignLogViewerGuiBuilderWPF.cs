using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Diagnostics;

/// <summary>Reusable live diagnostic stream surface with filtering and bounded history.</summary>
public sealed class SovereignLogViewerGuiBuilderWPF : GuiPanelBuilderWPF<DockPanel, SovereignLogViewerGuiBuilderWPF>
{
    private readonly ObservableCollection<LogEntry> _entries = new();
    private readonly ListBox _list = new();
    private string _filter = string.Empty;
    private int _maxEntries = 500;

    public SovereignLogViewerGuiBuilderWPF(GuiConfiguration config) : base(config) { }

    public IReadOnlyList<LogEntry> Entries => _entries;
    public SovereignLogViewerGuiBuilderWPF WithMaximumEntries(int maximum)
    {
        if (maximum <= 0) throw new ArgumentOutOfRangeException(nameof(maximum));
        _maxEntries = maximum;
        Trim();
        return this;
    }

    public SovereignLogViewerGuiBuilderWPF Filter(string? text)
    {
        _filter = text ?? string.Empty;
        Refresh();
        return this;
    }

    public SovereignLogViewerGuiBuilderWPF Write(string message, LogLevel level = LogLevel.Info)
    {
        _entries.Add(new LogEntry(DateTimeOffset.UtcNow, level, message ?? string.Empty));
        Trim();
        Refresh();
        return this;
    }

    public override FrameworkElement Build()
    {
        Root.Children.Clear();
        var filterBox = new TextBox { Text = _filter, Margin = new Thickness(0, 0, 0, 5) };
        filterBox.TextChanged += (_, _) => Filter(filterBox.Text);

        _list.ItemsSource = null;
        _list.ItemsSource = _entries.Where(Matches).Select(Format).ToList();

        Root.Children.Add(filterBox);
        Root.Children.Add(_list);
        return Root;
    }

    private bool Matches(LogEntry entry) =>
        string.IsNullOrWhiteSpace(_filter) ||
        entry.Message.Contains(_filter, StringComparison.OrdinalIgnoreCase) ||
        entry.Level.ToString().Contains(_filter, StringComparison.OrdinalIgnoreCase);

    private string Format(LogEntry entry) =>
        $"[{entry.Timestamp:HH:mm:ss}] [{entry.Level}] {entry.Message}";

    private void Refresh()
    {
        if (_list.ItemsSource != null)
            _list.ItemsSource = _entries.Where(Matches).Select(Format).ToList();
    }

    private void Trim()
    {
        while (_entries.Count > _maxEntries)
            _entries.RemoveAt(0);
    }

    public enum LogLevel { Trace, Info, Warning, Error, Critical }
    public sealed record LogEntry(DateTimeOffset Timestamp, LogLevel Level, string Message);
}
