namespace TheSingularityWorkshop.GUI.WPF.Abstractions
{
    /// <summary>
    /// RLM-8: Defines the cross-platform intent for a UI component.
    /// The generic TProduct allows for platform-specific manifestation (e.g., FrameworkElement).
    /// </summary>
    public interface ICoreBuilder<out TProduct>
    {
        /// <summary>
        /// Manifests the component into its platform-specific representation.
        /// </summary>
        TProduct Build();

        /// <summary>
        /// Unique identifier for the builder instance (for SovereignLog traceability).
        /// </summary>
        string GetBuilderId();
    }
}