namespace PenDragScroll.Native.Linux.Evdev;

internal enum EventCode : uint
{
    SYN_REPORT = 0x00,
    REL_HWHEEL = 0x06,
    REL_WHEEL = 0x08,
    REL_WHEEL_HI_RES = 0x0b,
    REL_HWHEEL_HI_RES = 0x0c,
}
