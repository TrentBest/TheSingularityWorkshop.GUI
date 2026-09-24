using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TheSingularityWorkshop.GUI.WPF.Abstractions
{
    public class NativeGuiBuilder<T> : IComponentBuilder where T : FrameworkElement, new()
    {
        protected readonly T _element;
        private object? _parent;
        private readonly string _builderId = Guid.NewGuid().ToString();

        public object? Parent { get => _parent; set => _parent = value; }

        public NativeGuiBuilder()
        {
            _element = new T();
            ApplyDefaultNativeTheme(_element);
        }

        private void ApplyDefaultNativeTheme(FrameworkElement el)
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

        public NativeGuiBuilder<T> WithBackground(Brush brush)
        {
            if (_element is Control ctrl) ctrl.Background = brush;
            return this;
        }

        public NativeGuiBuilder<T> WithForeground(Brush brush)
        {
            if (_element is Control ctrl) ctrl.Foreground = brush;
            return this;
        }

        public NativeGuiBuilder<T> WithBorder(Brush brush, Thickness thickness)
        {
            if (_element is Control ctrl)
            {
                ctrl.BorderBrush = brush;
                ctrl.BorderThickness = thickness;
            }
            return this;
        }

        public NativeGuiBuilder<T> WithPadding(Thickness padding)
        {
            if (_element is Control ctrl) ctrl.Padding = padding;
            return this;
        }

        public NativeGuiBuilder<T> WithMargin(Thickness margin)
        {
            _element.Margin = margin;
            return this;
        }

        public NativeGuiBuilder<T> WithSize(double width, double height)
        {
            _element.Width = width;
            _element.Height = height;
            return this;
        }

        public NativeGuiBuilder<T> WithAlignment(HorizontalAlignment h, VerticalAlignment v)
        {
            _element.HorizontalAlignment = h;
            _element.VerticalAlignment = v;
            return this;
        }

        public NativeGuiBuilder<T> Configure(Action<T> action)
        {
            action(_element);
            return this;
        }

        public FrameworkElement Build() => _element;
        public string GetBuilderId() => _builderId;
    }
}