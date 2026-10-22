using System.Windows;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Layouts;

/// <summary>Builds a Canvas panel whose children can be positioned by absolute coordinates.</summary>
public class CanvasGuiBuilderWPF : GuiPanelBuilderWPF<Canvas, CanvasGuiBuilderWPF>
{
    private readonly HashSet<UIElement> _placedElements = new(ReferenceEqualityComparer.Instance);

    /// <summary>Initializes the canvas builder with the supplied GUI configuration.</summary>
    /// <param name="config">Configuration used by the shared panel builder.</param>
    public CanvasGuiBuilderWPF(GuiConfiguration config) : base(config) { }

    /// <summary>Queues an element to be placed at the specified left and top coordinates.</summary>
    /// <param name="element">Element to add to the canvas.</param>
    /// <param name="left">Horizontal offset from the canvas's left edge.</param>
    /// <param name="top">Vertical offset from the canvas's top edge.</param>
    /// <returns>This builder, for fluent configuration.</returns>
    public CanvasGuiBuilderWPF PlaceAt(UIElement element, double left, double top)
    {
        ArgumentNullException.ThrowIfNull(element);

        if (ReferenceEquals(element, Root))
            throw new InvalidOperationException("A canvas cannot place itself as a child.");

        if (VisualTreeHelper.GetParent(element) is not null || LogicalTreeHelper.GetParent(element) is not null)
            throw new InvalidOperationException("The UIElement already belongs to a visual or logical parent. Supply an unattached element.");

        if (!_placedElements.Add(element))
            throw new InvalidOperationException("The same UIElement is already registered with this canvas. Create a distinct element for each placement.");

        _definitionRoster.Add(() => {
            Canvas.SetLeft(element, left);
            Canvas.SetTop(element, top);
            Root.Children.Add(element);
        });
        return this;
    }
}