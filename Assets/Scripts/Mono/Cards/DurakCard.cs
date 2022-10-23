using System.Collections;
using UnityEngine;

namespace Cards
{
    public class DurakCard : Card
    {

        public override void Interact(MoveInfo info)
        {
            
        }
        public override void Take(MoveInfo info)
        {
            OnTaken?.Invoke(this);
        }
        public override void Drop(ICardHolder holder, MoveInfo info)
        {
            transform.position = info.position;
            transform.up = info.normal;

            holder.Drop(this);
        }
        public override void Drag(MoveInfo info)
        {
            transform.position = info.position;
            transform.up = info.normal;
        }

        public override void Accept(ICardVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}