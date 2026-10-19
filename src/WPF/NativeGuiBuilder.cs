using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TheSingularityWorkshop.GUI.WPF.Abstractions
{
    /// <summary>
    /// Creates and configures one native WPF element of type <typeparamref name="T"/>.
    /// The element is allocated once by the builder and returned by each call to <see cref="Build"/>.
    /// </summary>
    /// <typeparam name="T">A concrete WPF framework element with a public parameterless constructor.</typeparam>
    public class NativeGuiBuilder<T> : IComponentBuilder where T : FrameworkElement, new()
    {
        /// <summary>The single element configured and returned by this builder.</summary>
        protected readonly T _element;
        private object? _parent;
        private readonly string _builderId = Guid.NewGuid().ToString();

        /// <summary>Gets or sets the host-defined parent reference; this does not attach the element to a WPF visual tree.</summary>
        public object? Parent { get => _parent; set => _parent = value; }

        /// <summary>Creates the element and applies system-theme defaults when it is a WPF <see cref="Control"/>.</summary>
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

        /// <summary>Sets the background when the element is a <see cref="Control"/>; other framework elements are unchanged.</summary>
        /// <param name="brush">Brush assigned to the control background.</param>
        /// <returns>This builder for fluent configuration.</returns>
        public NativeGuiBuilder<T> WithBackground(Brush brush)
        {
            if (_element is Control ctrl) ctrl.Background = brush;
            return this;
        }

        /// <summary>Sets the foreground when the element is a <see cref="Control"/>; other framework elements are unchanged.</summary>
        /// <param name="brush">Brush assigned to the control foreground.</param>
        /// <returns>This builder for fluent configuration.</returns>
        public NativeGuiBuilder<T> WithForeground(Brush brush)
        {
            if (_element is Control ctrl) ctrl.Foreground = brush;
            return this;
        }

        /// <summary>Sets the border brush and thickness when the element is a <see cref="Control"/>.</summary>
        /// <param name="brush">Brush used to draw the border.</param>
        /// <param name="thickness">Thickness applied to each border edge.</param>
        /// <returns>This builder for fluent configuration.</returns>
        public NativeGuiBuilder<T> WithBorder(Brush brush, Thickness thickness)
        {
            if (_element is Control ctrl)
            {
                ctrl.BorderBrush = brush;
                ctrl.BorderThickness = thickness;
            }
            return this;
        }

        /// <summary>Sets control padding when the element supports the WPF <see cref="Control.Padding"/> property.</summary>
        /// <param name="padding">Interior spacing around the control's content.</param>
        /// <returns>This builder for fluent configuration.</returns>
        public NativeGuiBuilder<T> WithPadding(Thickness padding)
        {
            if (_element is Control ctrl) ctrl.Padding = padding;
            return this;
        }

        /// <summary>Sets the element's outer layout margin.</summary>
        /// <param name="margin">Space outside the element on each edge.</param>
        /// <returns>This builder for fluent configuration.</returns>
        public NativeGuiBuilder<T> WithMargin(Thickness margin)
        {
            _element.Margin = margin;
            return this;
        }

        /// <summary>Sets explicit width and height values on the element.</summary>
        /// <param name="width">Requested element width, in WPF device-independent units.</param>
        /// <param name="height">Requested element height, in WPF device-independent units.</param>
        /// <returns>This builder for fluent configuration.</returns>
        public NativeGuiBuilder<T> WithSize(double width, double height)
        {
            _element.Width = width;
            _element.Height = height;
            return this;
        }

        /// <summary>Sets horizontal and vertical alignment within the parent layout.</summary>
        /// <param name="h">Horizontal alignment.</param>
        /// <param name="v">Vertical alignment.</param>
        /// <returns>This builder for fluent configuration.</returns>
        public NativeGuiBuilder<T> WithAlignment(HorizontalAlignment h, VerticalAlignment v)
        {
            _element.HorizontalAlignment = h;
            _element.VerticalAlignment = v;
            return this;
        }

        /// <summary>Runs caller-supplied configuration against the underlying element.</summary>
        /// <param name="action">Configuration callback invoked immediately with the element.</param>
        /// <returns>This builder for fluent configuration.</returns>
        /// <exception cref="ArgumentNullException">The callback is <see langword="null"/>.</exception>
        public NativeGuiBuilder<T> Configure(Action<T> action)
        {
            action(_element);
            return this;
        }

        /// <summary>Returns the element instance managed by this builder; it is not cloned.</summary>
        /// <returns>The configured WPF element.</returns>
        public FrameworkElement Build() => _element;

        /// <summary>Returns the stable identifier generated for this builder instance.</summary>
        /// <returns>A GUID string identifying this builder.</returns>
        public string GetBuilderId() => _builderId;
    }
}