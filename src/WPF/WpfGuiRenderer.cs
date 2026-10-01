using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.WPF;

/// <summary>
/// WPF manifestation adapter for the platform-neutral GUI node tree.
/// </summary>
public static class WpfGuiRenderer
{
    public static FrameworkElement Render(GuiNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        return RenderNode(node);
    }

    private static FrameworkElement RenderNode(GuiNode node)
    {
        FrameworkElement element = node.Kind switch
        {
            GuiKinds.Text => new TextBlock { Text = node.Text ?? string.Empty },
            GuiKinds.Button => CreateButton(node),
            GuiKinds.TextBox => CreateTextBox(node),
            GuiKinds.Image => CreateImage(node),
            GuiKinds.Warning => CreateWarning(node),
            GuiKinds.Separator => new Separator(),
            GuiKinds.Row => new StackPanel { Orientation = Orientation.Horizontal },
            GuiKinds.Stack => new StackPanel { Orientation = ResolveOrientation(node) },
            GuiKinds.Column => new StackPanel { Orientation = Orientation.Vertical },
            GuiKinds.Panel => new Grid(),
            _ => new Grid()
        };

        element.Name = SanitizeName(node.Id);
        ApplyProperties(element, node);

        if (element is Panel panel)
        {
            foreach (var child in node.Children)
                panel.Children.Add(RenderNode(child));
        }
        else if (element is ContentControl content && node.Children.Count > 0)
        {
            content.Content = RenderNode(node.Children[0]);
        }

        return element;
    }

    private static Button CreateButton(GuiNode node) =>
        new() { Content = node.Text ?? string.Empty };

    private static TextBox CreateTextBox(GuiNode node) =>
        new() { Text = GetProperty(node, "value") ?? node.Text ?? string.Empty };

    private static Image CreateImage(GuiNode node)
    {
        var image = new Image();
        if (!string.IsNullOrWhiteSpace(node.Source))
        {
            try
            {
                image.Source = new BitmapImage(new Uri(node.Source, UriKind.RelativeOrAbsolute));
            }
            catch (UriFormatException)
            {
                // Leave Source unset; the semantic tree remains renderable.
            }
        }

        return image;
    }

    private static Border CreateWarning(GuiNode node) =>
        new()
        {
            Child = new TextBlock
            {
                Text = node.Text ?? string.Empty,
                TextWrapping = TextWrapping.Wrap
            },
            Padding = new Thickness(8)
        };

    private static Orientation ResolveOrientation(GuiNode node) =>
        string.Equals(GetProperty(node, "orientation"), "Horizontal", StringComparison.OrdinalIgnoreCase)
            ? Orientation.Horizontal
            : Orientation.Vertical;

    private static void ApplyProperties(FrameworkElement element, GuiNode node)
    {
        if (TryDouble(node, "width", out var width))
            element.Width = width;

        if (TryDouble(node, "height", out var height))
            element.Height = height;

        if (TryDouble(node, "opacity", out var opacity))
            element.Opacity = opacity;

        if (bool.TryParse(GetProperty(node, "isEnabled"), out var enabled))
            element.IsEnabled = enabled;

        if (bool.TryParse(GetProperty(node, "isVisible"), out var visible))
            element.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;

        if (TryThickness(node, "margin", out var margin))
            element.Margin = margin;

        if (TryThickness(node, "padding", out var padding) && element is Control control)
            control.Padding = padding;

        if (element is Control styled)
        {
            if (TryBrush(node, "foreground", out var foreground))
                styled.Foreground = foreground;

            if (TryBrush(node, "background", out var background))
                styled.Background = background;
        }

        if (element is Control control)
        {
            if (TryDouble(node, "fontSize", out var fontSize))
                control.FontSize = fontSize;

            if (int.TryParse(GetProperty(node, "fontWeight"), out var fontWeight))
                control.FontWeight = FontWeight.FromOpenTypeWeight(fontWeight);

            if (node.Properties.TryGetValue("fontFamily", out var fontFamily) &&
                !string.IsNullOrWhiteSpace(fontFamily))
            {
                control.FontFamily = new System.Windows.Media.FontFamily(fontFamily);
            }
        }

        if (TryHorizontalAlignment(node, out var horizontalAlignment))
            element.HorizontalAlignment = horizontalAlignment;

        if (TryVerticalAlignment(node, out var verticalAlignment))
            element.VerticalAlignment = verticalAlignment;

        ApplyWaveAnimation(element, node);

        if (element is Panel panel && TryBrush(node, "background", out var panelBackground))
            panel.Background = panelBackground;

        if (node.Properties.TryGetValue("tooltip", out var tooltip))
            element.ToolTip = tooltip;
    }

    private static bool TryHorizontalAlignment(GuiNode node, out HorizontalAlignment alignment)
    {
        alignment = HorizontalAlignment.Stretch;
        return Enum.TryParse(GetProperty(node, "horizontalAlignment"), true, out alignment);
    }

    private static bool TryVerticalAlignment(GuiNode node, out VerticalAlignment alignment)
    {
        alignment = VerticalAlignment.Stretch;
        return Enum.TryParse(GetProperty(node, "verticalAlignment"), true, out alignment);
    }

    private static void ApplyWaveAnimation(FrameworkElement element, GuiNode node)
    {
        if (!TryDouble(node, "wavePeriod", out var period) || period <= 0)
            return;

        if (!TryDouble(node, "waveAmplitude", out var amplitude) || amplitude == 0)
            amplitude = 4;

        if (!TryDouble(node, "waveRotation", out var rotation) || rotation == 0)
            rotation = 2;

        var phase = 0d;
        TryDouble(node, "wavePhase", out phase);
        phase = Math.Clamp(phase, 0, 1);

        var group = new TransformGroup();
        var translate = new TranslateTransform();
        var rotate = new RotateTransform();
        group.Children.Add(translate);
        group.Children.Add(rotate);
        element.RenderTransformOrigin = new Point(.5, 1);
        element.RenderTransform = group;

        var duration = new Duration(TimeSpan.FromSeconds(period));
        translate.BeginAnimation(
            TranslateTransform.YProperty,
            new DoubleAnimation
            {
                From = -amplitude,
                To = amplitude,
                Duration = duration,
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                BeginTime = TimeSpan.FromSeconds(period * phase)
            });
        rotate.BeginAnimation(
            RotateTransform.AngleProperty,
            new DoubleAnimation
            {
                From = -rotation,
                To = rotation,
                Duration = duration,
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                BeginTime = TimeSpan.FromSeconds(period * phase)
            });
    }

    private static string? GetProperty(GuiNode node, string name) =>
        node.Properties.TryGetValue(name, out var value) ? value : null;

    private static bool TryDouble(GuiNode node, string name, out double value) =>
        double.TryParse(GetProperty(node, name), NumberStyles.Float, CultureInfo.InvariantCulture, out value);

    private static bool TryThickness(GuiNode node, string name, out Thickness value)
    {
        value = default;
        var raw = GetProperty(node, name);
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        try
        {
            value = (Thickness)new ThicknessConverter().ConvertFromInvariantString(raw)!;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryBrush(GuiNode node, string name, out Brush? brush)
    {
        brush = null;
        var raw = GetProperty(node, name);
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        try
        {
            brush = (Brush)new BrushConverter().ConvertFromInvariantString(raw)!;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string SanitizeName(string id)
    {
        var chars = id.Where(char.IsLetterOrDigit).ToArray();
        if (chars.Length == 0)
            return "GuiNode";

        var name = new string(chars);
        return char.IsLetter(name[0]) ? name : "Gui_" + name;
    }}
