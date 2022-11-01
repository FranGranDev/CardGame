using Cards;
using UI;
using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Managament
{
    public class HostController : MonoBehaviourPunCallbacks
    {
        private Table table;
        private Deck deck;

        private List<PlayerWrapper> players;

        private System.Action<Data> onClientGetServerData;

        public void ServerInitilize(System.Action<Data> onClientGetServerData)
        {
            this.onClientGetServerData = onClientGetServerData; 

            if(!PhotonNetwork.IsConnected)
            {
                //Not connected - test mode
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
        public List<PlayerWrapper> LocalInitilize(Table table, Deck deck, Hand player, Hand enemy)
        {
            RegisterCustomTypes();

            this.table = table;
            this.deck = deck;

            players = new List<PlayerWrapper>()
            {
                new PlayerWrapper(PhotonNetwork.LocalPlayer.ActorNumber, player, true),
            };
            if(PhotonNetwork.PlayerListOthers.Length > 0)
            {
                players.Add(new PlayerWrapper(PhotonNetwork.PlayerListOthers[0].ActorNumber, enemy));
            }
            else
            {
                PhotonNetwork.OfflineMode = true;
            }

            players = players.OrderBy(x => x.Id).ToList();

            SubscribeForGameActions();

            return players;
        }
        private void SubscribeForGameActions()
        {
            deck.OnDeckGenerated += OnDeckGenerated;
            deck.OnSendCard += OnDeckSendCard;

            table.OnNextMove += OnTableNextMove;
            table.OnEndMove += OnTableEndMove;
            table.OnCardPlaced += OnCardPlaced;
        }
        private void RegisterCustomTypes()
        {
            PhotonPeer.RegisterType(typeof(Vector2Int), 10, Serialization.SerializeVector2Int, Serialization.DeserializeVector2Int);
            PhotonPeer.RegisterType(typeof(CardInfo), 11, Serialization.SerializeCardInfo, Serialization.DeserializeCardInfo);
            PhotonPeer.RegisterType(typeof(PlayerWrapper.Data), 12, Serialization.SerializePlayerState, Serialization.DeserializePlayerState);
        }

        public void StartGame(bool offline = false)
        {
            if (PhotonNetwork.IsMasterClient || offline)
            {
                deck.GenerateDeckData();
                deck.BuildDeck();

                table.NextMove();
            }
        }


        #region Game-Networking

        [PunRPC]
        public void DoClientInitilize(object[] info)
        {
            Data data = new Data((int)info[0]);

            onClientGetServerData?.Invoke(data);
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

        private void OnTableNextMove(int move, PlayerWrapper attacker, PlayerWrapper defender)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(DoTableNextMove), RpcTarget.Others, move);
            }
        }
        [PunRPC]
        public void DoTableNextMove(object info)
        {
            int move = (int)info;

            table.NextMove(move);
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