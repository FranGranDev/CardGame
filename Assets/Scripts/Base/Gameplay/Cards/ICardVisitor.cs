using System.Collections;
using UnityEngine;

namespace Cards
{
    public interface ICardVisitor
    {
        public void Visit(Card card);
    }
}