using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TheSingularityWorkshop.GUI.WPF.Abstractions;

/// <summary>
/// Full-featured WPF root builder. Provides the mature window/chrome and
/// interaction surface without carrying any application or Revit dependencies.
/// </summary>
public class GuiBuilderWPF : IComponentBuilder
{
    private readonly Border _container;
    private readonly StackPanel _content;
    private readonly List<Action> _definitions = new();
    private readonly Dictionary<string, Action<object?>> _bus = new();

    private double _width = 700;
    private double _height = 700;
    private double? _left;
    private double? _top;
    private bool _showInTaskbar;
    private bool _showActivated = true;
    private bool _allowsTransparency;
    private WindowStyle _windowStyle = WindowStyle.SingleBorderWindow;
    private WindowStartupLocation _startupLocation = WindowStartupLocation.CenterScreen;
    private ResizeMode _resizeMode = ResizeMode.CanResize;
    private SizeToContent _sizeToContent = SizeToContent.Manual;
    private Brush _windowBackground = SystemColors.WindowBrush;
    private ImageSource? _icon;
    private Window? _owner;
    private IntPtr _ownerHandle;
    private FrameworkElement? _explicitContent;
    private bool _showWindowControls = true;

    public GuiBuilderWPF(string name)
    {
        Name = string.IsNullOrWhiteSpace(name) ? "GUI" : name;
        Id = $"GUI_{Guid.NewGuid():N}";
        _content = new StackPanel { Margin = new Thickness(15) };
        _container = new Border
        {
            CornerRadius = new CornerRadius(7),
            BorderThickness = new Thickness(1),
            BorderBrush = SystemColors.ActiveBorderBrush,
            Background = SystemColors.WindowBrush,
            Child = _content
        };
    }

    public string Id { get; }
    public string Name { get; set; }
    public object? Parent { get; set; }
    public FrameworkElement ContentRoot => _container;
    public UIElementCollection Children => _content.Children;

    public static Action<string, Brush>? StatusReporter { get; set; }

    public static void ReportStatus(string message, Brush? brush = null) =>
        StatusReporter?.Invoke(message, brush ?? SystemColors.GrayTextBrush);

    public GuiMode Mode { get; set; } = GuiMode.Reporting;
    public bool IsUnlocked => Mode == GuiMode.Interactable;

    public GuiBuilderWPF WithSize(double width, double height)
    {
        if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException();
        _width = width;
        _height = height;
        return this;
    }

    public GuiBuilderWPF WithWidth(double width) { _width = width; return this; }
    public GuiBuilderWPF WithHeight(double height) { _height = height; return this; }
    public GuiBuilderWPF AtPosition(double left, double top)
    {
        _left = left;
        _top = top;
        _startupLocation = WindowStartupLocation.Manual;
        return this;
    }

    public GuiBuilderWPF WithBackground(Brush brush)
    {
        _windowBackground = brush ?? throw new ArgumentNullException(nameof(brush));
        return this;
    }

    public GuiBuilderWPF WithTransparency(bool enabled = true)
    {
        _allowsTransparency = enabled;
        if (enabled) _windowStyle = WindowStyle.None;
        return this;
    }

    public GuiBuilderWPF SetStyle(WindowStyle style) { _windowStyle = style; return this; }
    public GuiBuilderWPF SetStartupLocation(WindowStartupLocation location) { _startupLocation = location; return this; }
    public GuiBuilderWPF SetResizeMode(ResizeMode mode) { _resizeMode = mode; return this; }
    public GuiBuilderWPF AutoSize(SizeToContent mode) { _sizeToContent = mode; return this; }
    public GuiBuilderWPF ShowInTaskbar(bool show = true) { _showInTaskbar = show; return this; }
    public GuiBuilderWPF ShowActivated(bool show = true) { _showActivated = show; return this; }
    public GuiBuilderWPF WithIcon(ImageSource icon) { _icon = icon; return this; }
    public GuiBuilderWPF OwnedBy(Window owner) { _owner = owner; return this; }
    public GuiBuilderWPF OwnedBy(IntPtr handle) { _ownerHandle = handle; return this; }
    public GuiBuilderWPF WithWindowControls(bool show = true) { _showWindowControls = show; return this; }

