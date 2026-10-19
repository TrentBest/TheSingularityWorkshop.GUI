using System.Windows;
using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Layouts;

/// <summary>Builds a Canvas panel whose children can be positioned by absolute coordinates.</summary>
public class CanvasGuiBuilderWPF : GuiPanelBuilderWPF<Canvas, CanvasGuiBuilderWPF>
{
    /// <summary>Initializes the canvas builder with the supplied GUI configuration.</summary>
    /// <param name="config">Configuration used by the shared panel builder.</param>
    public CanvasGuiBuilderWPF(GuiConfiguration config) : base(config) { }

    /// <summary>Queues an element to be placed at the specified left and top coordinates.</summary>
    /// <param name="element">Element to add to the canvas.</param>
    /// <param name="left">Horizontal offset from the canvas's left edge.</param>
    /// <param name="top">Vertical offset from the canvas's top edge.</param>
    /// <returns>This builder, for fluent configuration.</returns>
    public CanvasGuiBuilderWPF PlaceAt(UIElement element, double left, double top)
    {
        _definitionRoster.Add(() => {
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);
            Root.Children.Add(element);
        });
        return this;
    }
}