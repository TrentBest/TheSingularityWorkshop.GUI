using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Controls;

/// <summary>Numeric input builder supporting slider and text-stepper manifestations.</summary>
public sealed class NumericGuiBuilderWPF : GuiPanelBuilderWPF<DockPanel, NumericGuiBuilderWPF>
{
    private double _value;
    private double _minimum;
    private double _maximum = 100;
    private double _step = 1;
    private bool _slider = true;
    private TextBox? _text;
    private Slider? _sliderControl;

    public NumericGuiBuilderWPF(GuiConfiguration config, double value = 0) : base(config)
    {
        _value = value;
    }

    public double Value => _value;
    public Action<double>? ValueChanged { get; set; }

    public NumericGuiBuilderWPF WithRange(double minimum, double maximum)
    {
        if (maximum < minimum) throw new ArgumentException("Maximum must be >= minimum.");
        _minimum = minimum;
        _maximum = maximum;
        _value = Math.Clamp(_value, minimum, maximum);
        return this;
    }

    public NumericGuiBuilderWPF WithStep(double step)
    {
        if (step <= 0) throw new ArgumentOutOfRangeException(nameof(step));
        _step = step;
        return this;
    }

    public NumericGuiBuilderWPF UseSlider(bool enabled = true) { _slider = enabled; return this; }

    public override FrameworkElement Build()
    {
        Root.Children.Clear();

        var label = new TextBlock { Text = "VALUE", Margin = new Thickness(0, 0, 0, 4) };
        Root.Children.Add(label);

        if (_slider)
        {
            _sliderControl = new Slider { Minimum = _minimum, Maximum = _maximum, Value = _value, TickFrequency = _step, IsSnapToTickEnabled = true };
            _sliderControl.ValueChanged += (_, e) => SetValue(e.NewValue);
            Root.Children.Add(_sliderControl);
        }

        _text = new TextBox { Text = _value.ToString(CultureInfo.InvariantCulture), Margin = new Thickness(0, 4, 0, 0) };
        _text.LostFocus += (_, _) => ParseText();
        _text.KeyDown += (_, e) => { if (e.Key == System.Windows.Input.Key.Enter) ParseText(); };
        Root.Children.Add(_text);

        return Root;
    }

    private void ParseText()
    {
        if (double.TryParse(_text?.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed))
            SetValue(parsed);
        else if (_text != null)
            _text.Text = _value.ToString(CultureInfo.InvariantCulture);
    }

    private void SetValue(double value)
    {
        _value = Math.Clamp(value, _minimum, _maximum);
        if (_text != null) _text.Text = _value.ToString(CultureInfo.InvariantCulture);
        if (_sliderControl != null && Math.Abs(_sliderControl.Value - _value) > double.Epsilon) _sliderControl.Value = _value;
        ValueChanged?.Invoke(_value);
    }
}
