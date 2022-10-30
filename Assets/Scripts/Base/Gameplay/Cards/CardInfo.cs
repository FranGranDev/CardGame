namespace Cards
{
    [System.Serializable]
    public class CardInfo
    {
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


        public readonly int index;
        public readonly int suit;
        public readonly int skin;

        public override bool Equals(object obj)
        {
            if(obj is CardInfo)
            {
                CardInfo other = (CardInfo)obj;
                return other.index == index && other.suit == suit;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return 0;
        }
    }
}
