using Avalonia;
using Avalonia.Controls;
using System;
using NP.Utilities;
using Avalonia.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace NP.DataGridFilteringDemo
{
    public class AttachedProps
    {
        #region ColumnFilterText Attached Avalonia Property
        public static string GetColumnFilterText(IControl obj)
        {
            return obj.GetValue(ColumnFilterTextProperty);
        }

        public static void SetColumnFilterText(IControl obj, string value)
        {
            obj.SetValue(ColumnFilterTextProperty, value);
        }

        public static readonly AttachedProperty<string> ColumnFilterTextProperty =
            AvaloniaProperty.RegisterAttached<AttachedProps, IControl, string>
            (
                "ColumnFilterText"
            );
        #endregion ColumnFilterText Attached Avalonia Property


        #region FilterPropName Attached Avalonia Property
        public static string GetFilterPropName(AvaloniaObject obj)
        {
            return obj.GetValue(FilterPropNameProperty);
        }

        public static void SetFilterPropName(AvaloniaObject obj, string value)
        {
            obj.SetValue(FilterPropNameProperty, value);
        }

        public static readonly AttachedProperty<string> FilterPropNameProperty =
            AvaloniaProperty.RegisterAttached<AttachedProps, AvaloniaObject, string>
            (
                "FilterPropName"
            );
        #endregion FilterPropName Attached Avalonia Property

        public AttachedProps()
        {
            ColumnFilterTextProperty.Changed.Subscribe(OnColumnFilterTextChanged);
        }

        private static void OnColumnFilterTextChanged(AvaloniaPropertyChangedEventArgs<string> args)
        {
            DataGridColumnHeader header = (DataGridColumnHeader)args.Sender;

            string val = args.NewValue.Value;

            DataGrid dataGrid = (DataGrid)header.GetPropValue("OwningGrid");

            DataGridCollectionView collectionView = (DataGridCollectionView) dataGrid.Items;
        }

        public static void BuildFilter(DataGrid dataGrid)
        {
        }
    }
}
