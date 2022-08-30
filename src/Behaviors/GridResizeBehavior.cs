using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using NP.Concepts.Behaviors;
using System;
using System.Collections.Generic;
using NP.Utilities;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class GridResizeBehavior
    {
        #region CurrentGridSplitter Attached Avalonia Property
        public static GridSplitter? GetCurrentGridSplitter(Grid obj)
        {
            return obj.GetValue(CurrentGridSplitterProperty);
        }

        public static void SetCurrentGridSplitter(Grid obj, GridSplitter? value)
        {
            obj.SetValue(CurrentGridSplitterProperty, value);
        }

        public static readonly AttachedProperty<GridSplitter?> CurrentGridSplitterProperty =
            AvaloniaProperty.RegisterAttached<Grid, Grid, GridSplitter?>
            (
                "CurrentGridSplitter"
            );
        #endregion CurrentGridSplitter Attached Avalonia Property


        #region IsOn Attached Avalonia Property
        public static bool GetIsOn(Grid obj)
        {
            return obj.GetValue(IsOnProperty);
        }

        public static void SetIsOn(Grid obj, bool value)
        {
            obj.SetValue(IsOnProperty, value);
        }

        public static readonly AttachedProperty<bool> IsOnProperty =
            AvaloniaProperty.RegisterAttached<Grid, Grid, bool>
            (
                "IsOn"
            );
        #endregion IsOn Attached Avalonia Property


        #region CurrentSplitterPosition Attached Avalonia Property
        public static double? GetCurrentSplitterPosition(Grid obj)
        {
            return obj.GetValue(CurrentSplitterPositionProperty);
        }

        public static void SetCurrentSplitterPosition(Grid obj, double? value)
        {
            obj.SetValue(CurrentSplitterPositionProperty, value);
        }

        public static readonly AttachedProperty<double?> CurrentSplitterPositionProperty =
            AvaloniaProperty.RegisterAttached<Grid, Grid, double?>
            (
                "CurrentSplitterPosition"
            );
        #endregion CurrentSplitterPosition Attached Avalonia Property



        static GridResizeBehavior()
        {
            IsOnProperty.Changed.Subscribe(OnIsOnChanged);
        }

        private static void OnIsOnChanged(AvaloniaPropertyChangedEventArgs<bool> changedArgs)
        {
            Grid grid = (Grid) changedArgs.Sender;

            IDisposable? behavior = null;
            Func<Point, double>? shiftConverter = null;
            double initialSplitterPosition = 0;
            double initialShift = 0;
            double minChange = 0, maxChange = 0;
            GridSplitter gridSplitter = null;

            void OnDragStarted(object? sender, VectorEventArgs e)
            {
                gridSplitter = (GridSplitter)sender!;

                object resizeData = gridSplitter.GetFieldValue<object>("_resizeData", true, typeof(GridSplitter));

                minChange = resizeData.GetFieldValue<double>("MinChange", true);

                maxChange = resizeData.GetFieldValue<double>("MaxChange", true);

                GridResizeDirection gridResizeDirection = gridSplitter.GetRealResizeDirection();

                if (gridResizeDirection == GridResizeDirection.Columns)
                {
                    shiftConverter = (p) => p.X;
                }
                else
                {
                    shiftConverter = (p) => p.Y;
                }


                Point gridSplitterSize = new Point(gridSplitter.Bounds.Width, gridSplitter.Bounds.Height);

                double gridSplitterDimension = shiftConverter(gridSplitterSize);

                maxChange -= gridSplitterDimension;

                initialSplitterPosition = shiftConverter(gridSplitter.TranslatePoint(new Point(), grid)!.Value);
                initialShift = shiftConverter(new Point(e.Vector.X, e.Vector.Y));

                SetCurrentSplitterPosition(grid, initialShift + initialSplitterPosition);
                SetCurrentGridSplitter(grid, gridSplitter);
            }

            void OnDragDelta(object? sender, VectorEventArgs e)
            {
                double newShift = shiftConverter!(new Point(e.Vector.X, e.Vector.Y));

                double delta = newShift - initialShift;

                if (delta < minChange)
                {
                    delta = minChange;
                }

                if (delta > maxChange)
                {
                    delta = maxChange;  
                }

                double currentSplitterPosition = delta + initialSplitterPosition;

                SetCurrentSplitterPosition(grid, initialShift + currentSplitterPosition);
            }

            void OnDragCompleted(object? sender, VectorEventArgs e)
            {
                SetCurrentGridSplitter(grid, null);
                SetCurrentSplitterPosition(grid, null);
            }

            void OnChildAdded(IControl child)
            {
                if (child is GridSplitter gridSplitter)
                {
                    gridSplitter.DragStarted += OnDragStarted;
                    gridSplitter.DragDelta += OnDragDelta;
                    gridSplitter.DragCompleted += OnDragCompleted;
                }
            }
            void OnChildRemoved(IControl child)
            {
                if (child is GridSplitter gridSplitter)
                {
                    gridSplitter.DragCompleted -= OnDragCompleted;
                    gridSplitter.DragDelta -= OnDragDelta;
                    gridSplitter.DragStarted -= OnDragStarted;
                }
            }

            if (changedArgs.NewValue.Value)
            {
                behavior = grid.Children.AddBehavior(OnChildAdded, OnChildRemoved);
            }
            else
            {
                behavior?.Dispose();
            }
        }
    }
}
