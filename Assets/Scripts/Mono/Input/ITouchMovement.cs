using UnityEngine;

namespace TouchInput
{
    public interface ITouchMovement
    {

        public delegate void TouchAction(Vector2 screenPos);
        public delegate void DragAction(Vector2 firstTap, Vector2 secondTap);

        TouchAction OnClick { get; set; }
        TouchAction OnStartSelect { get; set; }

        DragAction OnEndSelect { get; set; }
        DragAction OnDrag { get; set; }
    }
}
