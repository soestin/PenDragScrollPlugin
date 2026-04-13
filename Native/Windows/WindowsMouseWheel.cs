using System.Runtime.InteropServices;

namespace PenDragScroll.Native.Windows;

internal sealed class WindowsMouseWheel : IMouseWheel
{
    private INPUT _input;
    private bool _dirty;

    public WindowsMouseWheel()
    {
        _input = new INPUT
        {
            type = 0,
            U = new InputUnion
            {
                mi = new MOUSEINPUT
                {
                    dx = 0,
                    dy = 0,
                    mouseData = 0,
                    dwFlags = 0,
                    time = 0,
                    dwExtraInfo = nint.Zero
                }
            }
        };
    }

    public void ScrollVertically(int amount)
    {
        _dirty = true;
        _input.U.mi.dwFlags |= MouseEventFlags.WHEEL | MouseEventFlags.VIRTUALDESK;
        _input.U.mi.mouseData = amount;
    }

    public void ScrollHorizontally(int amount)
    {
        _dirty = true;
        _input.U.mi.dwFlags |= MouseEventFlags.HWHEEL | MouseEventFlags.VIRTUALDESK;
        _input.U.mi.mouseData = amount;
    }

    public void Flush()
    {
        if (!_dirty)
            return;

        var inputs = new[] { _input };
        SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<INPUT>());

        _input.U.mi.dwFlags = 0;
        _input.U.mi.mouseData = 0;
        _dirty = false;
    }

    public void Dispose() {}

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint cInputs, INPUT[] pInputs, int cbSize);

    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type;
        public InputUnion U;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)]
        public MOUSEINPUT mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public int mouseData;
        public uint dwFlags;
        public uint time;
        public nint dwExtraInfo;
    }

    private static class MouseEventFlags
    {
        public const uint WHEEL = 0x0800;
        public const uint HWHEEL = 0x01000;
        public const uint VIRTUALDESK = 0x4000;
    }
}
