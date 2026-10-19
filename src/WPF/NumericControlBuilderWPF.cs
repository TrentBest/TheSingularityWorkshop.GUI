using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace TheSingularityWorkshop.GUI.WPF.Controls;

/// <summary>
/// Builds a reusable numeric input row using native WPF controls.
/// The caller owns the control lifetime and decides how values are persisted.
/// </summary>
public sealed class NumericControlBuilderWPF
{
    private readonly string _label;
    private readonly double _minimum;
    private readonly double _maximum;
    private double _value;

    public Action<double>? ValueChanged { get; set; }

    public double Value => _value;

    public NumericControlBuilderWPF(string label, double value, double minimum = 0, double maximum = 100)
    {
        _label = label ?? throw new ArgumentNullException(nameof(label));
        if (!double.IsFinite(minimum))
            throw new ArgumentOutOfRangeException(nameof(minimum), "Minimum must be finite.");
        if (!double.IsFinite(maximum) || maximum <= minimum)
            throw new ArgumentOutOfRangeException(nameof(maximum), "Maximum must be finite and greater than minimum.");
        if (!double.IsFinite(value) || value < minimum || value > maximum)
            throw new ArgumentOutOfRangeException(nameof(value), "Value must be finite and within the configured range.");

        _minimum = minimum;
        _maximum = maximum;
        _value = value;
    }

    /// <summary>Creates a label, slider, and readout row. ValueChanged fires only after user interaction.</summary>
    public FrameworkElement Build()
    {
        var row = new Grid { VerticalAlignment = VerticalAlignment.Center };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var label = new TextBlock
        {
            Text = _label,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 8, 0)
        };
        Grid.SetColumn(label, 0);

        var slider = new Slider
        {
            Minimum = _minimum,
            Maximum = _maximum,
            Value = _value,
            VerticalAlignment = VerticalAlignment.Center,
            IsSnapToTickEnabled = false
        };
        Grid.SetColumn(slider, 1);

        var readout = new TextBlock
        {
            Text = Format(_value),
            MinWidth = 64,
            TextAlignment = TextAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(8, 0, 0, 0)
        };
        Grid.SetColumn(readout, 2);

        slider.ValueChanged += (_, args) =>
        {
            _value = args.NewValue;
            readout.Text = Format(_value);
            ValueChanged?.Invoke(_value);
        };

        row.Children.Add(label);
        row.Children.Add(slider);
        row.Children.Add(readout);
        return row;
    }

    private static string Format(double value) => value.ToString("G", CultureInfo.InvariantCulture);
}
