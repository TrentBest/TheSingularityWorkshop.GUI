using System.Windows;
using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Layouts;

public class DockGuiPanelBuilderWPF : GuiPanelBuilderWPF<DockPanel, DockGuiPanelBuilderWPF>
{
    public DockGuiPanelBuilderWPF(GuiConfiguration config) : base(config) { }

    public DockGuiPanelBuilderWPF WithLastChildFill(bool fill)
    {
        _definitionRoster.Add(() => Root.LastChildFill = fill);
        return this;
    }

    public DockGuiPanelBuilderWPF AddDocked(UIElement element, Dock edge)
    {
        _definitionRoster.Add(() => {
            DockPanel.SetDock(element, edge);
            Root.Children.Add(element);
        });
        return this;
    }
}