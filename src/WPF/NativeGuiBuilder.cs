using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TheSingularityWorkshop.GUI.WPF.Abstractions;

/// <summary>
/// CRTP base for native WPF element builders. Fluent operations retain the concrete builder type,
/// so specialized builders can extend the chain without losing their own methods.
/// </summary>
/// <typeparam name="T">The native WPF element type.</typeparam>
/// <typeparam name="TBuilder">The concrete builder type returned from fluent operations.</typeparam>
public class NativeGuiBuilder<T, TBuilder> : IComponentBuilder
    where T : FrameworkElement, new()
    where TBuilder : NativeGuiBuilder<T, TBuilder>
{
    /// <summary>The single element configured and returned by this builder.</summary>
    protected readonly T _element;
    private object? _parent;
    private readonly string _builderId = Guid.NewGuid().ToString();

    /// <summary>Gets or sets the host-defined parent reference; this does not attach the element to a WPF visual tree.</summary>
    public object? Parent { get => _parent; set => _parent = value; }

    /// <summary>Creates the element and applies system-theme defaults when it is a WPF control.</summary>
    public NativeGuiBuilder()
    {
        _element = new T();
        ApplyDefaultNativeTheme(_element);
    }

    /// <summary>Returns this instance as its concrete fluent builder type.</summary>
    protected TBuilder Actual => (TBuilder)this;

    private static void ApplyDefaultNativeTheme(FrameworkElement el)
    {
        if (el is Control ctrl)
        {
            ctrl.Background = SystemColors.ControlBrush;
            ctrl.Foreground = SystemColors.ControlTextBrush;
            ctrl.BorderBrush = SystemColors.ActiveBorderBrush;
            ctrl.BorderThickness = new Thickness(1);
            ctrl.FontFamily = SystemFonts.MessageFontFamily;
        }
    }

    /// <summary>Sets the background when the element is a Control; other framework elements are unchanged.</summary>
    public TBuilder WithBackground(Brush brush)
    {
        ArgumentNullException.ThrowIfNull(brush);
        if (_element is Control ctrl) ctrl.Background = brush;
        return Actual;
    }

    /// <summary>Sets the foreground when the element is a Control; other framework elements are unchanged.</summary>
    public TBuilder WithForeground(Brush brush)
    {
        ArgumentNullException.ThrowIfNull(brush);
        if (_element is Control ctrl) ctrl.Foreground = brush;
        return Actual;
    }

    /// <summary>Sets the border brush and thickness when the element is a Control.</summary>
    public TBuilder WithBorder(Brush brush, Thickness thickness)
    {
        ArgumentNullException.ThrowIfNull(brush);
        if (_element is Control ctrl)
        {
            ctrl.BorderBrush = brush;
            ctrl.BorderThickness = thickness;
        }
        return Actual;
    }

    /// <summary>Sets control padding when the element supports the WPF Control.Padding property.</summary>
    public TBuilder WithPadding(Thickness padding)
    {
        if (_element is Control ctrl) ctrl.Padding = padding;
        return Actual;
    }

    /// <summary>Sets the element's outer layout margin.</summary>
    public TBuilder WithMargin(Thickness margin)
    {
        _element.Margin = margin;
        return Actual;
    }

    /// <summary>Sets explicit width and height values on the element.</summary>
    public TBuilder WithSize(double width, double height)
    {
        if (!double.IsFinite(width) || width < 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be finite and non-negative.");
        if (!double.IsFinite(height) || height < 0)
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be finite and non-negative.");
        _element.Width = width;
        _element.Height = height;
        return Actual;
    }

    /// <summary>Sets horizontal and vertical alignment within the parent layout.</summary>
    public TBuilder WithAlignment(HorizontalAlignment h, VerticalAlignment v)
    {
        _element.HorizontalAlignment = h;
        _element.VerticalAlignment = v;
        return Actual;
    }

    /// <summary>Runs caller-supplied configuration against the underlying element.</summary>
    public TBuilder Configure(Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        action(_element);
        return Actual;
    }

    /// <summary>Returns the element instance managed by this builder; it is not cloned.</summary>
    public FrameworkElement Build() => _element;

    /// <summary>Returns the stable identifier generated for this builder instance.</summary>
    public string GetBuilderId() => _builderId;
}

/// <summary>
/// Convenient fluent builder for one native WPF element. Derive from
/// <see cref="NativeGuiBuilder{T, TBuilder}"/> when adding specialized fluent methods.
/// </summary>
/// <typeparam name="T">A concrete WPF framework element with a public parameterless constructor.</typeparam>
public class NativeGuiBuilder<T> : NativeGuiBuilder<T, NativeGuiBuilder<T>>
    where T : FrameworkElement, new()
{
}
