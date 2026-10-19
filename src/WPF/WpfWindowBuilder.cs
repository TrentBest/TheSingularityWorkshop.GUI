using System;
using System.Windows;
using System.Windows.Media;

namespace TheSingularityWorkshop.GUI.WPF;

/// <summary>
/// Builds a native WPF window around content created by this adapter or supplied by the host.
/// Window lifetime is host-controlled: Build does not show the window or start an application.
/// </summary>
public sealed class WpfWindowBuilder
{
    private readonly Window _window = new()
    {
        Width = 700,
        Height = 500,
        WindowStartupLocation = WindowStartupLocation.CenterScreen,
        ResizeMode = ResizeMode.CanResize,
        WindowStyle = WindowStyle.SingleBorderWindow,
        ShowInTaskbar = true,
        Background = SystemColors.WindowBrush
    };

    /// <summary>Sets the window title.</summary>
    public WpfWindowBuilder WithTitle(string title)
    {
        _window.Title = title ?? throw new ArgumentNullException(nameof(title));
        return this;
    }

    /// <summary>Sets an explicit positive, finite client size.</summary>
    public WpfWindowBuilder WithSize(double width, double height)
    {
        if (!double.IsFinite(width) || width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive and finite.");
        if (!double.IsFinite(height) || height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive and finite.");

        _window.Width = width;
        _window.Height = height;
        _window.SizeToContent = SizeToContent.Manual;
        return this;
    }

    /// <summary>Sets the root content. The element must not already belong to another visual parent.</summary>
    public WpfWindowBuilder WithContent(FrameworkElement content)
    {
        _window.Content = content ?? throw new ArgumentNullException(nameof(content));
        return this;
    }

    /// <summary>Sets an owner window for modal or owned-window workflows.</summary>
    public WpfWindowBuilder OwnedBy(Window owner)
    {
        _window.Owner = owner ?? throw new ArgumentNullException(nameof(owner));
        return this;
    }

    public WpfWindowBuilder WithStartupLocation(WindowStartupLocation location)
    {
        _window.WindowStartupLocation = location;
        return this;
    }

    public WpfWindowBuilder WithResizeMode(ResizeMode mode)
    {
        _window.ResizeMode = mode;
        return this;
    }

    public WpfWindowBuilder WithWindowStyle(WindowStyle style)
    {
        _window.WindowStyle = style;
        return this;
    }

    public WpfWindowBuilder WithBackground(Brush background)
    {
        _window.Background = background ?? throw new ArgumentNullException(nameof(background));
        return this;
    }

    public WpfWindowBuilder WithIcon(ImageSource icon)
    {
        _window.Icon = icon ?? throw new ArgumentNullException(nameof(icon));
        return this;
    }

    public WpfWindowBuilder ShowInTaskbar(bool show = true)
    {
        _window.ShowInTaskbar = show;
        return this;
    }

    /// <summary>Returns the configured window without showing it.</summary>
    public Window Build() => _window;
}
