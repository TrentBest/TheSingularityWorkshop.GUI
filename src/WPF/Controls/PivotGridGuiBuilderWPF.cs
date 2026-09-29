using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Controls;

/// <summary>
/// Cross-tabulates an arbitrary enumerable by public property names.
/// This is intentionally data-source agnostic and has no host/application dependency.
/// </summary>
public sealed class PivotGridGuiBuilderWPF : GuiPanelBuilderWPF<Grid, PivotGridGuiBuilderWPF>
{
    private readonly IEnumerable _dataSource;
    private readonly string _rowProperty;
    private readonly string _columnProperty;
    private readonly string? _valueProperty;

    public PivotGridGuiBuilderWPF(
        GuiConfiguration config,
        IEnumerable dataSource,
        string rowProperty,
        string columnProperty,
        string? valueProperty = null) : base(config)
    {
        _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        _rowProperty = rowProperty ?? throw new ArgumentNullException(nameof(rowProperty));
        _columnProperty = columnProperty ?? throw new ArgumentNullException(nameof(columnProperty));
        _valueProperty = valueProperty;
    }

    public override FrameworkElement Build()
    {
        Root.Children.Clear();
        Root.RowDefinitions.Clear();
        Root.ColumnDefinitions.Clear();

        var rows = new List<string>();
        var columns = new List<string>();
        var cells = new Dictionary<(string Row, string Column), string>();

        foreach (var item in _dataSource)
        {
            if (item == null) continue;
            var type = item.GetType();
            var row = Read(item, type, _rowProperty);
            var column = Read(item, type, _columnProperty);
            var value = _valueProperty == null ? item.ToString() ?? string.Empty : Read(item, type, _valueProperty);

            if (!rows.Contains(row, StringComparer.Ordinal)) rows.Add(row);
            if (!columns.Contains(column, StringComparer.Ordinal)) columns.Add(column);
            cells[(row, column)] = value;
        }

        Root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        foreach (var _ in rows) Root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        Root.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        foreach (var _ in columns) Root.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        AddText("Row", 0, 0, true);
        for (var c = 0; c < columns.Count; c++) AddText(columns[c], 0, c + 1, true);
        for (var r = 0; r < rows.Count; r++)
        {
            AddText(rows[r], r + 1, 0, true);
            for (var c = 0; c < columns.Count; c++)
                AddText(cells.GetValueOrDefault((rows[r], columns[c]), string.Empty), r + 1, c + 1, false);
        }

        return Root;
    }

    private void AddText(string text, int row, int column, bool header)
    {
        var block = new TextBlock
        {
            Text = text,
            FontWeight = header ? FontWeights.SemiBold : FontWeights.Normal,
            Margin = new Thickness(6)
        };
        Grid.SetRow(block, row);
        Grid.SetColumn(block, column);
        Root.Children.Add(block);
    }

    private static string Read(object item, Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException($"Property '{propertyName}' was not found on '{type.Name}'.");
        return property.GetValue(item)?.ToString() ?? string.Empty;
    }
}
