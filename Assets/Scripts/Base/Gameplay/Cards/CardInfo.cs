namespace Cards.Data
{
    [System.Serializable]
    public class CardInfo
    {
        public readonly int index;
        public readonly int suit;
        public readonly int skin;

        public CardInfo(int index, int suit, int skin)
        {
            this.index = index;
            this.suit = suit;
            this.skin = skin;
        }
        public CardInfo(int index, int suit)
        {
            this.index = index;
            this.suit = suit;
            this.skin = 0;
        }
    }
}
