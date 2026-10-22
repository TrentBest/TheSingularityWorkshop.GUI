using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TheSingularityWorkshop.GUI.WPF.Abstractions;

namespace TheSingularityWorkshop.GUI.WPF.Layouts
{
    /// <summary>
    /// RLM-12: Dedicated structural builder for resizable Grid layouts.
    /// Fully parameterized dimensions, behaviors, and theme-aware runtime generation.
    /// Augmented with a Smart Compatibility Proxy Layer to satisfy legacy layout mutations.
    /// </summary>
    public class GridGuiBuilderWPF : GuiPanelBuilderWPF<Grid, GridGuiBuilderWPF>
    {
        private readonly List<GridDimensionDefinition> _columns = new();
        private readonly List<GridDimensionDefinition> _rows = new();
        private readonly List<ComponentPlacement> _placements = new();

        private double _splitterTrackThickness = 5;
        private double _splitterVisualThickness = 3;
        private string _splitterResourceKey = "FTIBorder";
        private Brush _splitterFallbackBrush = Brushes.DimGray;
        private GridResizeBehavior _splitterResizeBehavior = GridResizeBehavior.PreviousAndNext;

        #region Smart Backward-Compatibility Bridge Proxy

        /// <summary>Child components that receive forwarded bus messages.</summary>
        protected readonly List<IComponentWPF> _childBuilders = new();

        /// <summary>Gets or sets the compatibility name associated with this grid builder.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the parent component through the shared component relationship.</summary>
        public IComponentWPF? Parent
        {
            get => GetParent();
            set => SetParent(value);
        }

        /// <summary>
        /// Compatibility Handle: Exposes a smart wrapper proxy that duck-types Background, 
        /// BorderThickness, and Child assignments to shield legacy consumer code.
        /// </summary>
        protected CompatibilityElementProxy _container => new CompatibilityElementProxy(Root);

        /// <summary>Gets the root grid exposed for compatibility with older derived builders.</summary>
        protected Grid _overlayGrid => Root;

        /// <summary>Initializes a grid builder with a name and default GUI configuration.</summary>
        /// <param name="name">Compatibility name; null is normalized to an empty string.</param>
        public GridGuiBuilderWPF(string name) : base(new GuiConfiguration())
        {
            Name = name ?? string.Empty;
        }

        /// <summary>Forwards a named bus payload through this component and its child builders.</summary>
        /// <param name="busName">Bus topic or name to forward.</param>
        /// <param name="payload">Payload to deliver.</param>
        public void ReceiveBusPayload(string busName, object payload)
        {
            Receive(busName, payload);
        }

        /// <summary>Receives a message and forwards it to each registered child component.</summary>
        /// <param name="topic">Message topic.</param>
        /// <param name="payload">Message payload.</param>
        public override void Receive(string topic, object payload)
        {
            base.Receive(topic, payload);
            foreach (var child in _childBuilders)
            {
                child.Receive(topic, payload);
            }
        }

        /// <summary>
        /// Dynamic structural interceptor class that maps legacy properties seamlessly.
        /// </summary>
        protected class CompatibilityElementProxy
        {
            /// <summary>The underlying grid used by this compatibility proxy.</summary>
            private readonly Grid _grid;
            /// <summary>Creates a proxy over the specified grid.</summary>
            /// <param name="grid">Grid whose properties and children are exposed.</param>
            public CompatibilityElementProxy(Grid grid) => _grid = grid;

            /// <summary>Gets or sets the underlying grid background.</summary>
            public Brush Background
            {
                get => _grid.Background;
                set => _grid.Background = value;
            }

            /// <summary>Gets a transparent placeholder; setting this property has no effect because Grid has no border brush.</summary>
            public Brush BorderBrush
            {
                get => Brushes.Transparent;
                set { /* Grids do not have a border brush; safe operational fallback */ }
            }

            /// <summary>Gets zero thickness; setting this property has no effect because Grid has no border thickness.</summary>
            public Thickness BorderThickness
            {
                get => new Thickness(0);
                set { /* Grids do not have border metrics; safe operational fallback */ }
            }

            /// <summary>Gets a zero corner radius; setting this compatibility property has no effect.</summary>
            public CornerRadius CornerRadius
            {
                get => new CornerRadius(0);
                set { /* Intercepted smoothly */ }
            }

