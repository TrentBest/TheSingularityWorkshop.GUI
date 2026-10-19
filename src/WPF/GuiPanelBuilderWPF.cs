using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TheSingularityWorkshop.GUI.WPF.Abstractions;

/// <summary>
/// Base implementation for WPF panel builders. It queues child-creation actions until <see cref="Build"/>
/// and uses a self-referential generic type so fluent calls return the concrete builder subtype.
/// </summary>
/// <typeparam name="TPanel">The WPF panel created and populated by this builder.</typeparam>
/// <typeparam name="TBuilder">The concrete builder type returned by fluent methods.</typeparam>
public abstract class GuiPanelBuilderWPF<TPanel, TBuilder> : IComponentWPF
    where TPanel : Panel, new()
    where TBuilder : GuiPanelBuilderWPF<TPanel, TBuilder>
{
    /// <summary>The panel instance populated by this builder and returned from <see cref="Build"/>.</summary>
    public TPanel Root { get; } = new TPanel();

    private IComponentWPF? _parent;
    /// <summary>Gets the stable identifier assigned to this component instance.</summary>
    public string Id { get; } = Guid.NewGuid().ToString();

    /// <summary>Deferred actions that create and attach the panel's direct children during <see cref="Build"/>.</summary>
    protected readonly List<Action> _definitionRoster = new();

    // Type-safe cast loop that returns the actual terminal subclass instance
    /// <summary>Returns this instance as the concrete builder type for fluent chaining.</summary>
    protected TBuilder Actual => (TBuilder)this;

    /// <summary>
    /// Initializes the panel builder and imports the supported initial element definitions from configuration.
    /// Currently, configured TextBox elements are initialized from the DefaultText property.
    /// </summary>
    /// <param name="config">Initial element configuration; a null configuration produces an empty roster.</param>
    protected GuiPanelBuilderWPF(GuiConfiguration config)
    {
        if (config?.Elements != null)
        {
            foreach (var def in config.Elements)
            {
                if (def.TypeName == "TextBox")
                {
                    Add<TextBox>(tb => {
                        tb.Text = def.Properties.GetValueOrDefault("DefaultText", "");
                        tb.Margin = new Thickness(5);
                    });
                }
            }
        }
    }

    /// <summary>Associates this component with its logical parent for component messaging.</summary>
    /// <param name="parent">Parent component, or null to detach the logical relationship.</param>
    public void SetParent(IComponentWPF? parent) => _parent = parent;

    /// <summary>Returns the logical parent previously assigned through <see cref="SetParent"/>.</summary>
    /// <returns>The parent component, or null when this component has no parent.</returns>
    public IComponentWPF? GetParent() => _parent;

    /// <summary>Forwards a published message to the logical parent when one is assigned.</summary>
    /// <param name="topic">Message topic to publish.</param>
    /// <param name="payload">Message payload forwarded unchanged.</param>
    public virtual void Publish(string topic, object payload) => _parent?.Publish(topic, payload);

    /// <summary>Receives a message from a component or host; the base implementation intentionally does nothing.</summary>
    /// <param name="topic">Message topic.</param>
    /// <param name="payload">Message payload.</param>
    public virtual void Receive(string topic, object payload) { }

    /// <summary>Queues creation of a new direct child and returns the concrete builder for fluent chaining.</summary>
    /// <typeparam name="T">A UI element type with a public parameterless constructor.</typeparam>
    /// <param name="config">Optional-at-runtime callback used to configure the new element before it is attached.</param>
    /// <returns>This builder's concrete subtype.</returns>
    public TBuilder Add<T>(Action<T> config) where T : UIElement, new()
    {
        _definitionRoster.Add(() => {
            var el = new T();
            config?.Invoke(el);
            Root.Children.Add(el);
        });
        return Actual;
    }

    /// <summary>Queues a child component's build operation and attaches its resulting element to this panel.</summary>
    /// <param name="component">Component to parent and embed; null is ignored.</param>
    /// <returns>This builder's concrete subtype.</returns>
    public TBuilder AddComponent(IComponentWPF component)
    {
        if (component != null)
        {
            component.SetParent(this);
            _definitionRoster.Add(() => Root.Children.Add(component.Build()));
        }
        return Actual;
    }

    /// <summary>Recreates the panel's direct children from the queued definitions and returns the root panel.</summary>
    /// <returns>The root panel instance owned by this builder.</returns>
    public virtual FrameworkElement Build()
    {
        Root.Children.Clear();
        foreach (var action in _definitionRoster)
        {
            action.Invoke();
        }
        return Root;
    }

    /// <summary>Captures a limited configuration snapshot of currently built direct TextBox children.</summary>
    /// <remarks>The snapshot stores each TextBox's current text under the <c>Text</c> property; other child types are omitted.</remarks>
    /// <returns>A new configuration containing the supported captured elements.</returns>
    public GuiConfiguration Capture()
    {
        var config = new GuiConfiguration();
        foreach (var child in Root.Children)
        {
            if (child is TextBox tb)
            {
                config.Elements.Add(new ElementDefinition
                {
                    TypeName = "TextBox",
                    Properties = new() { { "Text", tb.Text } }
                });
            }
        }
        return config;
    }
}