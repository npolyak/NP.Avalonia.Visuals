using Avalonia;
using Avalonia.Controls;
using NP.Utilities;
using System;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class StringContactBehavior
    {
        #region Str1 Attached Avalonia Property
        public static string? GetStr1(IControl obj)
        {
            return obj.GetValue(Str1Property);
        }

        public static void SetStr1(IControl obj, string? value)
        {
            obj.SetValue(Str1Property, value);
        }

        public static readonly AttachedProperty<string?> Str1Property =
            AvaloniaProperty.RegisterAttached<StringContactBehavior, IControl, string?>
            (
                "Str1"
            );
        #endregion Str1 Attached Avalonia Property


        #region Str2 Attached Avalonia Property
        public static string? GetStr2(IControl obj)
        {
            return obj.GetValue(Str2Property);
        }

        public static void SetStr2(IControl obj, string? value)
        {
            obj.SetValue(Str2Property, value);
        }

        public static readonly AttachedProperty<string?> Str2Property =
            AvaloniaProperty.RegisterAttached<StringContactBehavior, IControl, string?>
            (
                "Str2"
            );
        #endregion Str2 Attached Avalonia Property


        #region Result Attached Avalonia Property
        public static string? GetResult(IControl obj)
        {
            return obj.GetValue(ResultProperty);
        }

        private static void SetResult(IControl obj, string? value)
        {
            obj.SetValue(ResultProperty, value);
        }

        public static readonly AttachedProperty<string?> ResultProperty =
            AvaloniaProperty.RegisterAttached<StringContactBehavior, IControl, string?>
            (
                "Result"
            );
        #endregion Result Attached Avalonia Property


        static StringContactBehavior()
        {
            Str1Property.Changed.Subscribe(CalculateResult);
            Str2Property.Changed.Subscribe(CalculateResult);
        }

        private static void CalculateResult(AvaloniaPropertyChangedEventArgs<string?> args)
        {
            var control = (IControl) args.Sender;

            string? str1 = GetStr1(control);

            string? str2 = GetStr2(control);

            string? result = null;

            if ((str1 != null) && (str2 != null))
            {
                result = str1 + str2;
            }

            SetResult(control, result);
        }
    }
}