            /// <summary>Gets the first currently mounted grid child, if any.</summary>
            /// <remarks>
            /// This is intentionally read-only. Mutating the visual tree directly bypasses the builder's
            /// deferred definitions, component ownership checks, and repeatable-build contract. Use the
            /// grid builder's declarative composition methods to add children.
            /// </remarks>
            public UIElement? Child => _grid.Children.Count > 0 ? _grid.Children[0] : null;

            /// <summary>Begins an animation on the underlying grid.</summary>
            /// <param name="dp">Dependency property to animate.</param>
            /// <param name="animation">Animation timeline to apply.</param>
            public void BeginAnimation(DependencyProperty dp, AnimationTimeline animation)
            {
                _grid.BeginAnimation(dp, animation);
            }

            // Implicit conversions allow this wrapper to be used anywhere FrameworkElement is requested
            /// <summary>Converts the proxy to its underlying grid as a framework element.</summary>
            /// <param name="proxy">Proxy to convert.</param>
            public static implicit operator FrameworkElement(CompatibilityElementProxy proxy) => proxy._grid;
            /// <summary>Converts the proxy to its underlying grid as a UI element.</summary>
            /// <param name="proxy">Proxy to convert.</param>
            public static implicit operator UIElement(CompatibilityElementProxy proxy) => proxy._grid;
        }

        #endregion

        /// <summary>Initializes the grid builder with the supplied GUI configuration.</summary>
        /// <param name="config">Configuration used by the shared panel builder.</param>
        public GridGuiBuilderWPF(GuiConfiguration config) : base(config)
        {
        }

        /// <summary>Sets the layout-track thickness and visible thickness used for grid splitters.</summary>
        /// <param name="trackThickness">Width or height reserved for the splitter track.</param>
        /// <param name="visualThickness">Width or height of the visible splitter.</param>
        /// <returns>This builder, for fluent configuration.</returns>
        public GridGuiBuilderWPF WithSplitterThickness(double trackThickness, double visualThickness)
        {
            _splitterTrackThickness = trackThickness;
            _splitterVisualThickness = visualThickness;
            return this;
        }

        /// <summary>Configures the resource key and fallback brush used to style splitters.</summary>
        /// <param name="resourceKey">Resource key looked up on the generated splitter.</param>
        /// <param name="fallbackBrush">Brush used when the resource is unavailable.</param>
        /// <returns>This builder, for fluent configuration.</returns>
        public GridGuiBuilderWPF WithSplitterStyle(string resourceKey, Brush fallbackBrush)
        {
            _splitterResourceKey = resourceKey;
            _splitterFallbackBrush = fallbackBrush;
            return this;
        }

        /// <summary>Sets how a splitter distributes resizing between adjacent grid tracks.</summary>
        /// <param name="behavior">Resize behavior applied to generated splitters.</param>
        /// <returns>This builder, for fluent configuration.</returns>
        public GridGuiBuilderWPF WithSplitterResizeBehavior(GridResizeBehavior behavior)
        {
            _splitterResizeBehavior = behavior;
            return this;
        }

        /// <summary>Adds a column definition and optionally reserves splitter tracks after it.</summary>
        /// <param name="value">Grid length value interpreted using <paramref name="unitType"/>.</param>
        /// <param name="unitType">Sizing mode, such as pixel, auto, or star.</param>
        /// <param name="isResizable">Whether a splitter track follows this column when another column exists.</param>
        /// <returns>This builder, for fluent configuration.</returns>
        public GridGuiBuilderWPF AddColumnTrack(double value, GridUnitType unitType, bool isResizable = false)
        {
            _columns.Add(new GridDimensionDefinition { Value = value, UnitType = unitType, IsResizable = isResizable });
            return this;
        }

        /// <summary>Adds a row definition and optionally reserves splitter tracks after it.</summary>
        /// <param name="value">Grid length value interpreted using <paramref name="unitType"/>.</param>
        /// <param name="unitType">Sizing mode, such as pixel, auto, or star.</param>
        /// <param name="isResizable">Whether a splitter track follows this row when another row exists.</param>
        /// <returns>This builder, for fluent configuration.</returns>
        public GridGuiBuilderWPF AddRowTrack(double value, GridUnitType unitType, bool isResizable = false)
        {
            _rows.Add(new GridDimensionDefinition { Value = value, UnitType = unitType, IsResizable = isResizable });
            return this;
        }

