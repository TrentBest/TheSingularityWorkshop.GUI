using System;
using System.Windows;
using System.Windows.Controls;

namespace TheSingularityWorkshop.GUI.WPF.Controls;

/// <summary>
/// Builds a reusable enum selector from a declared enum type without reflecting over arbitrary object models.
/// The caller owns the control lifetime and decides how selected values are persisted.
/// </summary>
public sealed class EnumControlBuilderWPF
{
    private readonly string _label;
    private readonly Type _enumType;
    private object _selectedValue;

    public Action<object>? ValueChanged { get; set; }

    public object SelectedValue => _selectedValue;

    public EnumControlBuilderWPF(
        string label,
        Type enumType,
        object selectedValue,
        Action<object>? valueChanged = null)
    {
        _label = label ?? throw new ArgumentNullException(nameof(label));
        _enumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
        if (!_enumType.IsEnum)
            throw new ArgumentException("The supplied type must be an enum.", nameof(enumType));
        if (selectedValue is null || selectedValue.GetType() != _enumType)
            throw new ArgumentException("The selected value must be a value of the supplied enum type.", nameof(selectedValue));

        _selectedValue = selectedValue;
        ValueChanged = valueChanged;
    }

    /// <summary>Creates a label and native ComboBox. ValueChanged fires only after selection changes.</summary>
    public FrameworkElement Build()
    {
        var row = new DockPanel { LastChildFill = true, VerticalAlignment = VerticalAlignment.Center };
        var label = new TextBlock
        {
            Text = _label,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 8, 0)
        };
        DockPanel.SetDock(label, Dock.Left);

        var selector = new ComboBox
        {
            ItemsSource = Enum.GetValues(_enumType),
            SelectedItem = _selectedValue,
            MinWidth = 120,
            VerticalContentAlignment = VerticalAlignment.Center
        };
        selector.SelectionChanged += (_, _) =>
        {
            if (selector.SelectedItem is { } selected && selected.GetType() == _enumType)
            {
                _selectedValue = selected;
                ValueChanged?.Invoke(selected);
            }
        };

        row.Children.Add(label);
        row.Children.Add(selector);
        return row;
    }
}
