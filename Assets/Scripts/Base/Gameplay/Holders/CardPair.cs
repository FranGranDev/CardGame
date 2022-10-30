namespace Cards
{
    public class CardPair
    {
        public CardPair(Card first, ICardComparator comparator)
        {
            Attacker = first;
            Attacker.Takable = false;
            this.comparator = comparator;

            Done = false;
        }

        private ICardComparator comparator;

        public bool Done { get; private set; }
        public Card Attacker { get; private set; }
        public Card Defender { get; private set; }


        public bool TryBeat(Card other)
        {
            if (Done)
                return false;

            Done = comparator.CanBeat(other, Attacker);
            if(Done)
            {
                Defender = other;
                Defender.Takable = false;
            }
            return Done;
        }
    }
}