        /// <summary>Registers a child component at a logical row and column, accounting for inserted splitter tracks.</summary>
        /// <param name="row">Zero-based logical row index.</param>
        /// <param name="col">Zero-based logical column index.</param>
        /// <param name="component">Component to parent, build, and place in the grid.</param>
        /// <returns>This builder, for fluent configuration.</returns>
        public GridGuiBuilderWPF PlaceComponent(int row, int col, IComponentWPF component)
        {
            RegisterChildComponent(component);
            _placements.Add(new ComponentPlacement { Row = row, Col = col, Component = component });
            _childBuilders.Add(component);
            return this;
        }

        /// <summary>Builds the configured grid, including definitions, splitters, and placed child components.</summary>
        /// <returns>The root grid after its definitions and generated elements are updated.</returns>
        public override FrameworkElement Build()
        {
            // Validate the complete deferred layout before mutating the mounted root.
            // Callers may declare placements before they finish declaring tracks.
            ValidatePlacements();

            Root.ColumnDefinitions.Clear();
            Root.RowDefinitions.Clear();

            // 1. Process base roster layout elements first
            base.Build();

            // 2. Unroll columns and splitters
            for (int i = 0; i < _columns.Count; i++)
            {
                var colDef = _columns[i];
                Root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(colDef.Value, colDef.UnitType) });

                if (colDef.IsResizable && i < _columns.Count - 1)
                {
                    Root.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_splitterTrackThickness) });
                    var splitter = new GridSplitter
                    {
                        Width = _splitterVisualThickness,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        ResizeBehavior = _splitterResizeBehavior
                    };
                    SafeSetResource(splitter, GridSplitter.BackgroundProperty, _splitterResourceKey, _splitterFallbackBrush);
                    Grid.SetColumn(splitter, Root.ColumnDefinitions.Count - 1);
                    Grid.SetRowSpan(splitter, Math.Max(1, _rows.Count * 2));
                    Root.Children.Add(splitter);
                }
            }

            // 3. Unroll rows and splitters
            for (int i = 0; i < _rows.Count; i++)
            {
                var rowDef = _rows[i];
                Root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowDef.Value, rowDef.UnitType) });

                if (rowDef.IsResizable && i < _rows.Count - 1)
                {
                    Root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(_splitterTrackThickness) });
                    var splitter = new GridSplitter
                    {
                        Height = _splitterVisualThickness,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        ResizeBehavior = _splitterResizeBehavior
                    };
                    SafeSetResource(splitter, GridSplitter.BackgroundProperty, _splitterResourceKey, _splitterFallbackBrush);
                    Grid.SetRow(splitter, Root.RowDefinitions.Count - 1);
                    Grid.SetColumnSpan(splitter, Math.Max(1, _columns.Count * 2));
                    Root.Children.Add(splitter);
                }
            }

            // 4. Position and mount nested elements into the grid matrix
            foreach (var placement in _placements)
            {
                var element = placement.Component.Build();
                int targetCol = CalculateActualIndex(_columns, placement.Col);
                int targetRow = CalculateActualIndex(_rows, placement.Row);

                Grid.SetColumn(element, targetCol);
                Grid.SetRow(element, targetRow);
                Root.Children.Add(element);
            }

            return Root;
        }

        private void ValidatePlacements()
        {
            foreach (var placement in _placements)
            {
                ValidateLogicalIndex("row", placement.Row, _rows.Count);
                ValidateLogicalIndex("column", placement.Col, _columns.Count);
            }
        }

        private static void ValidateLogicalIndex(string dimension, int logicalIndex, int trackCount)
        {
            if (logicalIndex < 0 || logicalIndex >= trackCount)
            {
                throw new InvalidOperationException(
                    $"Grid component {dimension} index {logicalIndex} is outside the declared logical track range [0, {trackCount}). " +
                    $"Declare at least {logicalIndex + 1} {dimension} track(s) before building the grid.");
            }
        }

        private int CalculateActualIndex(List<GridDimensionDefinition> source, int logicalIndex)
        {
            int actualIndex = 0;
            for (int i = 0; i < logicalIndex; i++)
            {
                actualIndex++;
                if (i < source.Count && source[i].IsResizable) actualIndex++;
            }
            return actualIndex;
        }

        private static void SafeSetResource(FrameworkElement element, DependencyProperty property, string key, object fallback)
        {
            try { element.SetValue(property, element.TryFindResource(key) ?? fallback); } catch { element.SetValue(property, fallback); }
        }

        private class GridDimensionDefinition { public double Value { get; set; } public GridUnitType UnitType { get; set; } public bool IsResizable { get; set; } }
        private class ComponentPlacement { public int Row { get; set; } public int Col { get; set; } public IComponentWPF Component { get; set; } = null!; }
    }
}