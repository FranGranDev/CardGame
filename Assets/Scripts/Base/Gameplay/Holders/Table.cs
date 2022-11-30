using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Cards
{
    public class Table : MonoBehaviour, ICardHolder, ICardVisitor
    {
        [Header("States")]
        [SerializeField] private States state;
        [SerializeField] private int currantMove = 0;
        [SerializeField] private PlayerWrapper attacker;
        [SerializeField] private PlayerWrapper defender;
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


        private Dictionary<Vector2Int, PairPoint> pairPoints;
        private List<CardPair> currantPairs;
        private ICardComparator comparator;


        public States State
        {
            get => state;
            set
            {
                state = value;
            }
        }
        public int CardsCount => 0;
        public int CurrantMove
        {
            get => currantMove;
            set
            {
                currantMove = value;

                attacker = Attacker;
                defender = Defender;
            }
        }
        public PlayerWrapper GetPlayer(Card card) 
        {
            return card.Owner;
        } //fix
        public PlayerWrapper Attacker
        {
            get => attacker;
            private set => attacker = value;
        }
        public PlayerWrapper Defender
        {
            get => defender;
            private set => defender = value;
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


        public event System.Action<int, PlayerWrapper, PlayerWrapper> OnNextMove;
        public event System.Action<States> OnEndMove;
        public event System.Action<PairPoint, Card> OnCardPlaced;


        public event System.Action<MatchData> OnGameEnded;
        public event System.Action<MatchData> OnGameEndedLocal;


        #region Initilize

        public void Initilize(List<PlayerWrapper> players, Deck deck, DiscardPile discardPile, ICardComparator comparator)
        {
            this.players = players;
            this.discardPile = discardPile;
            this.comparator = comparator;
            this.deck = deck;

            foreach(PlayerWrapper player in players)
            {
                player.OnAction += OnPlayerAction;

                player.PlayerState = PlayerWrapper.PlayerStates.Idle;
                player.MoveState = PlayerWrapper.MoveStates.Idle;
            }

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
                    point.transform.position = new Vector3(x * 16 - 16, 0.1f, y * 18 - 18);
                    point.transform.parent = pointsPlace;

                    Vector2Int key = new Vector2Int(x, y);
                    pairPoints.Add(key, new PairPoint(key, point.transform.position, point.transform));
                }

            }
        }

        #endregion

        #region CardActions
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
            position.y += 0.5f;

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


        public void PlaceCard(Vector2Int pointKey, CardInfo info, PlayerWrapper player)//Accpet
        {
            Card card = player.Hands.Cards.First(x => x.Info.Equals(info));
            pointKey = new Vector2Int(2, 2) - pointKey;
            PairPoint point = pairPoints[pointKey];

            card.OnDropped?.Invoke(card);
            PlaceCard(card, point);

            UpdatePlayersStates();
        }
        public void PlaceCard(Card card, PairPoint point)//Accept
        {
            if (card.Owner == Attacker)
            {
                bool firstMove = currantPairs.Count == 0;
                bool canPut = currantPairs.Count(x => comparator.CanPut(card, x)) > 0;

                if (point.CanPutAttack && (firstMove || canPut))
                {
                    PlaceAttackCard(point, card, ICardAnimation.Types.OtherPlace, 0.75f);
                }
            }
            else if (card.Owner == Defender)
            {
                if (point.CanPutDefend && point.Pair.CanBeat(card))
                {
                    PlaceDefendCard(point, card, ICardAnimation.Types.OtherPlace, 0.75f);
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

            UpdatePlayersStates();
        }
    


        private void TryPlaceCard(Card card)//Send
        {
            if(card.Owner == Attacker)
            {
                PairPoint point = GetNearPoint(card.Body.position);

                bool firstMove = currantPairs.Count == 0;
                bool canPut = currantPairs.Count(x => comparator.CanPut(card, x)) > 0;

                if(point.CanPutAttack && (firstMove || canPut))
                {
                    OnCardPlaced?.Invoke(point, card);
                    Attacker.MoveState = PlayerWrapper.MoveStates.Playing;
                    Defender.MoveState = PlayerWrapper.MoveStates.Playing;
                    PlaceAttackCard(point, card, ICardAnimation.Types.MoveTo, 0.25f);
                }
                else
                {
                    card.Owner.Hands.Drop(card, new DropCardData { Sender = DropCardData.SenderTypes.Self });
                }
            }
            else if(card.Owner == Defender)
            {
                PairPoint point = GetNearPoint(card.Body.position);

                if (point.CanPutDefend && point.Pair.CanBeat(card))
                {
                    OnCardPlaced?.Invoke(point, card);
                    Attacker.MoveState = PlayerWrapper.MoveStates.Playing;
                    Defender.MoveState = PlayerWrapper.MoveStates.Playing;
                    PlaceDefendCard(point, card, ICardAnimation.Types.MoveTo, 0.25f);
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


            UpdatePlayersStates();
        }

        private void PlaceAttackCard(PairPoint point, Card card, ICardAnimation.Types animation, float time)
        {
            CardPair pair = new CardPair(card, comparator);
            point.CreateNewPair(pair);
            currantPairs.Add(pair);

            Vector3 offset = new Vector3(-Random.Range(PlacePositionOffsetX.x, PlacePositionOffsetX.y), 0f, Random.Range(PlacePositionOffsetY.x, PlacePositionOffsetY.y));
            Vector3 position = point.Position + offset;
            Debug.Log(point.Position);
            Quaternion rotation = Quaternion.Euler(0, Random.Range(PlaceAttackRotationOffset.x, PlaceAttackRotationOffset.y), 0);

            card.DoMove(animation, position, rotation, time, ICardAnimation.Order.Override);
        }
        private void PlaceDefendCard(PairPoint point, Card card, ICardAnimation.Types animation, float time)
        {
            point.Pair.Beat(card);
            
            Vector3 offset = new Vector3(Random.Range(PlacePositionOffsetX.x, PlacePositionOffsetX.y), 0.15f, -Random.Range(PlacePositionOffsetY.x, PlacePositionOffsetY.y));
            Vector3 position = point.Position + offset;
            Debug.Log(point.Position);
            Quaternion rotation = Quaternion.Euler(0, Random.Range(PlaceDefendRotationOffset.x, PlaceDefendRotationOffset.y), 0);

            card.DoMove(animation, position, rotation, time, ICardAnimation.Order.Override);
        }


        private void SendAllCard(PlayerWrapper player)
        {
            foreach (CardPair pair in currantPairs)
            {
                if (pair.Attacker)
                {
                    player.Hands.Drop(pair.Attacker, new DropCardData { Sender = DropCardData.SenderTypes.Table });
                }
                if (pair.Defender)
                {
                    player.Hands.Drop(pair.Defender, new DropCardData { Sender = DropCardData.SenderTypes.Table });
                }
            }

            ClearPairs();
        }
        private void SendAllCard(DiscardPile discardPile)
        {
            discardPile.Drop(currantPairs);

            ClearPairs();
        }

        private void ClearPairs()
        {
            currantPairs.Clear();

            foreach(PairPoint pair in pairPoints.Values)
            {
                pair.Clear();
            }
        }

        #endregion

        #region GameStates

        public enum States { Idle, Game, Defended, NotDefended }
        

        public void StartMove() //Send
        {
            if(Random.Range(0, 2) == 0)
            {
                Attacker = players[0];
                Defender = players[1];
            }
            else
            {
                Attacker = players[1];
                Defender = players[0];
            }

            State = States.Game;

            Attacker.PlayerState = PlayerWrapper.PlayerStates.Attacker;
            Defender.PlayerState = PlayerWrapper.PlayerStates.Defender;

            deck.DealtCards();
            UpdatePlayersStates();

            OnNextMove?.Invoke(CurrantMove, Attacker, Defender);
        }
        public void NextMove() //Send
        {
            if (State == States.Idle)
            {
                CurrantMove++;
                State = States.Game;


                Attacker.PlayerState = PlayerWrapper.PlayerStates.Attacker;
                Defender.PlayerState = PlayerWrapper.PlayerStates.Defender;

                deck.DealtCards();
                UpdatePlayersStates();

                OnNextMove?.Invoke(CurrantMove, Attacker, Defender);
            }
        }
        public void NextMove(int move, PlayerWrapper.Data attacker, PlayerWrapper.Data defender) //Accept
        {
            Debug.Log("Next Move");

            CurrantMove = move;
            State = States.Game;

            Debug.Log(attacker.id + "  " + defender.id);

            Attacker = players.First(x => x.Id == attacker.id);
            Defender = players.First(x => x.Id == defender.id);

            Attacker.PlayerState = attacker.playerState;
            Attacker.MoveState = attacker.moveState;

            Defender.PlayerState = defender.playerState;
            Defender.MoveState = defender.moveState;

            UpdatePlayersStates();
        }

        public void EndMove(States endState)
        {
            if (State == States.Game)
            {
                State = States.Idle;
                StartCoroutine(EndMoveCour(endState));
            }
        } //Send
        private IEnumerator EndMoveCour(States endState)
        {
            Debug.Log("Next Move: " + CurrantMove);
            switch (endState)
            {
                case States.Defended:                    
                    SendAllCard(discardPile);
                    SwapPlayers();
                    break;
                case States.NotDefended:
                    SendAllCard(Defender);
                    break;
            }


            Attacker.PlayerState = PlayerWrapper.PlayerStates.Idle;
            Attacker.MoveState = PlayerWrapper.MoveStates.Idle;

            Defender.PlayerState = PlayerWrapper.PlayerStates.Idle;
            Defender.MoveState = PlayerWrapper.MoveStates.Idle;

            yield return new WaitForSeconds(1f);

            NextMove();

            yield break;
        }
        public void EndMove(States endState, int move)
        {
            Debug.Log("End Move");

            State = States.Idle;
            switch (endState)
            {
                case States.Defended:
                    SendAllCard(discardPile);
                    break;
                case States.NotDefended:
                    SendAllCard(Defender);
                    break;
            }

            Attacker.PlayerState = PlayerWrapper.PlayerStates.Idle;
            Attacker.MoveState = PlayerWrapper.MoveStates.Idle;

            Defender.PlayerState = PlayerWrapper.PlayerStates.Idle;
            Defender.MoveState = PlayerWrapper.MoveStates.Idle;
        } //Accept


        
        private void UpdatePlayersStates() //Local only
        {
            if(pairPoints.Values.Count(x => x.Pair != null) == 0)
            {
                Attacker.MoveState = PlayerWrapper.MoveStates.FirstMove;
                Defender.MoveState = PlayerWrapper.MoveStates.FirstMove;
                return;
            }
            if (pairPoints.Values.Count(x => x.CanPutDefend) > 0)
            {
                Defender.MoveState = PlayerWrapper.MoveStates.Playing;
            }
            else
            {
                Defender.MoveState = PlayerWrapper.MoveStates.EnemyMove;
            }
        }
        public void UpdatePlayersStates(PlayerWrapper.Data attacker, PlayerWrapper.Data defender) //Accept
        {
            Attacker.MoveState = attacker.moveState;
            Attacker.PlayerState = attacker.playerState;

            Defender.MoveState = defender.moveState;
            Defender.PlayerState = defender.playerState;
        }
        private void SwapPlayers()
        {
            PlayerWrapper temp = Attacker;
            Attacker = Defender;
            Defender = temp;
        }

        #endregion

        #region PlayerActions
        
        private void OnPlayerAction(PlayerWrapper player)
        {
            if (State != States.Game)
                return;
            PlayerWrapper other;
            switch (player.PlayerState)
            {
                case PlayerWrapper.PlayerStates.Attacker:
                    other = Defender;
                    switch(player.MoveState)
                    {
                        case PlayerWrapper.MoveStates.Playing: //Done
                            if (currantPairs.Count(x => !x.Done) == 0)
                            {
                                OnEndMove?.Invoke(States.Defended);
                            }
                            break;
                    }

                    break;

                case PlayerWrapper.PlayerStates.Defender:
                    other = Attacker;
                    switch (player.MoveState)
                    {
                        case PlayerWrapper.MoveStates.Playing: //Take Cards
                            if(currantPairs.Count(x => !x.Done) > 0)
                            {
                                OnEndMove?.Invoke(States.NotDefended);
                            }
                            break;
                    }

                    break;
            }
            
        }

        public void SelfLeave(PlayerWrapper self) //Send
        {
            PlayerWrapper winner = players.First(x => x.Id != self.Id);

            MatchData data = new MatchData(winner, self, currantMove, MatchData.EndTypes.Exit);

            GameEndedLocal(data);
        }
        public void OtherLeave(PlayerWrapper self) //Send
        {
            PlayerWrapper looser = players.First(x => x.Id != self.Id);

            MatchData data = new MatchData(self, self, currantMove, MatchData.EndTypes.Exit);

            GameEndedLocal(data);
        }
        public void Surrender(PlayerWrapper self) //Send
        {
            PlayerWrapper winner = players.First(x => x.Id != self.Id);

            MatchData data = new MatchData(winner, self, currantMove, MatchData.EndTypes.Surrender);

            OnGameEnded?.Invoke(data);
        }

        public void GameEndedLocal(MatchData data) //Accept
        {
            OnGameEndedLocal?.Invoke(data);
        }

        #endregion

        public class MatchData
        {
            public MatchData(PlayerWrapper winner, PlayerWrapper looser, int moveCount, EndTypes endType)
            {
                Winner = winner;
                Looser = looser;
                MoveCount = moveCount;
                EndType = endType;
            }

            public enum EndTypes { Game, Surrender, Exit}

            public PlayerWrapper Winner { get; }
            public PlayerWrapper Looser { get; }
            public int MoveCount { get; }
            public EndTypes EndType { get; }
        }
    }
}
