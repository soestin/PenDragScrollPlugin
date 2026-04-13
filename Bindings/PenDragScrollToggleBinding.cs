using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using PenDragScroll.State;

namespace PenDragScroll.Bindings;

[PluginName("Pen Drag Scroll Toggle")]
public class PenDragScrollToggleBinding : IStateBinding
{
    public void Press(TabletReference tablet, IDeviceReport report)
        => DragScrollStateStore.SetActive(tablet, true);

    public void Release(TabletReference tablet, IDeviceReport report)
        => DragScrollStateStore.SetActive(tablet, false);

    public override string ToString() => "Pen Drag Scroll Toggle";
}
