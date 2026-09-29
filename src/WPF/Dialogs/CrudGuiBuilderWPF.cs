using System.Collections;
using System.Windows;
using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Dialogs;

/// <summary>Reusable collection editor surface with explicit create/update/delete operations.</summary>
public sealed class CrudGuiBuilderWPF<T> : GuiPanelBuilderWPF<DockPanel, CrudGuiBuilderWPF<T>>
{
    private readonly IList<T> _items;
    private readonly Func<T, string> _display;
    private readonly ListBox _list = new();

    public CrudGuiBuilderWPF(GuiConfiguration config, IList<T> items, Func<T, string>? display = null) : base(config)
    {
        _items = items ?? throw new ArgumentNullException(nameof(items));
        _display = display ?? (item => item?.ToString() ?? string.Empty);
    }

    public Action? CreateRequested { get; set; }
    public Action<T>? EditRequested { get; set; }
    public Action<T>? DeleteRequested { get; set; }

    public override FrameworkElement Build()
    {
        Root.Children.Clear();
        _list.ItemsSource = null;
        _list.ItemsSource = _items.Select(_display).ToList();

        Root.Children.Add(_list);

        var buttons = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 0) };
        var create = new Button { Content = "Create", Margin = new Thickness(0, 0, 5, 0) };
        var edit = new Button { Content = "Edit", Margin = new Thickness(0, 0, 5, 0) };
        var delete = new Button { Content = "Delete" };

        create.Click += (_, _) => CreateRequested?.Invoke();
        edit.Click += (_, _) =>
        {
            if (_list.SelectedIndex >= 0 && _list.SelectedIndex < _items.Count)
                EditRequested?.Invoke(_items[_list.SelectedIndex]);
        };
        delete.Click += (_, _) =>
        {
            if (_list.SelectedIndex >= 0 && _list.SelectedIndex < _items.Count)
                DeleteRequested?.Invoke(_items[_list.SelectedIndex]);
        };

        buttons.Children.Add(create);
        buttons.Children.Add(edit);
        buttons.Children.Add(delete);
        Root.Children.Add(buttons);
        return Root;
    }
}
