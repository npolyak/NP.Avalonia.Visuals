using Avalonia;
using Avalonia.Controls;
using NP.Concepts.Behaviors;
using System;
using NP.Utilities;
using NP.Avalonia.Visuals.Behaviors;
using Avalonia.Collections;
using Avalonia.Metadata;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Avalonia.Data;
using System.Reactive.Linq;
using NP.Avalonia.Visuals.Converters;

namespace NP.Avalonia.Visuals.Controls
{
    public class AutoGrid : Control
    {
        private Grid _grid = new Grid();
        private KeyedDisposables<IControl> _keyedDisposables = new KeyedDisposables<IControl>();

        private IDisposable _behaviorSubscription;
        private IBinding _minRowBinding;
        private IBinding _minColumnBinding;

        [Content]
        public global::Avalonia.Controls.Controls Children => _grid.Children;


        #region MinRow Direct Avalonia Property
        private int _MinRow = default;

        public static readonly DirectProperty<AutoGrid, int> MinRowProperty =
            AvaloniaProperty.RegisterDirect<AutoGrid, int>
            (
                nameof(MinRow),
                o => o.MinRow,
                (o, v) => o.MinRow = v
            );

        public int MinRow
        {
            get => _MinRow;
            private set
            {
                SetAndRaise(MinRowProperty, ref _MinRow, value);
            }
        }

        #endregion MinRow Direct Avalonia Property


        #region MinColumn Direct Avalonia Property
        private int _MinColumn = default;

        public static readonly DirectProperty<AutoGrid, int> MinColumnProperty =
            AvaloniaProperty.RegisterDirect<AutoGrid, int>
            (
                nameof(MinColumn),
                o => o.MinColumn,
                (o, v) => o.MinColumn = v
            );

        public int MinColumn
        {
            get => _MinColumn;
            private set
            {
                SetAndRaise(MinColumnProperty, ref _MinColumn, value);
            }
        }

        #endregion MinColumn Direct Avalonia Property

        public RowDefinitions RowDefinitions => _grid.RowDefinitions;

        public ColumnDefinitions ColumnDefinitions => _grid.ColumnDefinitions;

        public AutoGrid()
        {
            this.VisualChildren.Add(_grid);
            this.LogicalChildren.Add(_grid);
            _behaviorSubscription = this.Children.AddBehavior(OnChildAdded, OnChildRemoved);

            _minRowBinding = this.GetObservable(MinRowProperty).Select(r => -r).ToBinding();
            _minColumnBinding = this.GetObservable(MinColumnProperty).Select(c => -c).ToBinding();
        }

        private void OnChildRemoved(IControl child)
        {
            PropertiesChangeObserver.SetPropChangeObserver(child, null);
            _keyedDisposables.Remove(child);
        }

        private void OnChildAdded(IControl child)
        {
            PropertiesChangeObserver propertiesChangeObserver = new PropertiesChangeObserver();

            propertiesChangeObserver.Props =
                new AvaloniaProperty[]
                {
                    RowProperty,
                    Grid.RowSpanProperty,
                    ColumnProperty,
                    Grid.ColumnSpanProperty
                };

            PropertiesChangeObserver.SetPropChangeObserver(child, propertiesChangeObserver);

            if (propertiesChangeObserver.ResultObservable != null)
            {
                _keyedDisposables.Add(child, propertiesChangeObserver.ResultObservable.Subscribe(OnChildChanged));
            }

            MultiBinding rowNumberBinding = 
                new MultiBinding { Converter = IntSumConverter.Instance };

            rowNumberBinding.Bindings.Add(_minRowBinding);
            rowNumberBinding.Bindings.Add(child.GetObservable(RowProperty).ToBinding());
            child.Bind(Grid.RowProperty, rowNumberBinding);

            MultiBinding columnNumberBinding =
                new MultiBinding { Converter = IntSumConverter.Instance };
            columnNumberBinding.Bindings.Add(_minColumnBinding);
            columnNumberBinding.Bindings.Add(child.GetObservable(ColumnProperty).ToBinding());
            child.Bind(Grid.ColumnProperty, columnNumberBinding);
        }

        private void OnChildChanged(IAvaloniaObject child)
        {
            Control childControl = (Control)child;

            int currentMinRow = Children.Cast<Control>().Min(c => GetRow(c));

            int currentMaxRow = Children.Cast<Control>().Max(c => GetRow(c) + Grid.GetRowSpan(c));

            int totalRows = currentMaxRow - currentMinRow;
            int extraRows = totalRows - _grid.RowDefinitions.Count;

            AddRows(extraRows);

            MinRow = currentMinRow;

            int currentMinColumn = Children.Cast<Control>().Min(c => GetColumn(c));
            int currentMaxColumn = Children.Cast<Control>().Max(c => GetColumn(c) + Grid.GetColumnSpan(c));

            int totalColumns = currentMaxColumn - currentMinColumn;
            int extraColumns = totalColumns - _grid.ColumnDefinitions.Count;

            AddColumns(extraColumns);
            MinColumn = currentMinColumn;

            //int childRow = AGrid.GetRow(childControl);

            //int maxRow = childRow + Grid.GetRowSpan(childControl) - 1;

            //int numberExtraRows = maxRow + 1 - _grid.RowDefinitions.Count;
            //for (int i = 0; i < numberExtraRows; i++)
            //{
            //    _grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            //}

            //int maxColumn = AGrid.GetColumn(childControl) + Grid.GetColumnSpan(childControl) - 1;
            //int numberExtraCols = maxColumn + 1 - _grid.ColumnDefinitions.Count;
            //for (int i = 0; i < numberExtraCols; i++)
            //{
            //    _grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            //}
        }

        private void AddGridDefs<T>(int numberExtraDefs, IList<T> defs, Func<T> defCreator)
            where T : DefinitionBase
        {
            int absNumberExtraItems = Math.Abs(numberExtraDefs);

            if (absNumberExtraItems == 0)
                return;

            for (int i = 0; i < absNumberExtraItems; i++)
            {
                if (numberExtraDefs > 0)
                {
                    defs.Add(defCreator());
                }
                else
                {
                    defs.RemoveAt(0);
                }
            }
        }

        private void AddRows(int numberExtraRows) =>
            AddGridDefs(numberExtraRows, _grid.RowDefinitions, () => new RowDefinition(GridLength.Auto));

        private void AddColumns(int numberExtraCols) =>
            AddGridDefs(numberExtraCols, _grid.ColumnDefinitions, () => new ColumnDefinition(GridLength.Auto));

        #region Row Attached Avalonia Property
        public static int GetRow(Control obj)
        {
            return obj.GetValue(RowProperty);
        }

        public static void SetRow(Control obj, int value)
        {
            obj.SetValue(RowProperty, value);
        }

        public static readonly AttachedProperty<int> RowProperty =
            AvaloniaProperty.RegisterAttached<object, Control, int>
            (
                "Row"
            );
        #endregion Row Attached Avalonia Property


        #region Column Attached Avalonia Property
        public static int GetColumn(Control obj)
        {
            return obj.GetValue(ColumnProperty);
        }

        public static void SetColumn(Control obj, int value)
        {
            obj.SetValue(ColumnProperty, value);
        }

        public static readonly AttachedProperty<int> ColumnProperty =
            AvaloniaProperty.RegisterAttached<object, Control, int>
            (
                "Column"
            );
        #endregion Column Attached Avalonia Property
    }
}
