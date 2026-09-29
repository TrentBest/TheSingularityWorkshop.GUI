using System.Windows;
using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Controls;

/// <summary>Strongly typed enum selector with semantic change notification.</summary>
public sealed class EnumGuiBuilderWPF<TEnum> : GuiPanelBuilderWPF<StackPanel, EnumGuiBuilderWPF<TEnum>>
    where TEnum : struct, Enum
{
    private TEnum _value;

    public EnumGuiBuilderWPF(GuiConfiguration config, TEnum value) : base(config) => _value = value;

    public TEnum Value => _value;
    public Action<TEnum>? ValueChanged { get; set; }

    public override FrameworkElement Build()
    {
        Root.Children.Clear();
        var combo = new ComboBox
        {
            ItemsSource = Enum.GetValues<TEnum>(),
            SelectedItem = _value,
            MinWidth = 140
        };
        combo.SelectionChanged += (_, _) =>
        {
            if (combo.SelectedItem is TEnum selected)
            {
                _value = selected;
                ValueChanged?.Invoke(_value);
            }
        };
        Root.Children.Add(combo);
        return Root;
    }
}
