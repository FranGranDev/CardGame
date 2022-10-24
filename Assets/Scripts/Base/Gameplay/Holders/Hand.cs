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

        public CardFactory cardFactory;

        private List<Card> cards = new List<Card>();


        public List<Card> Cards => cards;


        #region Initilize
        private void Awake()
        {
            Initilize();
        }

        private void Initilize()
        {
            GetCards();
        }
        private void GetCards()
        {
            IDragable[] temp = cardPlace.GetComponentsInChildren<IDragable>();
            foreach(IDragable card in temp)
            {
                Drop(card);
            }

            SortCards();
        }
        private void CreateRandomCards()
        {
            for(int i = 0; i < 6; i++)
            {

            }
        }

        public void InitilizeCards(PlayerWrapper player)
        {
            foreach(Card card in cards)
            {
                card.Owner = player;
            }
        }

        #endregion

        #region CardMoving

        public void Drop(IDragable card)
        {
            card.Accept(this);
        }
        public void Visit(Card card)
        {
            AddCard(card);

            card.Takable = true;

            SortCards();
        }

        private void SortCards()
        {
            float count = 0.5f;
            Vector3 offsetY = Vector3.zero;
            foreach(Card card in cards)
            {
                float ratio = count / (float)cards.Count;
                Vector3 offsetZ = cardPlace.forward * placeCurve.Evaluate(ratio) * placeOffsetZ;
                card.Body.position = Vector3.Lerp(leftPoint.position, rightPoint.position, ratio) + offsetY + offsetZ;
                card.Body.rotation = Quaternion.LookRotation(Vector3.Lerp(leftPoint.forward, rightPoint.forward, ratio), cardPlace.up);
                offsetY += cardPlace.up * 0.1f;

                count++;
            }
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
