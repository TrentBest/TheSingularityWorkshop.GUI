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
        return builder => RenderNode(builder, node, commandDispatcher: null);
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
        return builder => RenderNode(builder, node, commandDispatcher);
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
        Action<string>? commandDispatcher)
    {
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
            RenderNode(builder, child, commandDispatcher);
        builder.CloseElement();
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
