﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using NP.Concepts.Behaviors;
using System;
using NP.Utilities;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class GridResizeBehavior
    {

        #region Target Attached Avalonia Property
        public static IControl GetTarget(Grid obj)
        {
            return obj.GetValue(TargetProperty);
        }

        public static void SetTarget(Grid obj, IControl value)
        {
            obj.SetValue(TargetProperty, value);
        }

        public static readonly AttachedProperty<IControl> TargetProperty =
            AvaloniaProperty.RegisterAttached<Grid, Grid, IControl>
            (
                "Target"
            );
        #endregion Target Attached Avalonia Property

        #region CurrentSplitterPosition Attached Avalonia Property
        public static double? GetCurrentSplitterPosition(IControl obj)
        {
            return obj.GetValue(CurrentSplitterPositionProperty);
        }

        public static void SetCurrentSplitterPosition(IControl obj, double? value)
        {
            obj.SetValue(CurrentSplitterPositionProperty, value);
        }

        public static readonly AttachedProperty<double?> CurrentSplitterPositionProperty =
            AvaloniaProperty.RegisterAttached<GridResizeBehavior, IControl, double?>
            (
                "CurrentSplitterPosition"
            );
        #endregion CurrentSplitterPosition Attached Avalonia Property



        static GridResizeBehavior()
        {
            TargetProperty.Changed.Subscribe(OnTargetChanged);
        }

        private static void OnTargetChanged(AvaloniaPropertyChangedEventArgs<IControl> changedArgs)
        {
            Grid grid = (Grid) changedArgs.Sender;

            IDisposable? behavior = null;
            Func<Point, double>? shiftConverter = null;
            double initialSplitterPosition = 0;
            double initialShift = 0;
            double minChange = 0, maxChange = 0;
            GridSplitter? gridSplitter = null;
            IControl targetControl = changedArgs.NewValue.Value;

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

                SetCurrentSplitterPosition(targetControl, initialShift + initialSplitterPosition);
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

                SetCurrentSplitterPosition(targetControl, initialShift + currentSplitterPosition);
            }

            void OnDragCompleted(object? sender, VectorEventArgs e)
            {
                SetCurrentSplitterPosition(targetControl, null);
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

            behavior?.Dispose();
            if (targetControl != null)
            {
                behavior = grid.Children.AddBehavior(OnChildAdded, OnChildRemoved);
            }
        }
    }
}
