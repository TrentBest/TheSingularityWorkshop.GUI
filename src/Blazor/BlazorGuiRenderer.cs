using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Gui;

/// <summary>Renders a semantic GUI node tree as a Blazor render fragment.</summary>
public static class BlazorGuiRenderer
{
    private const int MaximumTreeDepth = 256;

    /// <summary>Creates a Blazor render fragment for a semantic GUI node without dispatching host commands.</summary>
    public static RenderFragment Render(GuiNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        ValidateUniqueIds(node);
        return builder => RenderNode(builder, node, commandDispatcher: null, controlDispatcher: null, controlEventReceiver: null);
    }

    /// <summary>
    /// Creates a Blazor render fragment and routes declared button commands to the supplied host dispatcher.
    /// The host remains responsible for command allow-listing and domain authorization.
    /// </summary>
    public static RenderFragment Render(GuiNode node, Action<string> commandDispatcher)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(commandDispatcher);
        ValidateUniqueIds(node);
        return builder => RenderNode(builder, node, commandDispatcher, controlDispatcher: null, controlEventReceiver: null);
    }

    /// <summary>
    /// Creates a render fragment that routes typed value changes to a host-owned command dispatcher.
    /// Text values, selected option IDs, and boolean values are passed as strings; the host validates
    /// and authorizes each command before applying domain changes.
    /// </summary>
    public static RenderFragment Render(GuiNode node, Action<string, string?> controlDispatcher)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(controlDispatcher);
        ValidateUniqueIds(node);
        return builder => RenderNode(builder, node, commandDispatcher: null, controlDispatcher, controlEventReceiver: controlDispatcher);
    }

    /// <summary>
    /// Creates a render fragment that uses the supplied component/event receiver for Blazor's
    /// event lifecycle, allowing the host component to re-render after it handles a value change.
    /// </summary>
    public static RenderFragment Render(
        GuiNode node,
        object eventReceiver,
        Action<string, string?> controlDispatcher)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(eventReceiver);
        ArgumentNullException.ThrowIfNull(controlDispatcher);
        ValidateUniqueIds(node);
        return builder => RenderNode(builder, node, commandDispatcher: null, controlDispatcher, eventReceiver);
    }

    private static void ValidateUniqueIds(GuiNode root)
    {
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var pending = new Stack<(GuiNode Node, int Depth)>();
        pending.Push((root, 0));

        while (pending.Count > 0)
        {
            var (node, depth) = pending.Pop();
            if (depth > MaximumTreeDepth)
                throw new InvalidOperationException(
                    $"GUI tree exceeds the Blazor renderer's maximum depth ({MaximumTreeDepth}).");

            if (!ids.Add(node.Id))
                throw new ArgumentException(
                    $"GUI node id '{node.Id}' is duplicated. Node ids must be unique within a rendered tree.",
                    nameof(root));

            foreach (var child in node.Children)
                pending.Push((child, depth + 1));
        }
    }

    private static void RenderNode(
        RenderTreeBuilder builder,
        GuiNode node,
        Action<string>? commandDispatcher,
        Action<string, string?>? controlDispatcher,
        object? controlEventReceiver)
    {
        if (node.Control is not null)
        {
            RenderControl(builder, node, controlDispatcher, controlEventReceiver);
            return;
        }

        // Sequence numbers identify source-code positions, not runtime iteration
        // order. Keep each conditional/loop at a stable sequence position so
        // optional source/text and changing property counts do not shift later frames.
        builder.OpenElement(0, ResolveTag(node.Kind));
        builder.AddAttribute(1, "id", node.Id);
        if (!string.IsNullOrWhiteSpace(node.Source))
            builder.AddAttribute(2, "src", node.Source);
        foreach (var property in node.Properties)
        {
            // Node identity is owned by GuiNode.Id, never by an arbitrary property.
            // Host commands are never serialized as ordinary DOM attributes.
            if (string.Equals(property.Key, "id", StringComparison.OrdinalIgnoreCase)
                || string.Equals(property.Key, "onClickCommand", StringComparison.Ordinal))
                continue;

            builder.AddAttribute(3, property.Key, property.Value);
        }

        if (node.Kind == "Button"
            && commandDispatcher is not null
            && node.Properties.TryGetValue("onClickCommand", out var commandId)
            && !string.IsNullOrWhiteSpace(commandId))
        {
            builder.AddAttribute(5, "onclick",
                EventCallback.Factory.Create<MouseEventArgs>(
                    commandDispatcher,
                    _ => commandDispatcher(commandId)));
        }

        if (node.Text is not null)
            builder.AddContent(4, node.Text);

        foreach (var child in node.Children)
            RenderNode(builder, child, commandDispatcher, controlDispatcher, controlEventReceiver);
        builder.CloseElement();
    }

    private static void RenderControl(
        RenderTreeBuilder builder,
        GuiNode node,
        Action<string, string?>? dispatcher,
        object? eventReceiver)
    {
        switch (node.Control)
        {
            case TextInputDefinition text:
                builder.OpenElement(0, "input");
                builder.AddAttribute(1, "id", node.Id);
                builder.AddAttribute(2, "type", "text");
                builder.AddAttribute(3, "value", text.Value);
                if (!string.IsNullOrWhiteSpace(node.Text)) builder.AddAttribute(4, "aria-label", node.Text);
                if (text.Placeholder is not null) builder.AddAttribute(5, "placeholder", text.Placeholder);
                if (text.IsReadOnly || dispatcher is null) builder.AddAttribute(6, "readonly", true);
                AddValidationAttributes(builder, text.ValidationMessage);
                if (dispatcher is not null)
                    builder.AddAttribute(9, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(
                        eventReceiver ?? dispatcher, args => dispatcher(text.ChangeCommandId, args.Value?.ToString())));
                builder.CloseElement();
                break;

            case SelectDefinition select:
                builder.OpenElement(0, "select");
                builder.AddAttribute(1, "id", node.Id);
                if (!string.IsNullOrWhiteSpace(node.Text)) builder.AddAttribute(2, "aria-label", node.Text);
                builder.AddAttribute(3, "value", select.SelectedOptionId);
                if (select.IsReadOnly || dispatcher is null) builder.AddAttribute(4, "disabled", true);
                AddValidationAttributes(builder, select.ValidationMessage);
                if (dispatcher is not null)
                    builder.AddAttribute(9, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(
                        dispatcher, args => dispatcher(select.ChangeCommandId, args.Value?.ToString())));
                foreach (var option in select.Options)
                {
                    builder.OpenElement(10, "option");
                    builder.AddAttribute(11, "value", option.Id);
                    builder.AddAttribute(12, "selected",
                        string.Equals(option.Id, select.SelectedOptionId, StringComparison.Ordinal));
                    builder.AddContent(13, option.Label);
                    builder.CloseElement();
                }
                builder.CloseElement();
                break;

            case ToggleDefinition toggle:
                builder.OpenElement(0, "input");
                builder.AddAttribute(1, "id", node.Id);
                builder.AddAttribute(2, "type", "checkbox");
                if (!string.IsNullOrWhiteSpace(node.Text)) builder.AddAttribute(3, "aria-label", node.Text);
                builder.AddAttribute(4, "checked", toggle.IsChecked);
                if (toggle.IsReadOnly || dispatcher is null) builder.AddAttribute(5, "disabled", true);
                AddValidationAttributes(builder, toggle.ValidationMessage);
                if (dispatcher is not null)
                    builder.AddAttribute(9, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(
                        dispatcher, args => dispatcher(toggle.ChangeCommandId,
                            bool.TryParse(args.Value?.ToString(), out var value) ? value.ToString() : null)));
                builder.CloseElement();
                break;
        }
    }

    private static void AddValidationAttributes(RenderTreeBuilder builder, string? validationMessage)
    {
        if (string.IsNullOrWhiteSpace(validationMessage))
            return;
        builder.AddAttribute(7, "aria-invalid", "true");
        builder.AddAttribute(8, "title", validationMessage);
    }

    private static string ResolveTag(string kind) => kind switch
    {
        "Panel" => "div",
        "Button" => "button",
        "Image" => "img",
        "Text" => "span",
        "Warning" => "aside",
        _ => "div"
    };
}
