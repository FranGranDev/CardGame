using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cards
{
    public abstract class Card : MonoBehaviour, IDragable
    {
        public bool Takable { get; set; } = true;
        public Transform Body { get => transform; }


        #region Callbacks

        public CardAction OnTaken { get; set; }
        public CardAction OnDropped { get; set; }

        #endregion
        
        public PlayerWrapper Owner { get; set; }

        public abstract void Interact(MoveInfo info);
        public abstract void Take(MoveInfo info);
        public abstract void Drop(ICardHolder holder, MoveInfo info);
        public abstract void Drag(MoveInfo info);

        public abstract void Accept(ICardVisitor visitor);
    }
}
