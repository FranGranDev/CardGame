using System.Collections;
using UnityEngine;

namespace TouchInput
{
    public class TouchInput : MonoBehaviour, ITouchMovement
    {
        private const float MIN_DST_FOR_DRAG = 10;
        private const float MIN_TIME_FOR_DRAG = 0.25f;

        [SerializeField] private StateTypes state;

        public ITouchMovement.TouchAction OnClick { get; set; }
        public ITouchMovement.TouchAction OnStartDrag { get; set; }
        public ITouchMovement.TouchAction OnEndDrag { get; set; }
        public ITouchMovement.TouchAction OnDrag { get; set; }


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
        }
        private void InputExecute()
        {
            if (touched)
            {
                switch (state)
                {
                    case StateTypes.Drag:
                        OnDrag?.Invoke(new TapInfo(0, startTapPosition, Input.mousePosition, state));
                        break;
                }
            }
        }

        private IEnumerator WaitForDragSelect()
        {
            float time = 0;

            while ((startTapPosition - Input.mousePosition).magnitude < MIN_DST_FOR_DRAG && time < MIN_TIME_FOR_DRAG)
            {
                if (!touched)
                {
                    yield break;
                }
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            state = StateTypes.Drag;
            OnStartDrag?.Invoke(new TapInfo(0, startTapPosition, Input.mousePosition, state));
            yield break;
        }
        private void OnStartTap(Vector2 position)
        {
            startTapPosition = position;
            state = StateTypes.StartTap;
            touched = true;

            StartCoroutine(WaitForDragSelect());
        }
        private void OnEndTap(Vector2 position)
        {
            switch(state)
            {
                case StateTypes.Drag:
                    OnEndDrag?.Invoke(new TapInfo(0, startTapPosition, position, state));
                    break;
                case StateTypes.StartTap:
                    OnClick?.Invoke(new TapInfo(0, startTapPosition, position, state));
                    break;
            }

            state = StateTypes.Null;
            touched = false;
        }

        private void Update()
        {
            MouseInput();
        }
        private void FixedUpdate()
        {
            InputExecute();
        }

    }
}