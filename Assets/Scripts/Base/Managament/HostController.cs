using Cards;
using UI;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine;


namespace Managament
{
    public class HostController : MonoBehaviourPunCallbacks
    {
        public bool isServer => PhotonNetwork.IsMasterClient;

        private Table table;
        private Deck deck;
        private List<PlayerWrapper> players;
        private System.Action<Data> onClientGetServerData;

        private PlayerWrapper self
        {
            get => players.First(x => x.Local);
        }

        public event System.Action OnRestartGame;
        public event System.Action<PlayerWrapper> OnOtherReadyChanged;
        public event System.Action<PlayerWrapper> OnOtherExit;


        private void Awake()
        {
            RegisterCustomTypes();
        }

        public void ServerInitilize(System.Action<Data> onClientGetServerData)
        {
            this.onClientGetServerData = onClientGetServerData; 


            if(!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
            {
                onClientGetServerData?.Invoke(new Data(Random.Range(0, 4)));
                return;
            }
            if (PhotonNetwork.IsMasterClient)
            {
                object data = new object[1]
                {
                    Random.Range(0, 4),
                };
                photonView.RPC(nameof(DoClientInitilize), RpcTarget.AllBuffered, data);
            }
        }
        public void StartGame(bool offline = false)
        {
            if (PhotonNetwork.IsMasterClient || offline)
            {
                deck.GenerateDeckData();
                deck.BuildDeck();

                table.StartMove();
            }
        }


        private void UnsubscribeForGameActions()
        {
            deck.OnDeckGenerated -= OnDeckGenerated;
            deck.OnSendCard -= OnDeckSendCard;

            table.OnNextMove -= OnTableNextMove;
            table.OnEndMove -= OnTableEndMove;
            table.OnCardPlaced -= OnCardPlaced;
            table.OnGameEnded -= OnGameEnded;
        }
        private void SubscribeForGameActions()
        {
            self.OnAction += OnPlayersAction;

            deck.OnDeckGenerated += OnDeckGenerated;
            deck.OnSendCard += OnDeckSendCard;

            table.OnNextMove += OnTableNextMove;
            table.OnEndMove += OnTableEndMove;
            table.OnCardPlaced += OnCardPlaced;
            table.OnGameEnded += OnGameEnded;
        }
        private void RegisterCustomTypes()
        {
            PhotonPeer.RegisterType(typeof(Vector2Int), 10, Serialization.SerializeVector2Int, Serialization.DeserializeVector2Int);
            PhotonPeer.RegisterType(typeof(CardInfo), 11, Serialization.SerializeCardInfo, Serialization.DeserializeCardInfo);
            PhotonPeer.RegisterType(typeof(PlayerWrapper.Data), 12, Serialization.SerializePlayerState, Serialization.DeserializePlayerState);
        }


        #region Lobby-Networking

        public void OnPlayerReadyChanged(PlayerWrapper player)
        {
            photonView.RPC(nameof(DoPlayerReadyChanged), RpcTarget.MasterClient, new object[2] { player.Id, player.Ready });
        }
        [PunRPC]
        public void DoPlayerReadyChanged(object[] data)
        {
            int id = (int)data[0];

            PlayerWrapper player = players.First(x => x.Id == id);

            player.Ready = (bool)data[1];

            OnOtherReadyChanged?.Invoke(player);
        }




        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PlayerWrapper player = players.First(x => x.Id == otherPlayer.ActorNumber);

            player.Online = false;

            OnOtherExit?.Invoke(player);
        }


        public void RestartGame()
        {
            photonView.RPC(nameof(DoRestartGame), RpcTarget.All);
        }
        [PunRPC]
        public void DoRestartGame()
        {
            UnsubscribeForGameActions();

            OnRestartGame?.Invoke();
        }
        #endregion

        #region Game-Networking

        public List<PlayerWrapper> LocalInitilize(Table table, Deck deck, Hand player, Hand enemy)
        {
            this.table = table;
            this.deck = deck;

            players = new List<PlayerWrapper>()
            {
                new PlayerWrapper(PhotonNetwork.LocalPlayer.ActorNumber, player, PhotonNetwork.LocalPlayer.NickName, true),
            };
            if (PhotonNetwork.PlayerListOthers.Length > 0)
            {
                Player other = PhotonNetwork.PlayerListOthers[0];
                players.Add(new PlayerWrapper(other.ActorNumber, enemy, other.NickName));
            }
            else
            {
                PhotonNetwork.OfflineMode = true;
            }

            players = players.OrderBy(x => x.Id).ToList();

            SubscribeForGameActions();

            return players;
        }
        [PunRPC]
        public void DoClientInitilize(object[] info)
        {
            Data data = new Data((int)info[0]);

            onClientGetServerData?.Invoke(data);
        }        

