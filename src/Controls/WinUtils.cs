using Avalonia.Controls;
using Avalonia.Platform;
using System;

namespace NP.Avalonia.Visuals.Controls
{
    public static class WinUtils
    {
        public static IPlatformHandle GetPlatformHandle(this Window window)
        {
            return window.PlatformImpl.Handle;
        }

        public static IntPtr GetWinHandle(this Window window)
        {
            return window.GetPlatformHandle().Handle;
        }
    }
}
