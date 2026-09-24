using System.Windows.Controls.Primitives;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Layouts;

public class UniformGridGuiBuilderWPF : GuiPanelBuilderWPF<UniformGrid, UniformGridGuiBuilderWPF>
{
    public UniformGridGuiBuilderWPF(GuiConfiguration config) : base(config) { }

    public UniformGridGuiBuilderWPF WithMatrix(int rows, int columns)
    {
        _definitionRoster.Add(() => {
            Root.Rows = rows;
            Root.Columns = columns;
        });
        return this;
    }
}