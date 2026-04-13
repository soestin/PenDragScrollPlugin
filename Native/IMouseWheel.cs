namespace PenDragScroll.Native;

internal interface IMouseWheel : IDisposable
{
    void ScrollVertically(int amount);
    void ScrollHorizontally(int amount);
    void Flush();
}
