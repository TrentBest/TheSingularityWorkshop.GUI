using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Layouts;

public class StackGuiPanelBuilderWPF : GuiPanelBuilderWPF<StackPanel, StackGuiPanelBuilderWPF>
{
    public StackGuiPanelBuilderWPF(GuiConfiguration config) : base(config) { }

    public StackGuiPanelBuilderWPF WithOrientation(Orientation orientation)
    {
        _definitionRoster.Add(() => Root.Orientation = orientation);
        return this;
    }
}