using System.Windows;
using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Layouts;

/// <summary>Keyed multi-panel composition surface for dashboard and workspace builders.</summary>
public sealed class MultiPanelGuiBuilderWPF : GridGuiBuilderWPF
{
    private readonly Dictionary<string, IComponentWPF> _panels = new(StringComparer.Ordinal);

    public MultiPanelGuiBuilderWPF(GuiConfiguration config) : base(config) { }

    public MultiPanelGuiBuilderWPF AddPanel(string key, int row, int column, IComponentWPF panel, int rowSpan = 1, int columnSpan = 1)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Panel key is required.", nameof(key));
        ArgumentNullException.ThrowIfNull(panel);
        if (!_panels.TryAdd(key, panel)) throw new ArgumentException($"Panel '{key}' already exists.", nameof(key));
        PlaceComponent(row, column, panel);
        return this;
    }

    public bool TryGetPanel(string key, out IComponentWPF? panel) => _panels.TryGetValue(key, out panel);

    public MultiPanelGuiBuilderWPF RemovePanel(string key)
    {
        _panels.Remove(key);
        return this;
    }

    public IReadOnlyDictionary<string, IComponentWPF> Panels => _panels;
}
