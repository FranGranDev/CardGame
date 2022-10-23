using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchInput
{
    public class TouchInput : MonoBehaviour, ITouchMovement
    {
        private const float MIN_DST_FOR_BOX_SELECT = 50;

        public enum StateTypes { Null, StartTap, Drag}
        [SerializeField] private StateTypes state;

        public ITouchMovement.TouchAction OnClick { get; set; }
        public ITouchMovement.TouchAction OnStartSelect { get; set; }
        public ITouchMovement.DragAction OnEndSelect { get; set; }
        public ITouchMovement.DragAction OnDrag { get; set; }


        private bool touched;
        private Vector3 startTapPosition;

        private void MouseInput()
        {
            if (!touched && Input.GetMouseButtonDown(0))
            {
                OnStartTap(Input.mousePosition);
            }

            if (touched && Input.GetMouseButtonUp(0))
            {
                OnEndTap(Input.mousePosition);
            }

            if (touched)
            {
                switch (state)
                {
                    case StateTypes.Drag:
                        OnDrag?.Invoke(startTapPosition, Input.mousePosition);
                        break;
                }
            }
        }

        private IEnumerator WaitForDragSelect()
        {
            while ((startTapPosition - Input.mousePosition).magnitude < MIN_DST_FOR_BOX_SELECT)
            {
                if (!touched)
                {
                    yield break;
                }
                yield return new WaitForFixedUpdate();
            }

            state = StateTypes.Drag;
            OnStartSelect?.Invoke(Input.mousePosition);
            yield break;
        }
        private void OnStartTap(Vector2 position)
        {
            startTapPosition = position;
            state = StateTypes.StartTap;
            touched = true;
        }
        private void OnEndTap(Vector2 position)
        {
            switch(state)
            {
                case StateTypes.Drag:
                    OnEndSelect?.Invoke(startTapPosition, position);
                    break;
                case StateTypes.StartTap:
                    OnClick?.Invoke(position);
                    break;
            }

            state = StateTypes.Null;
            touched = false;
        }

        private void Update()
        {
            MouseInput();
        }

        public struct TapInfo 
        {
            public TapInfo(Vector2 point, StateTypes state)
            {
                this.point = point;
                this.state = state;
            }

            public readonly StateTypes state;
            public readonly Vector2 point;
            
        }
    }
}
