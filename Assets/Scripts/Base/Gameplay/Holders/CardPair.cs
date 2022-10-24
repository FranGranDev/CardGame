namespace Cards
{
    public class CardPair
    {
        public CardPair(Card first, ICardComparator comparator)
        {
            Defender = first;
            this.comparator = comparator;

            Done = false;
        }

        private ICardComparator comparator;

        public Card Defender { get; private set; }
        public Card Attacker { get; private set; }


        public bool TryBeat(Card other)
        {
            Attacker = other;

            Done = comparator.Compare(Defender, Attacker);
            return Done;
        }
        public bool Done { get; private set; }
    }
}