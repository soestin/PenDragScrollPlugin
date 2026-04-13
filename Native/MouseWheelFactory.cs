using System.Runtime.InteropServices;
using PenDragScroll.Native.Linux;
using PenDragScroll.Native.Windows;

namespace PenDragScroll.Native;

internal static class MouseWheelFactory
{
    public static IMouseWheel Create()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return new WindowsMouseWheel();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return new LinuxMouseWheel();

        throw new PlatformNotSupportedException("Pen Drag Scroll currently supports Linux and Windows.");
    }
}
