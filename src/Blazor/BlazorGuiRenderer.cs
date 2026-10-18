using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using TheSingularityWorkshop.Workshop.Gui;

namespace TheSingularityWorkshop.Workshop.Gui;

public static class BlazorGuiRenderer
{
    private const int MaximumTreeDepth = 256;

    public static RenderFragment Render(GuiNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        ValidateUniqueIds(node);
        return builder => RenderNode(builder, node);
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

    private static void RenderNode(RenderTreeBuilder builder, GuiNode node)
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
            if (string.Equals(property.Key, "id", StringComparison.OrdinalIgnoreCase))
                continue;

            builder.AddAttribute(3, property.Key, property.Value);
        }
        if (node.Text is not null)
            builder.AddContent(4, node.Text);
        foreach (var child in node.Children)
            RenderNode(builder, child);
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
