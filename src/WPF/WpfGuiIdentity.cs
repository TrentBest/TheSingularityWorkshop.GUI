using System.Windows;

namespace TheSingularityWorkshop.GUI.WPF;

/// <summary>
/// Preserves the platform-neutral GUI node identity on native WPF elements,
/// including identifiers that cannot be assigned to <see cref="FrameworkElement.Name"/>.
/// </summary>
public static class WpfGuiIdentity
{
    /// <summary>
    /// Attached property containing the exact <c>GuiNode.Id</c> used to create a WPF element.
    /// </summary>
    public static readonly DependencyProperty NodeIdProperty =
        DependencyProperty.RegisterAttached(
            "NodeId",
            typeof(string),
            typeof(WpfGuiIdentity),
            new FrameworkPropertyMetadata(null));

    /// <summary>Returns the original GUI model identifier stored on a WPF object.</summary>
    public static string? GetNodeId(DependencyObject element)
    {
        ArgumentNullException.ThrowIfNull(element);
        return (string?)element.GetValue(NodeIdProperty);
    }

    /// <summary>Sets the original GUI model identifier stored on a WPF object.</summary>
    public static void SetNodeId(DependencyObject element, string? value)
    {
        ArgumentNullException.ThrowIfNull(element);
        element.SetValue(NodeIdProperty, value);
    }
}
