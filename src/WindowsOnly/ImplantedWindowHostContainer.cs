using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Platform;
using NP.Avalonia.Visuals.Behaviors;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NP.Avalonia.Visuals.WindowsOnly
{
    public class ImplantedWindowHostContainer : Decorator
    {
        #region ImplantedWindowHandle Styled Avalonia Property
        public IntPtr ImplantedWindowHandle
        {
            get { return GetValue(ImplantedWindowHandleProperty); }
            set { SetValue(ImplantedWindowHandleProperty, value); }
        }

        public static readonly StyledProperty<IntPtr> ImplantedWindowHandleProperty =
            AvaloniaProperty.Register<ImplantedWindowHostContainer, IntPtr>
            (
                nameof(ImplantedWindowHandle),
                IntPtr.Zero
            );
        #endregion ImplantedWindowHandle Styled Avalonia Property

        #region ParentWindow Property
        private Window? _parentWindow;
        public Window? ParentWindow
        {
            get
            {
                return this._parentWindow;
            }
            private set
            {
                if (this._parentWindow == value)
                {
                    return;
                }

                if (_parentWindow != null)
                {
                    _parentWindow.Closed -= _parentWindow_Closed;
                }

                this._parentWindow = value;


                if (_parentWindow != null)
                {
                    _parentWindow.Closed += _parentWindow_Closed;
                }
            }
        }

        private async void _parentWindow_Closed(object? sender, EventArgs e)
        {
            this.DestroyProcess();
        }
        #endregion ParentWindow Property

        public ImplantedWindowHostContainer()
        {
            this.GetObservable(ImplantedWindowHandleProperty).Subscribe(OnImplantedWindowHandleChanged);
        }

        private void OnImplantedWindowHandleChanged(IntPtr implantedWindowHandle)
        {
            if (implantedWindowHandle != IntPtr.Zero)
            {
                this.Child = new ImplantedWindowHost(implantedWindowHandle);
            }
            else
            {
                this.Child = null;
            }
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            ParentWindow = (Window)e.Root;

            base.OnAttachedToVisualTree(e);
        }

        protected override async void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            if (ParentWindow != null)
            {
                this.DestroyProcess();
            }

            ParentWindow = null;

            base.OnDetachedFromVisualTree(e);
        }

        class ImplantedWindowHost : NativeControlHost
        {
            private IntPtr _implantedWindowHandle;

            public ImplantedWindowHost(IntPtr implantedWindowHandle)
            {
                _implantedWindowHandle = implantedWindowHandle;
            }

            protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
            {
                var parentWindow = (Window)e.Root;

                parentWindow.ImplantWindow(_implantedWindowHandle);


                // force refreshing the handle
                MethodInfo? methodInfo =
                    typeof(NativeControlHost)
                        .GetMethod("DestroyNativeControl", BindingFlags.Instance | BindingFlags.NonPublic);

                methodInfo?.Invoke(this, null);

                base.OnAttachedToVisualTree(e);
            }

            protected override async void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
            {
                base.OnDetachedFromVisualTree(e);
            }

            protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return new PlatformHandle(_implantedWindowHandle, "CTRL");
                }
                else
                {
                    return base.CreateNativeControlCore(parent);
                }
            }

            protected override void DestroyNativeControlCore(IPlatformHandle control)
            {
                if (control is INativeControlHostDestroyableControlHandle)
                {
                    base.DestroyNativeControlCore(control);
                }
            }
        }
    }
}
