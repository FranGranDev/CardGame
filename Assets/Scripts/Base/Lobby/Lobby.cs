using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UI;
using DG.Tweening;
using UnityEngine.SceneManagement;


namespace Lobby
{
    public class Lobby : MonoBehaviourPunCallbacks
    {
        [Header("UI Static")]
        [SerializeField] private UIPanel noInternetPanel;
        [Header("UI Loading")]
        [SerializeField] private GameObject loadingPanel;
        [Header("UI Start Menu")]
        [SerializeField] private GameObject startPanel;
        [SerializeField] private TMP_InputField startInputName;
        [Header("UI Room List")]
        [SerializeField] private GameObject roomListPanel;
        [Header("UI Create Room")]
        [SerializeField] private GameObject createRoomPanel;
        [SerializeField] private TMP_InputField roomInputName;
        [Header("UI Waiting Players")]
        [SerializeField] private GameObject waitingRoomPanel;
        [SerializeField] private TextMeshProUGUI roomName;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Transform playerItemsContent;
        [SerializeField] private PlayerLobbyItem playerStatePrefab;
        public enum ButtonStateTypes
        {
            MakeReady,
            MakeNotReady,
            StartGame
        }


        [Header("States")]
        [SerializeField] private PanelStateTypes panelState;
        [SerializeField] private ButtonStateTypes buttonState;
        public enum PanelStateTypes
        {
            Loading,
            Start,
            RoomList,
            CreateRoom,
            WaitPlayers,
        }
        private Dictionary<PanelStateTypes, List<IUiBehavior>> statePanels;
        private Dictionary<PanelStateTypes, GameObject> stateObjects;
        private Dictionary<ButtonStateTypes, string> buttonStateNames = new Dictionary<ButtonStateTypes, string>()
        {
            {ButtonStateTypes.MakeNotReady, "Not Ready" },
            {ButtonStateTypes.MakeReady, "Make Ready!" },
            {ButtonStateTypes.StartGame, "Start Game!" },
        };
        private Dictionary<int, bool> playersReady = new Dictionary<int, bool>();
        private PlayerLobbyItem selfItem;


        public PanelStateTypes PanelState
        {
            get => panelState;
            set
            {
                OnStateEnd(panelState);
                panelState = value;
                OnStateStart(panelState);
            }
        }
        public ButtonStateTypes ReadyButtonState
        {
            get => buttonState;
            set
            {
                buttonState = value;
                buttonText.text = buttonStateNames[value];
            }
        }

        private void Awake()
        {
            InitilizeUI();
        }
        private void Start()
        {
            SetupSettings();

            noInternetPanel.IsShown = true;

            PanelState = PanelStateTypes.Loading;
        }

        #region Initilize

        private void InitilizeUI()
        {
            statePanels = new Dictionary<PanelStateTypes, List<IUiBehavior>>()
        {
            {PanelStateTypes.Loading, loadingPanel.GetComponentsInChildren<IUiBehavior>(true).ToList() },
            {PanelStateTypes.Start, startPanel.GetComponentsInChildren<IUiBehavior>(true).ToList() },
            {PanelStateTypes.CreateRoom, createRoomPanel.GetComponentsInChildren<IUiBehavior>(true).ToList() },
            {PanelStateTypes.RoomList, roomListPanel.GetComponentsInChildren<IUiBehavior>(true).ToList() },
            {PanelStateTypes.WaitPlayers, waitingRoomPanel.GetComponentsInChildren<IUiBehavior>(true).ToList() },
        };
            stateObjects = new Dictionary<PanelStateTypes, GameObject>()
            {
            {PanelStateTypes.Start, startPanel.gameObject},
            {PanelStateTypes.Loading, loadingPanel.gameObject},
            {PanelStateTypes.CreateRoom, createRoomPanel.gameObject },
            {PanelStateTypes.RoomList, roomListPanel.gameObject },
            {PanelStateTypes.WaitPlayers, waitingRoomPanel.gameObject },
            };

            foreach(List<IUiBehavior> panels in statePanels.Values)
            {
                foreach(IUiBehavior panel in panels)
                {
                    panel.Initilize();
                }
            }

            foreach(GameObject obj in stateObjects.Values)
            {
                obj.SetActive(false);
            }
        }
        private void SetupSettings()
        {
            PhotonNetwork.NickName = PlayerPrefs.GetString("name", $"Player {Random.Range(10, 99)}");

            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "1";

            PhotonNetwork.ConnectUsingSettings();
        }
        public override void OnConnectedToMaster()
        {
            noInternetPanel.IsShown = false;

            PanelState = PanelStateTypes.Start;
        }

