using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

namespace Lobby
{
    public class PlayerLobbyItem : MonoBehaviour, IPunObservable
    {
        [Header("Components")]
        [SerializeField] private TextMeshProUGUI playerName;
        [SerializeField] private Toggle toggle;
        [SerializeField] PhotonView photonView;

        private bool isReady = false;

        public bool IsReady
        {
            get => isReady;
            set
            {
                isReady = value;
                toggle.isOn = isReady;
            }
        }

        public void Initilize(string name)
        {
            playerName.text = name;

            photonView.RPC(nameof(SetParent), RpcTarget.AllBuffered, transform.parent.name);
            photonView.RPC(nameof(SetPlayerName), RpcTarget.AllBuffered, name);
        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(IsReady);
            }
            else
            {
                IsReady = (bool)stream.ReceiveNext();
            }
        }

        [PunRPC]
        public void SetParent(string gameObjectName)
        {
            GameObject obj = GameObject.Find(gameObjectName);
            if(obj)
            {
                transform.SetParent(obj.transform, false);
            }
        }
        [PunRPC]
        public void SetPlayerName(string name)
        {
            playerName.text = name;
        }
    }
}
