using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>Base contract for a semantic control whose changes are routed to a host-owned command.</summary>
public abstract class GuiControlDefinition
{
    protected GuiControlDefinition(string changeCommandId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(changeCommandId);
        ChangeCommandId = changeCommandId;
    }

    /// <summary>Gets the stable command identity the host may authorize and dispatch.</summary>
    public string ChangeCommandId { get; }
}

/// <summary>Describes a text-entry control without depending on a UI framework.</summary>
public sealed class TextInputDefinition : GuiControlDefinition
{
    public TextInputDefinition(
        string value,
        string changeCommandId,
        string? placeholder = null,
        string? validationMessage = null,
        bool isReadOnly = false) : base(changeCommandId)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Placeholder = placeholder;
        ValidationMessage = validationMessage;
        IsReadOnly = isReadOnly;
    }

    public string Value { get; }
    public string? Placeholder { get; }
    public string? ValidationMessage { get; }
    public bool IsReadOnly { get; }
}

/// <summary>A stable option identity and its user-facing label.</summary>
public sealed class GuiSelectOption
{
    public GuiSelectOption(string id, string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(label);
        Id = id;
        Label = label;
    }

    public string Id { get; }
    public string Label { get; }
}

/// <summary>Describes a single-selection control using stable option IDs.</summary>
public sealed class SelectDefinition : GuiControlDefinition
{
    private readonly ReadOnlyCollection<GuiSelectOption> _options;

    public SelectDefinition(
        IEnumerable<GuiSelectOption> options,
        string selectedOptionId,
        string changeCommandId,
        string? validationMessage = null,
        bool isReadOnly = false) : base(changeCommandId)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(selectedOptionId);

        var snapshot = options.ToList();
        if (snapshot.Count == 0)
            throw new ArgumentException("A select control requires at least one option.", nameof(options));
        if (snapshot.Any(option => option is null))
            throw new ArgumentException("Select options cannot contain null entries.", nameof(options));
        if (snapshot.Select(option => option.Id).Distinct(StringComparer.Ordinal).Count() != snapshot.Count)
            throw new ArgumentException("Select option IDs must be unique.", nameof(options));
        if (!snapshot.Any(option => string.Equals(option.Id, selectedOptionId, StringComparison.Ordinal)))
            throw new ArgumentException("The selected option ID must identify one of the supplied options.", nameof(selectedOptionId));

        _options = new ReadOnlyCollection<GuiSelectOption>(snapshot);
        SelectedOptionId = selectedOptionId;
        ValidationMessage = validationMessage;
        IsReadOnly = isReadOnly;
    }

    public IReadOnlyList<GuiSelectOption> Options => _options;
    public string SelectedOptionId { get; }
    public string? ValidationMessage { get; }
    public bool IsReadOnly { get; }
}

/// <summary>Describes a boolean toggle control without depending on a UI framework.</summary>
public sealed class ToggleDefinition : GuiControlDefinition
{
    public ToggleDefinition(bool isChecked, string changeCommandId, string? validationMessage = null, bool isReadOnly = false)
        : base(changeCommandId)
    {
        IsChecked = isChecked;
        ValidationMessage = validationMessage;
        IsReadOnly = isReadOnly;
    }

    public bool IsChecked { get; }
    public string? ValidationMessage { get; }
    public bool IsReadOnly { get; }
}
