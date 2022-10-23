using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UILoopRotation : MonoBehaviour, IAddictive
    {
        [SerializeField] private float speed = 1;

        public bool IsPlaying { private get; set; }

        private void Start()
        {
            IsPlaying = false;
        }
        private void FixedUpdate()
        {
            if (IsPlaying)
            {
                transform.Rotate(Vector3.forward, speed * Time.fixedDeltaTime);
            }
        }
    }
}