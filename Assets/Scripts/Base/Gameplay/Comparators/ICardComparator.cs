namespace Cards
{
    public interface ICardComparator
    {
        public bool Compare(Card defender, Card attacker);
    }
}
