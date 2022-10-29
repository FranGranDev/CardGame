using Cards.Data;

namespace Cards
{
    public interface ICardHolder
    {
        public int CardsCount { get; }
        public void Drop(IDragable card, DropCardData data);
        public void Drag(IDragable card, MoveInfo info);
    }


    public class DropCardData
    {
        public SenderTypes Sender { get; set; }
        public enum SenderTypes { Dect, Self, Table }
    }
}
