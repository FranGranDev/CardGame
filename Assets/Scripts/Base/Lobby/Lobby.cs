using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UI;
using Data;
using DG.Tweening;
using UnityEngine.SceneManagement;


namespace Lobby
{
    public class Lobby : MonoBehaviourPunCallbacks
    {
        [Header("Links")]
        [SerializeField] private LocalizationController localizationController;
        [Header("UI Static")]
        [SerializeField] private UIPanel noInternetPanel;
        [Header("UI Loading")]
        [SerializeField] private GameObject loadingPanel;
        [Header("UI Start Menu")]
        [SerializeField] private UILanguageChanger languageChanger;
        [SerializeField] private GameObject startPanel;
        [SerializeField] private TMP_InputField startInputName;
        [SerializeField] private UIToggle soundToggle;
        [Header("UI Join Room")]
        [SerializeField] private GameObject jointPanel;
        [SerializeField] private TMP_InputField joinRoomName;
        [SerializeField] private TextMeshProUGUI joinRoomError;
        [Header("UI Stats Menu")]
        [SerializeField] private GameObject statsPanel;
        [SerializeField] private TextMeshProUGUI statsMatchesCount;
        [SerializeField] private TextMeshProUGUI statsVictoryCount;
        [SerializeField] private TextMeshProUGUI statsDefeatCount;
        [SerializeField] private TextMeshProUGUI statsWinRate;
        [SerializeField] private Transform matchesInfoContent;
        [SerializeField] private UIMatchInfo matchInfoPrefab;
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
            JoinRoom,
            CreateRoom,
            WaitPlayers,
            GameStats,
        }
        private Dictionary<PanelStateTypes, List<IUiBehavior>> statePanels;
        private Dictionary<PanelStateTypes, GameObject> stateObjects;
        private Dictionary<DataBase.MatchData.Types, string> matchTypeNames = new Dictionary<DataBase.MatchData.Types, string>()
        {
            {DataBase.MatchData.Types.Game, "Full Game" },
            {DataBase.MatchData.Types.Surrender, "Surrender" },
            {DataBase.MatchData.Types.Leave, "Leave" },
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
                switch(value)
                {
                    case ButtonStateTypes.MakeNotReady:
                        buttonText.text = Localization.CurrantData.Lobby.NotReady;
                        break;
                    case ButtonStateTypes.MakeReady:
                        buttonText.text = Localization.CurrantData.Lobby.Ready;
                        break;
                    case ButtonStateTypes.StartGame:
                        buttonText.text = Localization.CurrantData.Lobby.StartGame;
                        break;
                }
            }
        }

        private void Awake()
        {
            SetupSettings();
            InitilizeUI();
        }
        private void Start()
        {
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
            {PanelStateTypes.JoinRoom, jointPanel.GetComponentsInChildren<IUiBehavior>(true).ToList() },
            {PanelStateTypes.WaitPlayers, waitingRoomPanel.GetComponentsInChildren<IUiBehavior>(true).ToList() },
            {PanelStateTypes.GameStats, statsPanel.GetComponentsInChildren<IUiBehavior>(true).ToList() },
        };
            stateObjects = new Dictionary<PanelStateTypes, GameObject>()
            {
            {PanelStateTypes.Start, startPanel.gameObject},
            {PanelStateTypes.Loading, loadingPanel.gameObject},
            {PanelStateTypes.CreateRoom, createRoomPanel.gameObject },
            {PanelStateTypes.JoinRoom, jointPanel.gameObject },
            {PanelStateTypes.WaitPlayers, waitingRoomPanel.gameObject },
            {PanelStateTypes.GameStats, statsPanel.gameObject },
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


            languageChanger.OnChanged += SetNextLanguage;
            soundToggle.Init(MuteSound, DataBase.SoundMuted);
            localizationController.SetLanguage(DataBase.Language, LocalizationController.Place.Lobby);

            AudioListener.pause = DataBase.SoundMuted;
        }

        private void SetupSettings()
        {
            PhotonNetwork.NickName = DataBase.Name;

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
                    roomInputName.text = DataBase.Room;
                    break;
                case PanelStateTypes.JoinRoom:
                    joinRoomName.text = DataBase.PrevRoom;
                    joinRoomError.gameObject.SetActive(false);
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
            DataBase.Name = startInputName.text;
            PhotonNetwork.NickName = DataBase.Name;
        }
        public void GoJoinRoom()
        {
            PanelState = PanelStateTypes.JoinRoom;
        }
        public void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }
        
        public void GoCreateRoom()
        {
            PanelState = PanelStateTypes.CreateRoom;
        }
        public void GoGameStats()
        {
            PanelState = PanelStateTypes.GameStats;
            UpdateGameStats();
        }
        public void GoMainMenu()
        {
            PanelState = PanelStateTypes.Start;
        }


        private void MuteSound(bool value)
        {
            DataBase.SoundMuted = value;
            AudioListener.pause = value;
        }
        private void SetNextLanguage()
        {
            int currant = Localization.AllLanguages.IndexOf(DataBase.Language);
            if(currant >= Localization.AllLanguages.Count - 1)
            {
                currant = 0;
            }
            else
            {
                currant++;
            }

            DataBase.Language = Localization.AllLanguages[currant];

            localizationController.SetLanguage(DataBase.Language, LocalizationController.Place.Lobby);
        }

        #endregion

        #region Stats

        public void UpdateGameStats()
        {
            List<UIMatchInfo> all = new List<UIMatchInfo>(matchesInfoContent.GetComponentsInChildren<UIMatchInfo>());
            foreach(UIMatchInfo obj in all)
            {
                Destroy(obj.gameObject);
            }

            IEnumerable<DataBase.MatchData> matches = DataBase.GetAllMatches;
            matches = matches.Reverse();

            int count = 0;
            int victory = 0;
            int defeat = 0;
            float kd = 0;

            foreach (DataBase.MatchData match in matches)
            {
                if (count < 10)
                {
                    UIMatchInfo info = Instantiate(matchInfoPrefab, matchesInfoContent);
                    info.Initilize(match.Winner, match.Looser, matchTypeNames[match.EndType], match.Victory);
                }

                count++;
                if(match.Victory)
                {
                    victory++;
                }
                else
                {
                    defeat++;
                }
            }

            kd = (float)victory / (float)(Mathf.Max(count, 1)) * 100;

            statsMatchesCount.text = count.ToString();
            statsVictoryCount.text = victory.ToString();
            statsDefeatCount.text = defeat.ToString();
            statsWinRate.text = kd.ToString("00.00") + "%";
        }

        #endregion

        #region Create Room

        public void OnChangeRoomName()
        {
            DataBase.Room = roomInputName.text;
        }
        public void CreateRoom()
        {
            PhotonNetwork.CreateRoom(DataBase.Room, new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
        }

        #endregion

        #region Join Room

        public void OnChangeJoinRoomName()
        {
            DataBase.PrevRoom = joinRoomName.text;
        }
        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom(DataBase.PrevRoom);
        }

        #endregion

        #region WaitingPlayers

        public void MakeReady()
        {
            if(selfItem)
            {
                switch(buttonState)
                {
                    case ButtonStateTypes.MakeNotReady:
                        selfItem.IsReady = false;
                        break;
                    case ButtonStateTypes.MakeReady:
                        selfItem.IsReady = true;
                        break;
                    case ButtonStateTypes.StartGame:
                        SceneManager.LoadScene(1);
                        return;
                }
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
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            joinRoomError.gameObject.SetActive(true);
            Debug.Log(message);
        }
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log(message);
        }
        public override void OnLeftRoom()
        {
            if (selfItem)
            {
                PhotonNetwork.Destroy(selfItem.gameObject);
            }

            GoMainMenu();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if(Input.GetKeyDown(KeyCode.J))
            {
                PhotonNetwork.JoinRandomRoom();
            }
#endif
        }

        #endregion
    }
}