        private void OnGameEnded(Table.MatchData data)
        {
            object[] info = new object[4]
            {
                data.Winner.Id,
                data.Looser.Id,
                data.MoveCount,
                data.EndType,
            };

            photonView.RPC(nameof(DoGameEnded), RpcTarget.All, info);
        }
        [PunRPC]
        public void DoGameEnded(object[] data)
        {
            PlayerWrapper winner = players.First(x => x.Id == (int)data[0]);
            PlayerWrapper looser = players.First(x => x.Id == (int)data[1]);

            Table.MatchData info = new Table.MatchData(winner, looser, (int)data[2], (Table.MatchData.EndTypes)data[3]);

            table.GameEndedLocal(info);
        }

        #endregion

        #region Table-Networking

        private void OnCardPlaced(PairPoint point, Card card)
        {
            object[] data = new object[3]
            {
                point.key,
                card.Info,
                card.Owner.Id,
            };

            photonView.RPC(nameof(DoPlaceCard), RpcTarget.Others, data);
        }
        [PunRPC]
        public void DoPlaceCard(object[] data)
        {
            Vector2Int key = (Vector2Int)data[0];
            CardInfo info = (CardInfo)data[1];
            PlayerWrapper player = players.First(x => x.Id == (int)data[2]);

            table.PlaceCard(key, info, player);
        }

        private void OnPlayersAction(PlayerWrapper player)
        {
            photonView.RPC(nameof(DoPlayersAction), RpcTarget.Others, player.Id);
        }
        [PunRPC]
        public void DoPlayersAction(object data)
        {
            int id = (int)data;
            PlayerWrapper player = players.First(x => x.Id == id);

            player.DoAction();
        }

        private void OnTableNextMove(int move, PlayerWrapper attacker, PlayerWrapper defender)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                object[] info = new object[3]
                {
                    move,
                    new PlayerWrapper.Data(attacker),
                    new PlayerWrapper.Data(defender),
                };
                photonView.RPC(nameof(DoTableNextMove), RpcTarget.Others, info);
            }
        }
        [PunRPC]
        public void DoTableNextMove(object[] info)
        {
            table.NextMove((int)info[0], (PlayerWrapper.Data)(info[1]), (PlayerWrapper.Data)(info[2]));
        }

        private void OnTableEndMove(Table.States endState)
        {
            if(PhotonNetwork.OfflineMode)
            {
                table.EndMove(endState);
                return;
            }

            object[] data = new object[2]
                {
                    0,
                    (int)endState,
                };
            photonView.RPC(nameof(DoTableEndMove), RpcTarget.All, data);
        }
        [PunRPC]
        private void DoTableEndMove(object[] info)
        {
            int move = (int)info[0];
            Table.States endState = (Table.States)info[1];

            if (PhotonNetwork.IsMasterClient)
            {
                table.EndMove(endState);
            }
            else
            {
                table.EndMove(endState, move);
            }
        }
        #endregion

        #region Deck-Networking
        private void OnDeckSendCard(CardInfo cardInfo, PlayerWrapper player)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                object[] info = new object[2]
                {
                    cardInfo,
                    player.Id
                };
                photonView.RPC(nameof(DoDeckSendCard), RpcTarget.Others, info);
            }
        }
        [PunRPC]
        public void DoDeckSendCard(object[] info)
        {
            PlayerWrapper player = players.Where(x => x.Id == (int)info[1]).First();

            deck.SendCard((CardInfo)info[0], player);
        }

        private void OnDeckGenerated(List<CardInfo> cards)
        {
            object[] data = new object[cards.Count];
            for (int i = 0; i < cards.Count; i++)
            {
                data[i] = cards[i];
            }

            photonView.RPC(nameof(DoGenerateDeck), RpcTarget.Others, data);

        }
        [PunRPC]
        public void DoGenerateDeck(object[] info)
        {
            List<CardInfo> cards = new List<CardInfo>();
            for (int i = 0; i < info.Length; i++)
            {
                cards.Add((CardInfo)info[i]);
            }

            deck.SetDeckData(cards);
        }

        #endregion

        public class Data
        {
            public Data(int trump)
            {
                Trump = (DurakCard.SuitTypes)trump;
            }

            public readonly DurakCard.SuitTypes Trump;
        }
    }
}