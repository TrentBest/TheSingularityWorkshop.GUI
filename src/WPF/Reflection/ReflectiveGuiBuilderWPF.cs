using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Reflection;

/// <summary>
/// Builds a diagnostic/editing surface from public properties without knowing the source model's type.
/// </summary>
public sealed class ReflectiveGuiBuilderWPF : GuiPanelBuilderWPF<StackPanel, ReflectiveGuiBuilderWPF>
{
    private readonly object?[] _items;
    private readonly Dictionary<Type, Func<PropertyInfo, object, FrameworkElement>> _renderers = new();

    public ReflectiveGuiBuilderWPF(GuiConfiguration config, params object?[] items) : base(config)
    {
        _items = items ?? Array.Empty<object?>();
    }

    public ReflectiveGuiBuilderWPF Register<T>(Func<PropertyInfo, object, FrameworkElement> renderer)
    {
        ArgumentNullException.ThrowIfNull(renderer);
        _renderers[typeof(T)] = renderer;
        return this;
    }

    public override FrameworkElement Build()
    {
        Root.Children.Clear();

        foreach (var item in _items)
        {
            if (item == null) continue;

            foreach (var property in item.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!property.CanRead) continue;
                var editor = CreateEditor(item, property);
                editor.Margin = new Thickness(0, 3, 0, 3);
                Root.Children.Add(editor);
            }
        }

        return Root;
    }

    private FrameworkElement CreateEditor(object owner, PropertyInfo property)
    {
        var value = property.GetValue(owner);

        if (value != null && _renderers.TryGetValue(value.GetType(), out var renderer))
            return renderer(property, value);

        if (property.PropertyType == typeof(bool))
            return Labeled(property.Name, new CheckBox
            {
                IsChecked = value is bool boolean && boolean
            });

        if (property.PropertyType.IsEnum)
        {
            var combo = new ComboBox
            {
                ItemsSource = Enum.GetValues(property.PropertyType),
                SelectedItem = value
            };

            if (property.CanWrite)
            {
                combo.SelectionChanged += (_, _) =>
                {
                    if (combo.SelectedItem != null)
                        property.SetValue(owner, combo.SelectedItem);
                };
            }

            return Labeled(property.Name, combo);
        }

        if (property.PropertyType.IsPrimitive ||
            property.PropertyType == typeof(decimal) ||
            property.PropertyType == typeof(string))
        {
            var box = new TextBox { Text = value?.ToString() ?? string.Empty };
            if (property.CanWrite)
            {
                box.LostFocus += (_, _) =>
                {
                    try
                    {
                        var converted = property.PropertyType == typeof(string)
                            ? box.Text
                            : Convert.ChangeType(box.Text, property.PropertyType, System.Globalization.CultureInfo.InvariantCulture);
                        property.SetValue(owner, converted);
                    }
                    catch
                    {
                        box.Text = property.GetValue(owner)?.ToString() ?? string.Empty;
                    }
                };
            }
            return Labeled(property.Name, box);
        }

        if (value is System.Collections.IEnumerable enumerable && value is not string)
            return Labeled(property.Name, new ListBox { ItemsSource = enumerable });

        return Labeled(property.Name, new TextBlock { Text = value?.ToString() ?? "null" });
    }

    private static FrameworkElement Labeled(string label, FrameworkElement control)
    {
        var panel = new StackPanel();
        panel.Children.Add(new TextBlock { Text = label, FontSize = 10 });
        panel.Children.Add(control);
        return panel;
    }
}
