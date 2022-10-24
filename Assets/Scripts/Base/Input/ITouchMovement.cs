using UnityEngine;

namespace TouchInput
{
    public interface ITouchMovement
    {

        public delegate void TouchAction(TapInfo info);

        TouchAction OnClick { get; set; }
        TouchAction OnStartDrag { get; set; }

        TouchAction OnEndDrag { get; set; }
        TouchAction OnDrag { get; set; }
    }


    public enum StateTypes { Null, StartTap, Drag }
    public struct TapInfo
    {
        public TapInfo(int index, Vector2 startPoint, StateTypes state)
        {
            this.index = index;
            this.startPoint = startPoint;
            this.endPoint = Vector2.zero;
            this.state = state;
        }
        public TapInfo(int index, Vector2 startPoint, Vector2 endPoint, StateTypes state)
        {
            this.index = index;
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.state = state;
        }

        public readonly int index;
        public readonly StateTypes state;
        public readonly Vector2 startPoint;
        public readonly Vector2 endPoint;

    }
}
