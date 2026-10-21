using System.Windows;

namespace TheSingularityWorkshop.GUI.WPF.Abstractions;

/// <summary>
/// A composable WPF component. It shares the native build-and-identity contract while
/// adding component-tree parenting and message forwarding.
/// </summary>
public interface IComponentWPF : ICoreBuilder<FrameworkElement>
{
    /// <summary>Stable identity of this component instance.</summary>
    string Id { get; }

    /// <summary>Assigns the logical component parent; this is distinct from WPF visual parenting.</summary>
    void SetParent(IComponentWPF? parent);

    /// <summary>Returns the current logical component parent.</summary>
    IComponentWPF? GetParent();

    /// <summary>Forwards a message to the component's logical parent.</summary>
    void Publish(string topic, object payload);

    /// <summary>Handles a message delivered by the component tree.</summary>
    void Receive(string topic, object payload);
}
