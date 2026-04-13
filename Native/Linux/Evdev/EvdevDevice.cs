namespace PenDragScroll.Native.Linux.Evdev;

internal sealed class EvdevDevice : IDisposable
{
    private nint _device;
    private nint _uinputDevice;

    public EvdevDevice(string deviceName)
    {
        _device = Evdev.libevdev_new();
        Evdev.libevdev_set_name(_device, deviceName);
    }

    public bool CanWrite { get; private set; }

    public ERRNO Initialize()
    {
        int err = Evdev.libevdev_uinput_create_from_device(_device, Evdev.LIBEVDEV_UINPUT_OPEN_MANAGED, out _uinputDevice);
        CanWrite = err == 0;
        return (ERRNO)(-err);
    }

    public void EnableType(EventType type) => Evdev.libevdev_enable_event_type(_device, (uint)type);

    public void EnableCode(EventType type, EventCode code) => Evdev.libevdev_enable_event_code(_device, (uint)type, (uint)code, nint.Zero);

    public void EnableTypeCodes(EventType type, params EventCode[] codes)
    {
        EnableType(type);
        foreach (var code in codes)
            EnableCode(type, code);
    }

    public int Write(EventType type, EventCode code, int value)
        => CanWrite ? Evdev.libevdev_uinput_write_event(_uinputDevice, (uint)type, (uint)code, value) : int.MinValue;

    public bool Sync() => Write(EventType.EV_SYN, EventCode.SYN_REPORT, 0) == 0;

    public void Dispose()
    {
        CanWrite = false;
        if (_uinputDevice != nint.Zero)
        {
            Evdev.libevdev_uinput_destroy(_uinputDevice);
            _uinputDevice = nint.Zero;
            _device = nint.Zero;
        }
    }
}
