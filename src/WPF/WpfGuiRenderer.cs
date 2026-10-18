using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.GUI.WPF;

/// <summary>
/// Recursively manifests a platform-neutral GUI node tree as native WPF controls.
/// Core remains unaware of WPF; unknown kinds degrade to a simple content panel.
/// </summary>
public static class WpfGuiRenderer
{
    private const int MaximumTreeDepth = 256;

    public static FrameworkElement Render(GuiNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        ValidateTree(node);
        return RenderValidated(node);
    }

    private static FrameworkElement RenderValidated(GuiNode node)
    {
        FrameworkElement element = node.Kind switch
        {
            "Button" => new Button { Content = node.Text ?? string.Empty },
            "Text" => new TextBlock { Text = node.Text ?? string.Empty, TextWrapping = TextWrapping.Wrap },
            "Column" => new StackPanel { Orientation = Orientation.Vertical },
            "Panel" => new Border { Child = new StackPanel { Orientation = Orientation.Vertical } },
            "Warning" => CreateWarning(node.Text),
            "Image" => CreateImage(node.Source),
            _ => new Border { Child = new StackPanel { Orientation = Orientation.Vertical } }
        };

        ApplyProperties(element, node);

        if (node.Children.Count > 0)
        {
            var childrenHost = GetChildrenHost(element);
            if (childrenHost is not null)
            {
                foreach (var child in node.Children)
                    childrenHost.Children.Add(RenderValidated(child));
            }
            else if (element is Button button)
            {
                var content = new StackPanel { Orientation = Orientation.Vertical };
                if (button.Content is string text && !string.IsNullOrEmpty(text))
                    content.Children.Add(new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap });
                foreach (var child in node.Children)
                    content.Children.Add(RenderValidated(child));
                button.Content = content;
            }
        }

