using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using NP.Utilities;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class CallAction
    {
        #region TheEvent Attached Avalonia Property
        public static RoutedEvent GetTheEvent(AvaloniaObject obj)
        {
            return obj.GetValue(TheEventProperty);
        }

        public static void SetTheEvent(AvaloniaObject obj, RoutedEvent value)
        {
            obj.SetValue(TheEventProperty, value);
        }

        public static readonly AttachedProperty<RoutedEvent> TheEventProperty =
            AvaloniaProperty.RegisterAttached<object, Control, RoutedEvent>
            (
                "TheEvent"
            );
        #endregion TheEvent Attached Avalonia Property


        #region TargetObject Attached Avalonia Property
        public static object GetTargetObject(AvaloniaObject obj)
        {
            return obj.GetValue(TargetObjectProperty);
        }

        public static void SetTargetObject(AvaloniaObject obj, object value)
        {
            obj.SetValue(TargetObjectProperty, value);
        }

        public static readonly AttachedProperty<object> TargetObjectProperty =
            AvaloniaProperty.RegisterAttached<object, Control, object>
            (
                "TargetObject"
            );
        #endregion TargetObject Attached Avalonia Property


        #region MethodName Attached Avalonia Property
        public static string GetMethodName(AvaloniaObject obj)
        {
            return obj.GetValue(MethodNameProperty);
        }

        public static void SetMethodName(AvaloniaObject obj, string value)
        {
            obj.SetValue(MethodNameProperty, value);
        }

        public static readonly AttachedProperty<string> MethodNameProperty =
            AvaloniaProperty.RegisterAttached<object, Control, string>
            (
                "MethodName"
            );
        #endregion MethodName Attached Avalonia Property

        private static void ResetEvent(AvaloniaPropertyChangedEventArgs<RoutedEvent> e)
        {
            Interactive? sender = e.Sender as Interactive;

            if (sender == null)
                return;

            if (e.OldValue.HasValue)
            {
                RoutedEvent value = e.OldValue.Value;

                if (value != null)
                {
                    sender?.RemoveHandler(value, (EventHandler<RoutedEventArgs>)OnEvent);
                }
            }

            if (e.NewValue.HasValue)
            {
                RoutedEvent value = e.NewValue.Value;

                if (value != null)
                {
                    sender?.AddHandler
                        (
                        value,
                        (EventHandler<RoutedEventArgs>)OnEvent,
                        RoutingStrategies.Bubble | RoutingStrategies.Direct | RoutingStrategies.Tunnel);
                }
            }
        }

        private static void OnEvent(object? sender, RoutedEventArgs e)
        {
            Interactive? avaloniaObject = sender as Interactive;

            if (avaloniaObject == null)
                return;

            string methodName = avaloniaObject.GetValue(MethodNameProperty);

            if (methodName == null)
                return;

            object? targetObject = avaloniaObject.GetValue(TargetObjectProperty) ?? avaloniaObject.DataContext;

            if (targetObject == null)
                return;

            targetObject.CallMethod(methodName);
        }

        static CallAction()
        {
            TheEventProperty.Changed.Subscribe(ResetEvent);
        }
    }
}
