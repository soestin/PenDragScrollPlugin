using System.Runtime.InteropServices;

namespace PenDragScroll.Native.Linux.Evdev;

internal static class Evdev
{
    private const string LibEvdev = "libevdev.so.2";

    [DllImport(LibEvdev)]
    public static extern nint libevdev_new();

    [DllImport(LibEvdev)]
    public static extern void libevdev_set_name(nint dev, string name);

    [DllImport(LibEvdev)]
    public static extern int libevdev_enable_event_type(nint dev, uint type);

    [DllImport(LibEvdev)]
    public static extern int libevdev_enable_event_code(nint dev, uint type, uint code, nint data);

    [DllImport(LibEvdev)]
    public static extern int libevdev_uinput_create_from_device(nint dev, int uinputFd, out nint uinputDev);

    [DllImport(LibEvdev)]
    public static extern void libevdev_uinput_destroy(nint uinputDev);

    [DllImport(LibEvdev)]
    public static extern int libevdev_uinput_write_event(nint uinputDev, uint type, uint code, int value);

    public const int LIBEVDEV_UINPUT_OPEN_MANAGED = -2;
}
