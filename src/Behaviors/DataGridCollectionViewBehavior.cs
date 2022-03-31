using Avalonia;
using Avalonia.Controls;
using System.Collections;
using System;
using Avalonia.Collections;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class DataGridCollectionViewBehavior
    {
        #region ItemsSource Attached Avalonia Property
        public static IEnumerable GetItemsSource(DataGrid obj)
        {
            return obj.GetValue(ItemsSourceProperty);
        }

        public static void SetItemsSource(DataGrid obj, IEnumerable value)
        {
            obj.SetValue(ItemsSourceProperty, value);
        }

        public static readonly AttachedProperty<IEnumerable> ItemsSourceProperty =
            AvaloniaProperty.RegisterAttached<DataGridCollectionViewBehavior, DataGrid, IEnumerable>
            (
                "ItemsSource"
            );
        #endregion ItemsSource Attached Avalonia Property

        static DataGridCollectionViewBehavior()
        {
            ItemsSourceProperty.Changed.Subscribe(OnItemsSourcePropertyChanged);
        }

        private static void OnItemsSourcePropertyChanged(AvaloniaPropertyChangedEventArgs<IEnumerable> args)
        {
            DataGrid dataGrid = (DataGrid) args.Sender;

            IEnumerable itemsSource = args.NewValue.Value;

            dataGrid.Items = itemsSource == null ? null : new DataGridCollectionView(itemsSource);
        }
    }
}
