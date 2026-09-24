using System.Windows;
using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Layouts;

public class CanvasGuiBuilderWPF : GuiPanelBuilderWPF<Canvas, CanvasGuiBuilderWPF>
{
    public CanvasGuiBuilderWPF(GuiConfiguration config) : base(config) { }

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