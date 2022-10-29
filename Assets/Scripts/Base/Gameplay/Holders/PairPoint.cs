using UnityEngine;


namespace Cards
{
    class PairPoint
    {
        public PairPoint(Vector2Int key, Vector3 position, Transform point)
        {
            this.key = key;
            this.position = position;
            this.point = point;
        }

        public readonly Vector2Int key;
        public readonly Vector3 position;
        public readonly Transform point;

        public CardPair Cards
        {
            get => cards;
            set
            {
                cards = value;
            }
        }
        private CardPair cards;
    }
}
