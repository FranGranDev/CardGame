using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cards
{
    public delegate void CardAction(IDragable card);

    public interface IDragable
    {
        public bool Takable { get; set; }
        public Transform Body { get; }

        public void Interact(MoveInfo info);
        public void Take(MoveInfo info);
        public void Drop(ICardHolder holder, MoveInfo info);
        public void Drag(MoveInfo info);

        public void Accept(ICardVisitor visitor);


        public CardAction OnTaken { get; set; }
        public CardAction OnDropped { get; set; }
    }

    public struct MoveInfo
    {
        public MoveInfo(RaycastHit hit, float offset = 0.1f)
        {
            position = hit.point + hit.normal * offset;
            normal = hit.normal;
        }

        public Vector3 position;
        public Vector3 normal;
    }
}
