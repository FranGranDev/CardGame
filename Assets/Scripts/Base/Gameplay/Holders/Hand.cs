using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cards.Data;


namespace Cards
{ 
    public class Hand : MonoBehaviour, ICardHolder, ICardVisitor
    {
        [Header("State")]
        [SerializeField] private List<Card> cards;
        [SerializeField] private PlayerWrapper player;
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



        public int CardsCount => cards.Count;
        public List<Card> Cards => cards;


        #region Initilize
        public void Initilize(PlayerWrapper player)
        {
            this.player = player;

            cards = new List<Card>();
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
                int cardIndex = 0;

                switch (info.Sender)
                {
                    case DropCardData.SenderTypes.Self:
                        card.Takable = true;
                        SortCards(ICardAnimation.Order.Override);
                        break;
                    case DropCardData.SenderTypes.Dect:
                        card.Takable = false;
                        cardIndex = Cards.IndexOf(card);
                        card.DoMove(ICardAnimation.Types.FlyTo, CardPosition(cardIndex), CardRotation(cardIndex), 1f, ICardAnimation.Order.Override, () =>
                        {
                            card.Takable = true;
                            SortCards(ICardAnimation.Order.IfNotPlaying);
                        });
                        break;
                    case DropCardData.SenderTypes.Table:
                        card.Takable = false;
                        cardIndex = Cards.IndexOf(card);
                        card.DoMove(ICardAnimation.Types.FlyTo, CardPosition(cardIndex), CardRotation(cardIndex), 1f, ICardAnimation.Order.Override, () =>
                        {
                            card.Takable = true;
                            SortCards(ICardAnimation.Order.IfNotPlaying);
                        });
                        break;
                }

            }
            catch{ }
        }

        public void Drag(IDragable card, MoveInfo info)
        {
            card.Body.position = Vector3.Lerp(card.Body.position, info.position, 0.5f);
            float t = Mathf.InverseLerp(leftPoint.position.x, rightPoint.position.x, info.position.x);

            Quaternion rotation = Quaternion.LookRotation(Vector3.Lerp(leftPoint.forward, rightPoint.forward, t), cardPlace.up);

            card.Body.rotation = Quaternion.Lerp(card.Body.rotation, rotation, 0.5f);
        }

        private void SortCards(ICardAnimation.Order order = ICardAnimation.Order.IfNotPlaying)
        {
            //cards = cards.OrderBy(x => x.Comparator).ToList();
            cards = cards.OrderBy(x => x.Body.localPosition.x).ToList();

            int i = 0;
            foreach (Card card in cards)
            {
                Vector3 position = CardPosition(i);
                Quaternion rotation = CardRotation(i);

                card.Takable = false;
                card.DoMove(ICardAnimation.Types.MoveTo, position, rotation, 0.25f, order, () => card.Takable = true);

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
                Debug.LogError($"Card already exists", card.gameObject);
                return;
            }

            cards.Add(card);

            card.Owner = player;
            card.Body.transform.parent = cardPlace;
            card.OnDropped += RemoveCard;
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
                card.OnDropped -= RemoveCard;
                
                SortCards(ICardAnimation.Order.Override);
            }
            catch
            {
                Debug.Log("IDragable is not a card class!");
            }
        }

        #endregion
    }
}
