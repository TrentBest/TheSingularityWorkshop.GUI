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

        protected readonly List<IComponentWPF> _childBuilders = new();

        public string Name { get; set; } = string.Empty;

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

        protected Grid _overlayGrid => Root;

        public GridGuiBuilderWPF(string name) : base(new GuiConfiguration())
        {
            Name = name ?? string.Empty;
        }

        public void ReceiveBusPayload(string busName, object payload)
        {
            Receive(busName, payload);
        }

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
            private readonly Grid _grid;
            public CompatibilityElementProxy(Grid grid) => _grid = grid;

            public Brush Background
            {
                get => _grid.Background;
                set => _grid.Background = value;
            }

            public Brush BorderBrush
            {
                get => Brushes.Transparent;
                set { /* Grids do not have a border brush; safe operational fallback */ }
            }

            public Thickness BorderThickness
            {
                get => new Thickness(0);
                set { /* Grids do not have border metrics; safe operational fallback */ }
            }

            public CornerRadius CornerRadius
            {
                get => new CornerRadius(0);
                set { /* Intercepted smoothly */ }
            }

            public UIElement? Child
            {
                get => _grid.Children.Count > 0 ? _grid.Children[0] : null;
                set
                {
                    _grid.Children.Clear();
                    if (value != null) _grid.Children.Add(value);
                }
            }

            public void BeginAnimation(DependencyProperty dp, AnimationTimeline animation)
            {
                _grid.BeginAnimation(dp, animation);
            }

            // Implicit conversions allow this wrapper to be used anywhere FrameworkElement is requested
            public static implicit operator FrameworkElement(CompatibilityElementProxy proxy) => proxy._grid;
            public static implicit operator UIElement(CompatibilityElementProxy proxy) => proxy._grid;
        }

        #endregion

        public GridGuiBuilderWPF(GuiConfiguration config) : base(config)
        {
        }

        public GridGuiBuilderWPF WithSplitterThickness(double trackThickness, double visualThickness)
        {
            _splitterTrackThickness = trackThickness;
            _splitterVisualThickness = visualThickness;
            return this;
        }

        public GridGuiBuilderWPF WithSplitterStyle(string resourceKey, Brush fallbackBrush)
        {
            _splitterResourceKey = resourceKey;
            _splitterFallbackBrush = fallbackBrush;
            return this;
        }

        public GridGuiBuilderWPF WithSplitterResizeBehavior(GridResizeBehavior behavior)
        {
            _splitterResizeBehavior = behavior;
            return this;
        }

        public GridGuiBuilderWPF AddColumnTrack(double value, GridUnitType unitType, bool isResizable = false)
        {
            _columns.Add(new GridDimensionDefinition { Value = value, UnitType = unitType, IsResizable = isResizable });
            return this;
        }

        public GridGuiBuilderWPF AddRowTrack(double value, GridUnitType unitType, bool isResizable = false)
        {
            _rows.Add(new GridDimensionDefinition { Value = value, UnitType = unitType, IsResizable = isResizable });
            return this;
        }

        public GridGuiBuilderWPF PlaceComponent(int row, int col, IComponentWPF component)
        {
            component.SetParent(this);
            _placements.Add(new ComponentPlacement { Row = row, Col = col, Component = component });
            _childBuilders.Add(component);
            return this;
        }

        public override FrameworkElement Build()
        {
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