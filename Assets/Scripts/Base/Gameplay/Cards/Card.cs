using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cards
{
    [RequireComponent(typeof(ICardAnimation))]
    public abstract class Card : MonoBehaviour, IDragable
    {
        public abstract bool Visible { get; set; }
        public abstract bool Takable { get; set; }
        public abstract int Comparator { get; }
        public abstract CardInfo Info { get; protected set; }
        public Transform Body { get => transform; }
        public ICardAnimation Animations { get; protected set; }
        public PlayerWrapper Owner { get; set; }


        #region Callbacks

        public CardAction OnTaken { get; set; }
        public CardAction OnDropped { get; set; }

        #endregion

        private void Awake()
        {
            Initilize();
        }

        public virtual void Initilize()
        {
            Animations = GetComponent<ICardAnimation>();
        }

        public abstract void Return();
        public abstract void Interact(MoveInfo info);
        public abstract void Take(MoveInfo info);
        public abstract void Drop(ICardHolder holder, MoveInfo info);
        public abstract void Drag(ICardHolder holder, MoveInfo info);
        public abstract void Drag(MoveInfo info);

        public abstract void Accept(ICardVisitor visitor, object data = null);


        public void MoveTo(Vector3 position, Quaternion rotation, float time, ICardAnimation.Order order, Action onDone = null)
        {
            Animations.MoveTo(position, rotation, time, order, onDone);
        }
        public void FlyTo(Vector3 position, Quaternion rotation, float time, ICardAnimation.Order order, Action onDone = null)
        {
            Animations.FlyToHand(position, rotation, time, order, onDone);
        }
        public void DiscardMove(Vector3 position, Quaternion rotation, float time, ICardAnimation.Order order, Action onDone = null)
        {
            Animations.DiscardMove(position, rotation, time, order, onDone);
        }
    }
}
