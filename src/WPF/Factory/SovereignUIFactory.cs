using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Abstractions;
using TheSingularityWorkshop.GUI.WPF.Controls;
using TheSingularityWorkshop.GUI.WPF.Layouts;

namespace TheSingularityWorkshop.GUI.WPF.Factory;

/// <summary>
/// Central WPF builder factory. It standardizes creation without coupling callers to concrete layout wiring.
/// </summary>
public static class SovereignUIFactory
{
    public static GuiBuilderWPF Window(string name) => new(name);
    public static StackGuiPanelBuilderWPF Stack(GuiConfiguration? config = null) => new(config ?? new GuiConfiguration());
    public static GridGuiBuilderWPF Grid(GuiConfiguration? config = null) => new(config ?? new GuiConfiguration());
    public static DockGuiPanelBuilderWPF Dock(GuiConfiguration? config = null) => new(config ?? new GuiConfiguration());
    public static WrapGuiPanelBuilderWPF Wrap(GuiConfiguration? config = null) => new(config ?? new GuiConfiguration());
    public static UniformGridGuiBuilderWPF UniformGrid(GuiConfiguration? config = null) => new(config ?? new GuiConfiguration());
    public static CanvasGuiBuilderWPF Canvas(GuiConfiguration? config = null) => new(config ?? new GuiConfiguration());
    public static TabbedPanelGuiBuilderWPF Tabs(GuiConfiguration? config = null) => new(config ?? new GuiConfiguration());
    public static TreeViewGuiBuilderWPF Tree(GuiConfiguration? config = null) => new(config ?? new GuiConfiguration());
}