    public GuiBuilderWPF WithExplicitContent(FrameworkElement content)
    {
        _explicitContent = content ?? throw new ArgumentNullException(nameof(content));
        return this;
    }

    public GuiBuilderWPF Add<T>(Action<T>? configure = null) where T : UIElement, new()
    {
        _definitions.Add(() =>
        {
            var element = new T();
            configure?.Invoke(element);
            _content.Children.Add(element);
        });
        return this;
    }

    public GuiBuilderWPF AddElement(UIElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        _definitions.Add(() => _content.Children.Add(element));
        return this;
    }

    public GuiBuilderWPF AddField(string label, string value)
    {
        _definitions.Add(() =>
        {
            var panel = new StackPanel { Margin = new Thickness(0, 5, 0, 5) };
            panel.Children.Add(new TextBlock { Text = label.ToUpperInvariant(), FontSize = 9, Foreground = SystemColors.GrayTextBrush });
            panel.Children.Add(new TextBlock { Text = value, FontSize = 13, TextWrapping = TextWrapping.Wrap, Foreground = SystemColors.ControlTextBrush });
            _content.Children.Add(panel);
        });
        return this;
    }

    public GuiBuilderWPF AddSeparator()
    {
        _definitions.Add(() => _content.Children.Add(new Separator { Margin = new Thickness(0, 8, 0, 8) }));
        return this;
    }

    public GuiBuilderWPF Subscribe(string topic, Action<object?> handler)
    {
        ArgumentNullException.ThrowIfNull(topic);
        ArgumentNullException.ThrowIfNull(handler);
        _bus[topic] = handler;
        return this;
    }

    public void Publish(string topic, object payload)
    {
        if (_bus.TryGetValue(topic, out var handler))
        {
            handler(payload);
            return;
        }

        if (Parent is GuiBuilderWPF parent)
        {
            parent.Publish(topic, payload);
            return;
        }

        if (Parent is IComponentWPF component)
        {
            component.Publish(topic, payload);
        }
    }

    public void ReceiveBusPayload(string topic, object? payload) => Receive(topic, payload);
    public void Receive(string topic, object payload)
    {
        if (_bus.TryGetValue(topic, out var handler)) handler(payload);
    }

    public void ToggleMode() => Mode = IsUnlocked ? GuiMode.Reporting : GuiMode.Interactable;

    public virtual FrameworkElement Build()
    {
        if (_explicitContent != null)
        {
            _container.Child = _explicitContent;
            return _container;
        }

        _content.Children.Clear();
        foreach (var definition in _definitions) definition();
        return _container;
    }

    public Window BuildAsWindow()
    {
        Build();
        var window = new Window
        {
            Title = Name,
            Content = _container,
            Width = _width,
            Height = _height,
            AllowsTransparency = _allowsTransparency,
            WindowStyle = _windowStyle,
            WindowStartupLocation = _startupLocation,
            ResizeMode = _resizeMode,
            SizeToContent = _sizeToContent,
            ShowInTaskbar = _showInTaskbar,
            ShowActivated = _showActivated,
            Background = _windowBackground,
            Icon = _icon
        };

        if (_left.HasValue) window.Left = _left.Value;
        if (_top.HasValue) window.Top = _top.Value;
        if (_owner != null) window.Owner = _owner;
        if (_ownerHandle != IntPtr.Zero)
            new System.Windows.Interop.WindowInteropHelper(window) { Owner = _ownerHandle };

        if (_showWindowControls)
            window.MouseLeftButtonDown += (_, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
                    window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            };

        return window;
    }

    public void ShowAsDialog() => BuildAsWindow().ShowDialog();
    public string GetBuilderId() => Id;
}
