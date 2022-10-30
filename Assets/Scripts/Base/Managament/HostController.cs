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

        public List<PlayerWrapper> Initilize(Table table, Deck deck, Hand player, Hand enemy)
        {
            RegisterCustomTypes();

            this.table = table;
            this.deck = deck;

            players = new List<PlayerWrapper>()
            {
                new PlayerWrapper(PhotonNetwork.LocalPlayer.ActorNumber, player),
            };
            if(PhotonNetwork.PlayerListOthers.Length > 0)
            {
                players.Add(new PlayerWrapper(PhotonNetwork.PlayerListOthers[0].ActorNumber, enemy));
            }
            else
            {
                PhotonNetwork.OfflineMode = true;
            }
            
            deck.OnSendCard += OnDeckSendCard;


            return players;
        }
        private void RegisterCustomTypes()
        {
            PhotonPeer.RegisterType(typeof(Vector2Int), 10, Serialization.SerializeVector2Int, Serialization.DeserializeVector2Int);
            PhotonPeer.RegisterType(typeof(CardInfo), 11, Serialization.SerializeCardInfo, Serialization.DeserializeCardInfo);
        }

        public void StartGame(bool offline = false)
        {
            if (PhotonNetwork.IsMasterClient || offline)
            {
                List<CardInfo> cards = deck.GenerateDeckData();

                deck.BuildDeck();

                object[] data = new object[cards.Count];
                for(int i = 0; i < cards.Count; i++)
                {
                    data[i] = cards[i];
                }

                photonView.RPC(nameof(DoGenerateDeck), RpcTarget.Others, data);


                deck.DealtCards();
            }
        }

        #region Deck
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
    }
}