using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviourPunCallbacks
{
    [Header("UI Links")]
    [SerializeField] private UIPanel createRoomPanel;
    [SerializeField] private UIPanel joinRoomPanel;
    [SerializeField] private UIPanel noInternetPanel;

    private void Awake()
    {
        InitilizeUI();
    }
    private void Start()
    {
        SetupSettings();

        noInternetPanel.IsShown = true;
    }

    #region Initilize

    private void InitilizeUI()
    {
        noInternetPanel.Init();
        createRoomPanel.Init();
        joinRoomPanel.Init();
    }
    private void SetupSettings()
    {
        PhotonNetwork.NickName = "Player " + Random.Range(0, 256);

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = "1";

        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        noInternetPanel.IsShown = false;
    }

    #endregion

    #region Start Menu

    public void GoCreateRoom()
    {
        //PhotonNetwork.CreateRoom("Test Game", new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
    }
    public void GoJoinRoom()
    {
        //PhotonNetwork.JoinRandomRoom();
    }

    #endregion

    #region RoomsList



    #endregion

    #region Create Room

    public void OnChangeRoomName()
    {

    }
    public void CreateRoom()
    {

    }

    #endregion

    #region WaitingPlayers

    public void MakeReady()
    {

    }
    public override void OnJoinedRoom()
    {
        
    }
    public override void OnLeftRoom()
    {

    }

    #endregion
}
