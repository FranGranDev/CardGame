using UnityEngine;
using System.Collections.Generic;


namespace Cards
{
    class PairPoint
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
                float height = 0;
                foreach(CardPair pair in cardPairs)
                {
                    height += 0.1f;
                    if (pair.Done)
                    {
                        height += 0.15f;
                    }
                }

                return position + Vector3.up * height;
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

        public void CreateNewPair(CardPair pair)
        {
            cardPairs.Push(pair);
        }
    }
}
