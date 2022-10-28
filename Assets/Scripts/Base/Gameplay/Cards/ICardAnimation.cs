using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public interface ICardAnimation
    {
        public void MoveTo(Vector3 position, Quaternion rotation, float time, Order order, System.Action onDone = null);
        public void FlyTo(Vector3 position, Quaternion rotation, float time, Order order, System.Action onDone = null);

        public enum Order
        {
            Override,
            IfNotPlaying
        }
    }
}
