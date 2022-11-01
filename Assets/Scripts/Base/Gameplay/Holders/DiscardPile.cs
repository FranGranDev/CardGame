using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class DiscardPile : MonoBehaviour, ICardHolder, ICardVisitor
    {
        [Header("Settings")]
        [SerializeField] private Transform centerPoint;
        [SerializeField] private float offsetRadius;
        
        private List<Card> cards = new List<Card>();

        public int CardsCount => cards.Count;


        public void Drop(List<CardPair> pairs, System.Action onDone = null)
        {
            foreach(CardPair pair in pairs)
            {
                if(pair.Attacker)
                {
                    Drop(pair.Attacker, new DropCardData { Sender = DropCardData.SenderTypes.Table});
                }
                if(pair.Defender)
                {
                    Drop(pair.Defender, new DropCardData { Sender = DropCardData.SenderTypes.Table });
                }
            }
        }
        public void Drop(IDragable card, DropCardData data)
        {
            card.Accept(this, data);
        }
        public void Visit(Card card, object data = null)
        {
            try
            {
                DropCardData info = data as DropCardData;

                switch(info.Sender)
                {
                    case DropCardData.SenderTypes.Table:
                        cards.Add(card);

                        Vector3 position = centerPoint.position + Random.insideUnitSphere * offsetRadius;
                        position.y = CardsCount * 0.1f;
                        Quaternion rotation = Quaternion.Euler(0, Random.Range(180, 720), 0);

                        card.DoMove(ICardAnimation.Types.DiscardMove, position, rotation, 0.5f, ICardAnimation.Order.Override);
                        break;
                }
            }
            catch { }
        }
        public void Drag(IDragable card, MoveInfo info)
        {
            card.Body.transform.position = info.position;
            card.Body.up = info.normal;
        }
    }
}
