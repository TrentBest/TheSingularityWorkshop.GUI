using System.Windows;

namespace TheSingularityWorkshop.GUI.WPF.Abstractions
{
    /// <summary>
    /// Foundational contract for Sovereign WPF Builders.
    /// Tracks the Builder Hierarchy to enable automatic UI Chrome (Root Stance) logic.
    /// </summary>
    public interface IComponentBuilder : ICoreBuilder<FrameworkElement>
    {
        /// <summary>
        /// The architectural parent in the builder chain. 
        /// If null, this component manifests as a Root Window.
        /// </summary>
        object? Parent { get; set; }
    }
}