using Avalonia;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NP.Avalonia.Visuals.Behaviors
{
    public class ProcessWindowBehavior
    {
        #region ProcessExePath Attached Avalonia Property
        public static string GetProcessExePath(AvaloniaObject obj)
        {
            return obj.GetValue(ProcessExePathProperty);
        }

        public static void SetProcessExePath(AvaloniaObject obj, string value)
        {
            obj.SetValue(ProcessExePathProperty, value);
        }

        public static readonly AttachedProperty<string> ProcessExePathProperty =
            AvaloniaProperty.RegisterAttached<ProcessWindowBehavior, AvaloniaObject, string>
            (
                "ProcessExePath"
            );
        #endregion ProcessExePath Attached Avalonia Property


        #region MainWindowHandle Attached Avalonia Property
        public static IntPtr GetMainWindowHandle(AvaloniaObject obj)
        {
            return obj.GetValue(MainWindowHandleProperty);
        }

        private static void SetMainWindowHandle(AvaloniaObject obj, IntPtr value)
        {
            obj.SetValue(MainWindowHandleProperty, value);
        }

        public static readonly AttachedProperty<IntPtr> MainWindowHandleProperty =
            AvaloniaProperty.RegisterAttached<ProcessWindowBehavior, AvaloniaObject, IntPtr>
            (
                "MainWindowHandle"
            );
        #endregion MainWindowHandle Attached Avalonia Property

        static ProcessWindowBehavior()
        {
            ProcessExePathProperty.Changed.Subscribe(OnStartProcesPathPropertyChanged);
        }

        private static async void OnStartProcesPathPropertyChanged(AvaloniaPropertyChangedEventArgs<string> changeArgs)
        {
            var sender = (AvaloniaObject) changeArgs.Sender;

            string exePath = changeArgs.NewValue.Value;

            if (exePath == null)
            {
                SetMainWindowHandle(sender, IntPtr.Zero);
                return;
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo(exePath);

            Process p = Process.Start(processStartInfo)!;

            while(true)
            {
                await Task.Delay(200);

                if (p.MainWindowHandle != IntPtr.Zero)
                {
                    break;
                }
            }

            SetMainWindowHandle(sender, p.MainWindowHandle);
        }
    }
}
