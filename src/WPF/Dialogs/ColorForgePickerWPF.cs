using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Dialogs;

/// <summary>Reusable RGBA color editor with live preview and channel callbacks.</summary>
public sealed class ColorForgePickerWPF : GuiPanelBuilderWPF<Grid, ColorForgePickerWPF>
{
    private Color _color;
    private Border? _preview;

    public ColorForgePickerWPF(GuiConfiguration config, Color initialColor) : base(config) => _color = initialColor;

    public Color Color => _color;
    public Action<Color>? ColorChanged { get; set; }

    public override FrameworkElement Build()
    {
        Root.Children.Clear();
        Root.RowDefinitions.Clear();
        Root.ColumnDefinitions.Clear();

        Root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70) });
        Root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        Root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });

        AddChannel("R", 0, _color.R, value => Update(c => Color.FromArgb(c.A, value, c.G, c.B)));
        AddChannel("G", 1, _color.G, value => Update(c => Color.FromArgb(c.A, c.R, value, c.B)));
        AddChannel("B", 2, _color.B, value => Update(c => Color.FromArgb(c.A, c.R, c.G, value)));
        AddChannel("A", 3, _color.A, value => Update(c => Color.FromArgb(value, c.R, c.G, c.B)));

        Root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(55) });
        _preview = new Border { Margin = new Thickness(0, 8, 0, 0), Background = new SolidColorBrush(_color), CornerRadius = new CornerRadius(4) };
        Grid.SetRow(_preview, 4);
        Grid.SetColumnSpan(_preview, 3);
        Root.Children.Add(_preview);
        return Root;
    }

    private void AddChannel(string label, int row, byte value, Action<byte> onChanged)
    {
        Root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(32) });
        var text = new TextBlock { Text = label, VerticalAlignment = VerticalAlignment.Center };
        Grid.SetRow(text, row);
        Grid.SetColumn(text, 0);

        var slider = new Slider { Minimum = 0, Maximum = 255, Value = value, Margin = new Thickness(5, 0, 5, 0) };
        Grid.SetRow(slider, row);
        Grid.SetColumn(slider, 1);

        var readout = new TextBlock { Text = value.ToString(), VerticalAlignment = VerticalAlignment.Center };
        Grid.SetRow(readout, row);
        Grid.SetColumn(readout, 2);

        slider.ValueChanged += (_, e) => { var channel = (byte)Math.Round(e.NewValue); readout.Text = channel.ToString(); onChanged(channel); };
        Root.Children.Add(text);
        Root.Children.Add(slider);
        Root.Children.Add(readout);
    }

    private void Update(Func<Color, Color> transform)
    {
        _color = transform(_color);
        if (_preview != null) _preview.Background = new SolidColorBrush(_color);
        ColorChanged?.Invoke(_color);
    }
}
