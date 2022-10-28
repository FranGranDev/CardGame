using Cards.Data;

namespace Cards
{
    public interface ICardHolder
    {
        public int CardsCount { get; }
        public void Drop(IDragable card, DropCardData data);
    }


    public class DropCardData
    {
        public SenderTypes Sender { get; set; }
        public enum SenderTypes { Dect, Self, Others }
    }
}
