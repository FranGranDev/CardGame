using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cards
{
    public class Table : MonoBehaviour, ICardHolder, ICardVisitor
    {
        [Header("Placement")]
        [SerializeField] private Vector2 PlaceAttackRotationOffset;
        [SerializeField] private Vector2 PlaceDefendRotationOffset;
        [Space]
        [SerializeField] private Vector2 PlacePositionOffsetX;
        [SerializeField] private Vector2 PlacePositionOffsetY;
        [Header("Components")]
        [SerializeField] private Transform pointsPlace;

        private Deck deck;
        private DiscardPile discardPile;

        private List<PlayerWrapper> players;
        private int currantMove = 0;

        private Dictionary<Vector2Int, PairPoint> pairPoints;
        private List<CardPair> currantPairs;
        private ICardComparator comparator;


        public int CardsCount => 0;
        public int CurrantMove
        {
            get => currantMove;
            set
            {
                currantMove = value;
                if(currantMove >= players.Count)
                {
                    currantMove = 0;
                }
                else if(currantMove < 0)
                {
                    currantMove = 0;
                }
            }
        }
        public PlayerWrapper GetPlayer(Card card) 
        {
            return card.Owner;
        } //fix
        public PlayerWrapper Attacker
        {
            get => players[currantMove];
        }
        public PlayerWrapper Defender
        {
            get => players[currantMove + 1];
        }

        public float AvgPlaceAttackRotation
        {
            get => 0;
            //get => (PlaceAttackRotationOffset.x + PlaceAttackRotationOffset.y) / 2;
        }
        public float AvgPlaceDefendRotation
        {
            get => (PlaceDefendRotationOffset.x + PlaceDefendRotationOffset.y) / 2;
        }

        public Vector3 AvgPlaceAttackPosition
        {
            get => new Vector3(-(PlacePositionOffsetX.x + PlacePositionOffsetX.y) / 2, 0, (PlacePositionOffsetY.x + PlacePositionOffsetY.y));
        }
        public Vector3 AvgPlaceDefendPosition
        {
            get => new Vector3((PlacePositionOffsetX.x + PlacePositionOffsetX.y) / 2, 0, -(PlacePositionOffsetY.x + PlacePositionOffsetY.y));
        }


        #region Initilize

        public void Initilize(List<PlayerWrapper> players, Deck deck, DiscardPile discardPile, ICardComparator comparator)
        {
            this.players = players;
            this.discardPile = discardPile;
            this.comparator = comparator;
            this.deck = deck;

            CreatePoints();
        }

        private void CreatePoints()
        {
            currantPairs = new List<CardPair>();
            pairPoints = new Dictionary<Vector2Int, PairPoint>();
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    GameObject point = new GameObject($"{x}:{y}");
                    point.transform.position = new Vector3(x * 18 - 18, 0.1f, y * 18 - 18);
                    point.transform.parent = pointsPlace;

                    Vector2Int key = new Vector2Int(x, y);
                    pairPoints.Add(key, new PairPoint(key, point.transform.position, point.transform));
                }

            }
        }

        #endregion

        #region CardPlacement
        public void Drop(IDragable card, DropCardData data)
        {
            card.Accept(this, data);
        }
        public void Visit(Card card, object data = null)
        {
            try
            {
                DropCardData info = data as DropCardData;
                TryPlaceCard(card);
            }
            catch { }
        }

        public void Drag(IDragable card, MoveInfo info)
        {
            PairPoint min = GetNearPoint(info.position);

            Vector3 position = Vector3.Lerp(info.position, min.Position, 0.75f);
            position.y = min.position.y + 0.5f;

            Quaternion rotation = Quaternion.identity;

            if (min.Pair != null && !min.Pair.Done)
            {
                rotation = Quaternion.Euler(0, AvgPlaceDefendRotation, 0);
                position += AvgPlaceDefendPosition;
            }
            else
            {
                rotation = Quaternion.Euler(0, AvgPlaceAttackRotation, 0);
                position += AvgPlaceAttackPosition;
            }

            card.Body.position = Vector3.Lerp(card.Body.position, position, 0.3f);
            card.Body.rotation = Quaternion.Lerp(card.Body.rotation, rotation, 0.3f);
        }

        private PairPoint GetNearPoint(Vector3 position)
        {
            PairPoint min = null;
            float lenght = int.MaxValue;

            foreach (PairPoint point in pairPoints.Values)
            {
                if ((position - point.position).magnitude < lenght)
                {
                    min = point;
                    lenght = (position - point.position).magnitude;
                }
            }

            return min;
        }

        private void TryPlaceCard(Card card)
        {
            if(card.Owner == Attacker)
            {
                PairPoint point = GetNearPoint(card.Body.position);

                bool firstMove = currantPairs.Count == 0;
                bool canPut = currantPairs.Count(x => comparator.CanPut(card, x)) > 0;

                if(point.CanPutAttack && (firstMove || canPut))
                {
                    PlaceAttackCard(point, card);
                }
                else
                {
                    card.Owner.Hands.Drop(card, new DropCardData { Sender = DropCardData.SenderTypes.Self });
                }
            }
            else if(card.Owner == Defender)
            {
                PairPoint point = GetNearPoint(card.Body.position);

                if (point.CanPutDefend && point.Pair.TryBeat(card))
                {
                    PlaceDefendCard(point, card);
                }
                else
                {
                    card.Owner.Hands.Drop(card, new DropCardData { Sender = DropCardData.SenderTypes.Self });
                }
            }
            else
            {
                Debug.LogError("Card have no owner!");
            }
        }

        private void PlaceAttackCard(PairPoint point, Card card)
        {
            CardPair pair = new CardPair(card, comparator);
            point.CreateNewPair(pair);
            currantPairs.Add(pair);

            Vector3 offset = new Vector3(-Random.Range(PlacePositionOffsetX.x, PlacePositionOffsetX.y), 0f, Random.Range(PlacePositionOffsetY.x, PlacePositionOffsetY.y));
            Vector3 position = point.Position + offset;
            Quaternion rotation = Quaternion.Euler(0, Random.Range(PlaceAttackRotationOffset.x, PlaceAttackRotationOffset.y), 0);

            card.MoveTo(position, rotation, 0.25f, ICardAnimation.Order.Override);
        }
        private void PlaceDefendCard(PairPoint point, Card card)
        {
            Vector3 offset = new Vector3(Random.Range(PlacePositionOffsetX.x, PlacePositionOffsetX.y), 0f, -Random.Range(PlacePositionOffsetY.x, PlacePositionOffsetY.y));
            Vector3 position = point.Position + offset;
            Quaternion rotation = Quaternion.Euler(0, Random.Range(PlaceDefendRotationOffset.x, PlaceDefendRotationOffset.y), 0);

            card.MoveTo(position, rotation, 0.25f, ICardAnimation.Order.Override);
        }
        #endregion

        #region GameStates

        public void StartGame()
        {
            deck.DealtCards();
        }
        public void NextMove()
        {

        }
        public void EndMove()
        {
            deck.DealtCards();
            CurrantMove++;
        }

        #endregion
    }
}
