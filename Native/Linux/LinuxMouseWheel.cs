using PenDragScroll.Native.Linux.Evdev;

namespace PenDragScroll.Native.Linux;

internal sealed class LinuxMouseWheel : IDisposable
{
    private readonly EvdevDevice _device;
    private bool _dirty;

    public LinuxMouseWheel()
    {
        _device = new EvdevDevice("OpenTabletDriver Pen Drag Scroll");
        _device.EnableTypeCodes(
            EventType.EV_REL,
            EventCode.REL_WHEEL,
            EventCode.REL_WHEEL_HI_RES,
            EventCode.REL_HWHEEL,
            EventCode.REL_HWHEEL_HI_RES);

        _device.Initialize();
    }

    public void ScrollVertically(int amount)
    {
        _dirty = true;
        _device.Write(EventType.EV_REL, EventCode.REL_WHEEL_HI_RES, amount);
    }

    public void ScrollHorizontally(int amount)
    {
        _dirty = true;
        _device.Write(EventType.EV_REL, EventCode.REL_HWHEEL_HI_RES, amount);
    }

    public void Flush()
    {
        if (_dirty)
        {
            _device.Sync();
            _dirty = false;
        }
    }

    public void Dispose() => _device.Dispose();
}