        return element;
    }

    private static void ValidateTree(GuiNode root)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var pending = new Stack<(GuiNode Node, int Depth)>();
        pending.Push((root, 0));

        while (pending.Count > 0)
        {
            var (node, depth) = pending.Pop();
            if (depth > MaximumTreeDepth)
                throw new InvalidOperationException(
                    $"GUI tree exceeds the WPF renderer's maximum depth ({MaximumTreeDepth}).");

            if (!ids.Add(node.Id))
                throw new ArgumentException(
                    $"GUI node id '{node.Id}' is duplicated. Node ids must be unique within a rendered tree.",
                    nameof(root));

            foreach (var child in node.Children)
                pending.Push((child, depth + 1));
        }
    }

    private static StackPanel? GetChildrenHost(FrameworkElement element) => element switch
    {
        StackPanel panel => panel,
        Border { Child: StackPanel panel } => panel,
        _ => null
    };

    private static FrameworkElement CreateWarning(string? text) =>
        new Border
        {
            Background = new SolidColorBrush(Color.FromRgb(0x3A, 0x1B, 0x08)),
            BorderBrush = new SolidColorBrush(Color.FromRgb(0xFF, 0xA0, 0x30)),
            BorderThickness = new Thickness(1),
            Padding = new Thickness(12),
            Child = new TextBlock
            {
                Text = text ?? "Warning",
                Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xD0, 0x90)),
                TextWrapping = TextWrapping.Wrap
            }
        };

    private static FrameworkElement CreateImage(string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return new TextBlock { Text = "[image source not set]" };

        try
        {
            return new Image
            {
                Source = new BitmapImage(new Uri(source, UriKind.RelativeOrAbsolute)),
                Stretch = Stretch.Uniform
            };
        }
        catch (UriFormatException)
        {
            return new TextBlock { Text = $"[invalid image source: {source}]" };
        }
    }

    private static void ApplyProperties(FrameworkElement element, GuiNode node)
    {
        WpfGuiIdentity.SetNodeId(element, node.Id);
        element.Name = IsLegalName(node.Id) ? node.Id : string.Empty;

        foreach (var property in node.Properties)
        {
            var key = property.Key;
            var value = property.Value;
            switch (key)
            {
                case "width": if (TryDouble(value, out var width)) element.Width = width; break;
                case "height": if (TryDouble(value, out var height)) element.Height = height; break;
                case "margin": if (TryThickness(value, out var margin)) element.Margin = margin; break;
                case "padding":
                    if (TryThickness(value, out var padding))
                    {
                        if (element is Control control) control.Padding = padding;
                        else if (element is Border border) border.Padding = padding;
                    }
                    break;
                case "horizontalAlignment":
                    if (Enum.TryParse<HorizontalAlignment>(value, true, out var horizontal)) element.HorizontalAlignment = horizontal;
                    break;
                case "verticalAlignment":
                    if (Enum.TryParse<VerticalAlignment>(value, true, out var vertical)) element.VerticalAlignment = vertical;
                    break;
                case "tooltip": element.ToolTip = value; break;
                case "background":
                    if (TryBrush(value, out var background))
                    {
                        if (element is Control backgroundControl) backgroundControl.Background = background;
                        else if (element is Border b) b.Background = background;
                    }
                    break;
                case "foreground":
                    if (TryBrush(value, out var foreground))
                    {
                        if (element is Control foregroundControl) foregroundControl.Foreground = foreground;
                        else if (element is TextBlock foregroundTextBlock) foregroundTextBlock.Foreground = foreground;
                    }
                    break;
                case "fontSize":
                    if (TryDouble(value, out var fontSize))
                    {
                        if (element is Control fontSizeControl) fontSizeControl.FontSize = fontSize;
                        else if (element is TextBlock fontSizeTextBlock) fontSizeTextBlock.FontSize = fontSize;
                    }
                    break;
                case "fontWeight":
                    if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var weight))
                    {
                        var fontWeight = weight >= 700 ? FontWeights.Bold : weight >= 600 ? FontWeights.SemiBold : weight >= 500 ? FontWeights.Medium : FontWeights.Normal;
                        if (element is Control fontWeightControl) fontWeightControl.FontWeight = fontWeight;
                        else if (element is TextBlock fontWeightTextBlock) fontWeightTextBlock.FontWeight = fontWeight;
                    }
                    break;
                case "fontFamily":
                    if (element is Control fontFamilyControl) fontFamilyControl.FontFamily = new FontFamily(value);
                    else if (element is TextBlock fontFamilyTextBlock) fontFamilyTextBlock.FontFamily = new FontFamily(value);
                    break;
            }
        }
    }

    private static bool TryDouble(string value, out double result) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result)
        && !double.IsNaN(result) && !double.IsInfinity(result) && result >= 0;

    private static bool TryThickness(string value, out Thickness thickness)
    {
        thickness = default;
        var parts = value.Split(',').Select(p => p.Trim()).ToArray();
        var values = new double[parts.Length];
        if (parts.Length is not (1 or 2 or 4)) return false;
        for (var i = 0; i < parts.Length; i++)
            if (!TryDouble(parts[i], out values[i])) return false;

        thickness = parts.Length switch
        {
            1 => new Thickness(values[0]),
            2 => new Thickness(values[0], values[1], values[0], values[1]),
            _ => new Thickness(values[0], values[1], values[2], values[3])
        };
        return true;
    }

    private static bool TryBrush(string value, out Brush brush)
    {
        brush = Brushes.Transparent;
        try
        {
            var converted = new BrushConverter().ConvertFromString(value);
            if (converted is not Brush parsed) return false;
            brush = parsed;
            return true;
        }
        catch (FormatException) { return false; }
        catch (NotSupportedException) { return false; }
    }

    private static bool IsLegalName(string id) =>
        !string.IsNullOrWhiteSpace(id)
        && (char.IsLetter(id[0]) || id[0] == '_')
        && id.All(c => char.IsLetterOrDigit(c) || c == '_');
}
