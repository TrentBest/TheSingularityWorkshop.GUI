using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TheSingularityWorkshop.GUI.WPF.Abstractions;

/// <summary>
/// Core base implementation for all layout panel builders.
/// Employs CRTP to prevent type-truncation across fluent method chains.
/// </summary>
public abstract class GuiPanelBuilderWPF<TPanel, TBuilder> : IComponentWPF
    where TPanel : Panel, new()
    where TBuilder : GuiPanelBuilderWPF<TPanel, TBuilder>
{
    public TPanel Root { get; } = new TPanel();

    private IComponentWPF? _parent;
    public string Id { get; } = Guid.NewGuid().ToString();

    protected readonly List<Action> _definitionRoster = new();

    // Type-safe cast loop that returns the actual terminal subclass instance
    protected TBuilder Actual => (TBuilder)this;

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

    public void SetParent(IComponentWPF? parent) => _parent = parent;
    public IComponentWPF? GetParent() => _parent;

    public virtual void Publish(string topic, object payload) => _parent?.Publish(topic, payload);
    public virtual void Receive(string topic, object payload) { }

    /// <summary>
    /// Fluently queues an element for creation while keeping the exact builder subclass signature active.
    /// </summary>
    public TBuilder Add<T>(Action<T> config) where T : UIElement, new()
    {
        _definitionRoster.Add(() => {
            var el = new T();
            config?.Invoke(el);
            Root.Children.Add(el);
        });
        return Actual;
    }

    /// <summary>
    /// Embeds a compiled component builder block directly into the layout.
    /// </summary>
    public TBuilder AddComponent(IComponentWPF component)
    {
        if (component != null)
        {
            component.SetParent(this);
            _definitionRoster.Add(() => Root.Children.Add(component.Build()));
        }
        return Actual;
    }

    public virtual FrameworkElement Build()
    {
        Root.Children.Clear();
        foreach (var action in _definitionRoster)
        {
            action.Invoke();
        }
        return Root;
    }

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