        public void Exit()
        {
            Debug.Log("Exit main menu");
        }

        #endregion

        #region States

        private void OnStateStart(PanelStateTypes state)
        {            
            foreach (IUiBehavior panel in statePanels[state])
            {
                panel.IsShown = true;
            }
            stateObjects[state].SetActive(true);

            switch(state)
            {
                case PanelStateTypes.CreateRoom:
                    roomInputName.text = $"Room " + Random.Range(1000, 9999);
                    break;
                case PanelStateTypes.Start:
                    startInputName.text = PhotonNetwork.NickName;
                    break;
            }
        }
        private void OnStateEnd(PanelStateTypes state)
        {
            foreach (IUiBehavior panel in statePanels[state])
            {
                panel.IsShown = false;
            }
        }


        #endregion

        #region Start Menu

        public void OnChangeNickName()
        {
            PlayerPrefs.SetString("name", startInputName.text);
            PhotonNetwork.NickName = startInputName.text;
        }

        public void GoCreateRoom()
        {
            PanelState = PanelStateTypes.CreateRoom;
        }
        public void GoJoinRoom()
        {
            //PhotonNetwork.JoinRandomRoom();
        }

        public void GoMainMenu()
        {
            PanelState = PanelStateTypes.Start;
        }

        #endregion

        #region RoomsList

        //Coming soon

        public void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        #endregion

        #region Create Room

        public void OnChangeRoomName()
        {

        }
        public void CreateRoom()
        {
            PhotonNetwork.CreateRoom(roomInputName.text, new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
        }

        #endregion

        #region WaitingPlayers

        public void MakeReady()
        {
            if(selfItem)
            {
                selfItem.IsReady = !selfItem.IsReady;

                PhotonView.Get(this).RPC(nameof(UpdatePlayerState), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, selfItem.IsReady);

                UpdateButtonState();
            }
        }

        private void UpdateButtonState()
        {
            if(PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length > 1 && playersReady.Count(x => x.Value == false) == 0)
            {
                ReadyButtonState = ButtonStateTypes.StartGame;
            }
            else if(selfItem.IsReady)
            {
                ReadyButtonState = ButtonStateTypes.MakeNotReady;
            }
            else
            {
                ReadyButtonState = ButtonStateTypes.MakeReady;
            }
        }

        [PunRPC]
        public void UpdatePlayerState(int playerId, bool data)
        {
            if(playersReady.ContainsKey(playerId))
            {
                playersReady[playerId] = data;
            }
            else
            {
                playersReady.Add(playerId, data);
            }

            UpdateButtonState();
        }
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        public override void OnJoinedRoom()
        {
            roomName.text = PhotonNetwork.CurrentRoom.Name;

            PanelState = PanelStateTypes.WaitPlayers;

            GameObject obj = PhotonNetwork.Instantiate(playerStatePrefab.name, Vector3.zero, Quaternion.identity, 0);
            obj.transform.SetParent(playerItemsContent, false);

            selfItem = obj.GetComponent<PlayerLobbyItem>();
            selfItem.Initilize(PhotonNetwork.NickName);


            PhotonView.Get(this).RPC(nameof(UpdatePlayerState), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, selfItem.IsReady);
            UpdateButtonState();
        }
        public override void OnLeftRoom()
        {
            if (selfItem)
            {
                PhotonNetwork.Destroy(selfItem.gameObject);
            }

            GoMainMenu();
        }


        #endregion
    }
}