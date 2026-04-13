using System.Collections.Concurrent;
using OpenTabletDriver.Plugin.Tablet;

namespace PenDragScroll.State;

internal static class DragScrollStateStore
{
    private static readonly ConcurrentDictionary<string, bool> ActiveStates = new();

    public static bool IsActive(TabletReference? tablet)
        => ActiveStates.TryGetValue(GetKey(tablet), out var active) && active;

    public static void SetActive(TabletReference? tablet, bool active)
        => ActiveStates[GetKey(tablet)] = active;

    private static string GetKey(TabletReference? tablet)
        => tablet?.Properties?.Name ?? "default";
}
