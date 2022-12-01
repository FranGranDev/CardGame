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


        public bool CanBeat(Card other)
        {
            return comparator.CanBeat(other, Attacker);
        }
        public bool Beat(Card other)
        {
            Done = comparator.CanBeat(other, Attacker);
            if(Done)
            {
                Defender = other;
                Defender.Takable = false;
            }
            return Done;
        }
        public void Destroy()
        {
            if (Attacker)
            {
                Attacker.Destroy();
                Attacker = null;
            }
            if(Defender)
            {
                Defender.Destroy();
                Defender = null;
            }
        }
    }
}