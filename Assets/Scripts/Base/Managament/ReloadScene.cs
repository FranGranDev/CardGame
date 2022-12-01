using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ReloadScene : MonoBehaviourPunCallbacks
{
    private void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }
}
