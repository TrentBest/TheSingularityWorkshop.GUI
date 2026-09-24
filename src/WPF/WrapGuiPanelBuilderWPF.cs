using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Layouts;

public class WrapGuiPanelBuilderWPF : GuiPanelBuilderWPF<WrapPanel, WrapGuiPanelBuilderWPF>
{
    public WrapGuiPanelBuilderWPF(GuiConfiguration config) : base(config) { }

    public WrapGuiPanelBuilderWPF WithOrientation(Orientation orientation)
    {
        _definitionRoster.Add(() => Root.Orientation = orientation);
        return this;
    }

    public WrapGuiPanelBuilderWPF WithItemDimensions(double width, double height)
    {
        _definitionRoster.Add(() => {
            Root.ItemWidth = width;
            Root.ItemHeight = height;
        });
        return this;
    }
}