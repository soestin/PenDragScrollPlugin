using System.Numerics;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;
using PenDragScroll.Native.Linux;
using PenDragScroll.State;

namespace PenDragScroll.Filters;

[PluginName("Pen Drag Scroll")]
[SupportedPlatform(PluginPlatform.Linux)]
public sealed class PenDragScrollFilter : IPositionedPipelineElement<IDeviceReport>, IDisposable
{
    private readonly LinuxMouseWheel _wheel = new();
    private bool _scrolling;
    private Vector2 _anchorPosition;
    private Vector2 _lastPosition;
    private float _verticalAccumulator;
    private float _horizontalAccumulator;

    public event Action<IDeviceReport>? Emit;

    [TabletReference]
    public TabletReference? Tablet { get; set; }

    public PipelinePosition Position => PipelinePosition.PostTransform;

    [Property("Anchor Cursor"), DefaultPropertyValue(true),
     ToolTip("Pen Drag Scroll:\n\nWhen enabled, the cursor is held in place while the drag-scroll modifier is active.")]
    public bool AnchorCursor { get; set; } = true;

    [Property("Vertical Units Per Pixel"), DefaultPropertyValue(30f), Unit("u/px"),
     ToolTip("Pen Drag Scroll:\n\nHow much vertical high-resolution scroll to emit for each pixel of pen movement.\n120 units is roughly one traditional wheel tick on Linux/Windows.")]
    public float VerticalUnitsPerPixel { get; set; } = 30f;

    [Property("Horizontal Units Per Pixel"), DefaultPropertyValue(0f), Unit("u/px"),
     ToolTip("Pen Drag Scroll:\n\nHow much horizontal high-resolution scroll to emit for each pixel of pen movement.\nSet to 0 to disable horizontal drag-scrolling.")]
    public float HorizontalUnitsPerPixel { get; set; } = 0f;

    [Property("Invert Vertical"), DefaultPropertyValue(false),
     ToolTip("Pen Drag Scroll:\n\nInvert vertical drag-scroll direction.")]
    public bool InvertVertical { get; set; }

    [Property("Invert Horizontal"), DefaultPropertyValue(false),
     ToolTip("Pen Drag Scroll:\n\nInvert horizontal drag-scroll direction.")]
    public bool InvertHorizontal { get; set; }

    public void Consume(IDeviceReport value)
    {
        if (value is ITabletReport tabletReport)
            ProcessTabletReport(tabletReport);

        Emit?.Invoke(value);
    }

    private void ProcessTabletReport(ITabletReport report)
    {
        bool active = DragScrollStateStore.IsActive(Tablet);

        if (!active)
        {
            Reset();
            return;
        }

        if (!_scrolling)
        {
            _scrolling = true;
            _anchorPosition = report.Position;
            _lastPosition = report.Position;
            _verticalAccumulator = 0;
            _horizontalAccumulator = 0;
        }

        var delta = report.Position - _lastPosition;
        _lastPosition = report.Position;

        if (AnchorCursor)
            report.Position = _anchorPosition;

        // Default mapping: moving pen down/right scrolls down/right.
        float verticalDelta = -delta.Y * VerticalUnitsPerPixel;
        float horizontalDelta = -delta.X * HorizontalUnitsPerPixel;

        if (InvertVertical)
            verticalDelta *= -1;

        if (InvertHorizontal)
            horizontalDelta *= -1;

        _verticalAccumulator += verticalDelta;
        _horizontalAccumulator += horizontalDelta;

        bool dirty = false;

        int verticalWhole = (int)MathF.Truncate(_verticalAccumulator);
        if (verticalWhole != 0)
        {
            _wheel.ScrollVertically(verticalWhole);
            _verticalAccumulator -= verticalWhole;
            dirty = true;
        }

        int horizontalWhole = (int)MathF.Truncate(_horizontalAccumulator);
        if (horizontalWhole != 0)
        {
            _wheel.ScrollHorizontally(horizontalWhole);
            _horizontalAccumulator -= horizontalWhole;
            dirty = true;
        }

        if (dirty)
            _wheel.Flush();
    }

    private void Reset()
    {
        _scrolling = false;
        _verticalAccumulator = 0;
        _horizontalAccumulator = 0;
    }

    public void Dispose() => _wheel.Dispose();
}
