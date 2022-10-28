using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cards.Data;


namespace Cards
{ 
    public class Hand : MonoBehaviour, ICardHolder, ICardVisitor
    {
        [Header("Settings")]
        [SerializeField] private AnimationCurve placeCurve;
        [SerializeField] private float placeOffsetZ;
        [Header("Components")]
        [SerializeField] private Transform cardPlace;
        [Header("Points")]
        [SerializeField] private Transform leftPoint;
        [SerializeField] private Transform rightPoint;
        [Header("Links")]
        [SerializeField] private CardFactory cardFactory;


        private List<Card> cards = new List<Card>();

        public int CardsCount => cards.Count;
        public List<Card> Cards => cards;


        #region Initilize
        public void Initilize()
        {

        }


        #endregion

        #region CardMoving

        public void Drop(IDragable card, DropCardData data)
        {
            card.Accept(this, data);
        }
        public void Visit(Card card, object data = null)
        {
            try
            {
                DropCardData info = data as DropCardData;

                AddCard(card);

                switch(info.Sender)
                {
                    case DropCardData.SenderTypes.Self:
                        card.Takable = true;
                        SortCards();
                        break;
                    case DropCardData.SenderTypes.Dect:
                        card.Takable = false;
                        int index = Cards.IndexOf(card);
                        card.FlyTo(CardPosition(index), CardRotation(index), 1f, ICardAnimation.Order.Override, () =>
                        {
                            card.Takable = true;
                            SortCards();
                        });
                        break;
                    case DropCardData.SenderTypes.Others:

                        break;
                }

            }
            catch{ }
        }

        private void SortCards()
        {   
            cards.OrderBy(x => x.transform.position.x);

            int i = 0;
            foreach (Card card in cards)
            {
                Vector3 position = CardPosition(i);
                Quaternion rotation = CardRotation(i);

                card.MoveTo(position, rotation, 0.25f, ICardAnimation.Order.IfNotPlaying);

                i++;
            }
        }

        private Vector3 CardPosition(int cardIndex)
        {
            float ratio = ((float)cardIndex + 0.5f) / (float)cards.Count;
            Vector3 offsetZ = cardPlace.forward * placeCurve.Evaluate(ratio) * placeOffsetZ;
            Vector3 offsetY = cardPlace.up * cardIndex * 0.1f;

            return Vector3.Lerp(leftPoint.position, rightPoint.position, ratio) + offsetY + offsetZ;
        }
        private Quaternion CardRotation(int cardIndex)
        {
            float ratio = ((float)cardIndex + 0.5f) / (float)cards.Count;

            return Quaternion.LookRotation(Vector3.Lerp(leftPoint.forward, rightPoint.forward, ratio), cardPlace.up);
        }

        #endregion

        #region CardAction

        private void AddCard(Card card)
        {
            if(cards.Contains(card))
            {
                Debug.LogError($"Card already exists", card.Body);
                return;
            }

            cards.Add(card);
            card.Body.transform.parent = cardPlace;
            card.OnTaken += RemoveCard;
        }
        private void RemoveCard(IDragable card)
        {
            try
            {
                if (!cards.Contains((Card)card))
                {
                    Debug.LogError($"Card doesnt exists", card.Body);
                    return;
                }

                cards.Remove((Card)card);
                card.Body.transform.parent = null;
                card.OnTaken -= RemoveCard;

                SortCards();
            }
            catch
            {
                Debug.Log("IDragable is not a card class!");
            }
        }

        #endregion
    }
}
