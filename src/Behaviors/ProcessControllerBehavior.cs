using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Core.Plugins;
using NP.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NP.Avalonia.Visuals.Behaviors
{
    public static class ProcessControllerBehavior
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
            AvaloniaProperty.RegisterAttached<Control, AvaloniaObject, string>
            (
                "ProcessExePath"
            );
        #endregion ProcessExePath Attached Avalonia Property

        #region TheProcess Attached Avalonia Property
        public static Process? GetTheProcess(AvaloniaObject obj)
        {
            return obj.GetValue(TheProcessProperty);
        }

        private static void SetTheProcess(AvaloniaObject obj, Process? value)
        {
            obj.SetValue(TheProcessProperty, value);
        }

        public static readonly AttachedProperty<Process?> TheProcessProperty =
            AvaloniaProperty.RegisterAttached<Control, AvaloniaObject, Process?>
            (
                "TheProcess"
            );
        #endregion TheProcess Attached Avalonia Property


        #region MainWindowHandle Attached Avalonia Property
        public static IntPtr GetMainWindowHandle(AvaloniaObject obj)
        {
            return obj.GetValue(MainWindowHandleProperty);
        }

        public static void SetMainWindowHandle(AvaloniaObject obj, IntPtr value)
        {
            obj.SetValue(MainWindowHandleProperty, value);
        }

        public static readonly AttachedProperty<IntPtr> MainWindowHandleProperty =
            AvaloniaProperty.RegisterAttached<AvaloniaObject, AvaloniaObject, IntPtr>
            (
                "MainWindowHandle",
                IntPtr.Zero
            );
        #endregion MainWindowHandle Attached Avalonia Property

        static ProcessControllerBehavior()
        {
            ProcessExePathProperty.Changed.Subscribe(OnStartProcesPathPropertyChanged);

            TheProcessProperty.Changed.Subscribe(OnProcessChanged);
        }

        private static void OnProcessChanged(AvaloniaPropertyChangedEventArgs<Process?> obj)
        {
            Process? oldProcess = obj.OldValue.Value;

            oldProcess?.DestroyProcess();

            Control sender = (Control) obj.Sender;

            Process? p = obj.NewValue.Value;

            if (p != null)
            {

                void OnProcessExited(object? procObj, EventArgs e)
                {
                    Process proc = (Process) procObj!;

                    proc.Exited -= OnProcessExited;

                    if (GetTheProcess(sender) != null)
                    {
                        SetTheProcess(sender, null);
                    }
                }

                p.Exited += OnProcessExited;
            }
        }

        private static async void OnStartProcesPathPropertyChanged(AvaloniaPropertyChangedEventArgs<string> changeArgs)
        {
            var sender = (AvaloniaObject) changeArgs.Sender;

            string exePath = changeArgs.NewValue.Value;

            if (exePath == null)
            {
                SetTheProcess(sender, null);
                return;
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo(exePath);

            Process p = Process.Start(processStartInfo)!;

            SetTheProcess(sender, p);

            while (true)
            {
                await Task.Delay(200);

                if (p.MainWindowHandle != IntPtr.Zero)
                {
                    break;
                }
            }

            SetMainWindowHandle(sender, p.MainWindowHandle);
        }

        public static void DestroyProcess(this Process? p)
        {
            if (p == null)
                return;

            p.Kill();

            p.Dispose();
        }

        public static void DestroyProcess(this AvaloniaObject avaloniaObject)
        {
            Process? p = GetTheProcess(avaloniaObject);

            if (p != null)
            {
                p.DestroyProcess();
            }
        }
    }
}
