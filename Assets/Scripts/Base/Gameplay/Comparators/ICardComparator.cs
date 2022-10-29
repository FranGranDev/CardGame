namespace Cards
{
    public interface ICardComparator
    {
        public bool CanBeat(Card defender, Card attacker);
        public bool CanPut(Card attacker, CardPair pair);
    }
}
