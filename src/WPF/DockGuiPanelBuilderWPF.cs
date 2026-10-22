using System.Windows;
using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Layouts;

/// <summary>Builds a DockPanel and supports docking individual child elements to its edges.</summary>
public class DockGuiPanelBuilderWPF : GuiPanelBuilderWPF<DockPanel, DockGuiPanelBuilderWPF>
{
    /// <summary>Initializes the dock panel builder with the supplied GUI configuration.</summary>
    /// <param name="config">Configuration used by the shared panel builder.</param>
    public DockGuiPanelBuilderWPF(GuiConfiguration config) : base(config) { }

    /// <summary>Sets whether the last child fills the remaining panel area.</summary>
    /// <param name="fill">True to let the last child fill the remaining area; otherwise false.</param>
    /// <returns>This builder, for fluent configuration.</returns>
    public DockGuiPanelBuilderWPF WithLastChildFill(bool fill)
    {
        _definitionRoster.Add(() => Root.LastChildFill = fill);
        return this;
    }

    /// <summary>Queues a child element and assigns the edge against which it is docked.</summary>
    /// <param name="element">Element to add to the dock panel.</param>
    /// <param name="edge">Panel edge to dock the element against.</param>
    /// <returns>This builder, for fluent configuration.</returns>
    public DockGuiPanelBuilderWPF AddDocked(UIElement element, Dock edge)
    {
        RegisterRawElement(element);
        _definitionRoster.Add(() => {
            DockPanel.SetDock(element, edge);
            Root.Children.Add(element);
        });
        return this;
    }
}