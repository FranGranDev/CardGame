using UnityEngine;
using System.Collections.Generic;


namespace Cards
{
    public class PairPoint
    {
        public PairPoint(Vector2Int key, Vector3 position, Transform point)
        {
            this.key = key;
            this.position = position;
            this.point = point;

            cardPairs = new Stack<CardPair>();
        }

        public readonly Vector2Int key;
        public readonly Vector3 position;
        public readonly Transform point;
        private Stack<CardPair> cardPairs;

        public Vector3 Position
        {
            get
            {
                return position + Vector3.up * cardPairs.Count * 0.25f;
            }
        }

        public CardPair Pair
        {
            get
            {
                return cardPairs.Count == 0 ? null : cardPairs.Peek();
            }
        }
        public bool CanPutAttack
        {
            get => Pair == null || Pair.Done;
        }
        public bool CanPutDefend
        {
            get => Pair != null && !Pair.Done;
        }


        public void Clear()
        {
            cardPairs.Clear();
        }
        public void CreateNewPair(CardPair pair)
        {
            cardPairs.Push(pair);
        }
    }
}
