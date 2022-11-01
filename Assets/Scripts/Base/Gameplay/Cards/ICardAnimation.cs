using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public interface ICardAnimation
    {
        public void DoMove(Types type, Vector3 position, Quaternion rotation, float time, Order order, System.Action onDone = null);


        public enum Types
        {
            MoveTo,
            FlyTo,
            OtherPlace,
            DiscardMove
        }
        public enum Order
        {
            Override,
            IfNotPlaying
        }
    }
}
