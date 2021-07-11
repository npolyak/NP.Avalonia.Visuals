using Avalonia;
using Avalonia.Controls;
using System.Collections.Generic;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class ClassesBehavior
    {
        #region TheClasses Attached Avalonia Property
        public static string GetTheClasses(AvaloniaObject obj)
        {
            return obj.GetValue(TheClassesProperty);
        }

        public static void SetTheClasses(AvaloniaObject obj, string value)
        {
            obj.SetValue(TheClassesProperty, value);
        }

        public static readonly AttachedProperty<string> TheClassesProperty =
            AvaloniaProperty.RegisterAttached<object, StyledElement, string>
            (
                "TheClasses"
            );
        #endregion TheClasses Attached Avalonia Property

        static ClassesBehavior()
        {
            TheClassesProperty.Changed.Subscribe(OnClassesChanged);
        }

        private static void OnClassesChanged(AvaloniaPropertyChangedEventArgs<string> change)
        {
            IStyledElement sender = change.Sender as IStyledElement;

            string classesStr = change.NewValue.Value;

            if (classesStr != null)
            {
                var classes = classesStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
                sender.Classes = new Classes(classes);
            }
            else
            {
                sender.Classes = new Classes();
            }
        }
    }
}
