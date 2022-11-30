using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cards;
using TMPro;

namespace UI
{
    public class PlayerUI : MonoBehaviour
    {
        [Header("States")]
        [SerializeField] private PlayerWrapper.PlayerStates playerState;
        [SerializeField] private PlayerWrapper.MoveStates moveState;

        [Space, Header("End Game View")]
        [SerializeField] private UIPanel endBackground;
        [SerializeField] private UIEndGame victory;
        [SerializeField] private UIEndGame defeat;

        [Space, Header("Main Button")]
        [SerializeField] private UIPanel buttonPanel;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Button button;

        private PlayerWrapper player;
        private PlayerWrapper enemy;


        private UIEndGame currantEndPanel;


        public event System.Action OnSurrender;

        public event System.Action OnReady;
        public event System.Action OnRematch;
        public event System.Action OnExit;


        public bool IsMainButtonsShown
        {
            get => buttonPanel.IsShown;
            set
            {
                if (buttonPanel.IsShown == value)
                    return;
                buttonPanel.IsShown = value;
            }
        }


        public void Initilize(PlayerWrapper player, PlayerWrapper enemy)
        {
            this.player = player;
            this.enemy = enemy;

            player.OnStateChanged += UpdatePlayerState;
            enemy.OnStateChanged += UpdateEnemyState;


            foreach(UIPanel panel in GetComponentsInChildren<UIPanel>(true))
            {
                panel.Initilize();
            }
        }


        #region In Game Panel

        public void PlayerAction()
        {
            player.DoAction();
        }

        public void Leave()
        {
            OnSurrender?.Invoke();
        }

        #endregion

        #region End Panel

        public void Victory(Table.MatchData matchData, bool isServer)
        {
            ShowEndPopup(victory, matchData, isServer);
        }
        public void Defeat(Table.MatchData matchData, bool isServer)
        {
            ShowEndPopup(defeat, matchData, isServer);
        }

        public void MakeReady()
        {
            currantEndPanel?.UpdateState(UIEndGame.States.IamReady);
        }
        public void AllPlayersReady()
        {
            currantEndPanel?.UpdateState(UIEndGame.States.Rematch);
        }
        public void OtherExit()
        {
            currantEndPanel?.UpdateState(UIEndGame.States.Exit);
        }

        private void ShowEndPopup(UIEndGame panel, Table.MatchData matchData, bool isServer)
        {
            currantEndPanel = panel;

            UIEndGame.States state = isServer ? UIEndGame.States.WaitForPlayers : UIEndGame.States.NotReady;
            if (matchData.EndType == Table.MatchData.EndTypes.Exit)
            {
                state = UIEndGame.States.Exit;
            }

            panel.Show(matchData, state, Ready, Rematch, Exit);
            endBackground.IsShown = true;
        }

        private void Ready()
        {
            OnReady?.Invoke();
        }
        private void Exit()
        {
            OnExit?.Invoke();
        }
        private void Rematch()
        {
            OnRematch?.Invoke();
        }

        #endregion

        #region Update

        private void UpdatePlayerState(PlayerWrapper player)
        {
            playerState = player.PlayerState;
            moveState = player.MoveState;

            UpdateButton(player);
        }
        private void UpdateButton(PlayerWrapper player)
        {
            switch (player.PlayerState)
            {
                case PlayerWrapper.PlayerStates.Attacker:
                    switch (player.MoveState)
                    {
                        case PlayerWrapper.MoveStates.Idle:
                            button.interactable = false;
                            IsMainButtonsShown = false;
                            buttonText.text = "Wait";
                            break;
                        case PlayerWrapper.MoveStates.Playing:
                            button.interactable = true;
                            IsMainButtonsShown = true;
                            buttonText.text = "Done";
                            break;
                        case PlayerWrapper.MoveStates.Pass:
                            button.interactable = true;
                            IsMainButtonsShown = true;
                            buttonText.text = "Pass";
                            break;
                        case PlayerWrapper.MoveStates.FirstMove:
                            button.interactable = false;
                            IsMainButtonsShown = true;
                            buttonText.text = "Your turn";
                            break;
                        case PlayerWrapper.MoveStates.EnemyMove:
                            button.interactable = false;
                            IsMainButtonsShown = true;
                            buttonText.text = "Enemy turn";
                            break;
                    }
                    break;
                case PlayerWrapper.PlayerStates.Defender:
                    switch (player.MoveState)
                    {
                        case PlayerWrapper.MoveStates.Idle:
                            button.interactable = false;
                            IsMainButtonsShown = false;
                            buttonText.text = "Wait";
                            break;
                        case PlayerWrapper.MoveStates.Playing:
                            button.interactable = true;
                            IsMainButtonsShown = true;
                            buttonText.text = "Take";
                            break;
                        case PlayerWrapper.MoveStates.Pass:
                            button.interactable = false;
                            IsMainButtonsShown = false;
                            break;
                        case PlayerWrapper.MoveStates.FirstMove:
                            button.interactable = false;
                            IsMainButtonsShown = true;
                            buttonText.text = "Enemy turn";
                            break;
                        case PlayerWrapper.MoveStates.EnemyMove:
                            button.interactable = false;
                            IsMainButtonsShown = true;
                            buttonText.text = "Enemy turn";
                            break;
                    }
                    break;
                case PlayerWrapper.PlayerStates.Idle:
                    button.interactable = false;
                    IsMainButtonsShown = false;
                    buttonText.text = "Wait";
                    break;
            }
        }
        private void UpdateEnemyState(PlayerWrapper player)
        {
            
        }

        #endregion
    }

}
