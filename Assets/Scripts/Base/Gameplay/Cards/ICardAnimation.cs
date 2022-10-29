using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public interface ICardAnimation
    {
        public void MoveTo(Vector3 position, Quaternion rotation, float time, Order order, System.Action onDone = null);
        public void FlyToHand(Vector3 position, Quaternion rotation, float time, Order order, System.Action onDone = null);
        public void DiscardMove(Vector3 position, Quaternion rotation, float time, Order order, System.Action callback = null);

        public enum Order
        {
            Override,
            IfNotPlaying
        }
    }
}
