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
        
        [Space, Header("Main Button")]
        [SerializeField] private UIPanel buttonPanel;
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Button button;


        private PlayerWrapper player;
        private PlayerWrapper enemy;


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


        #region Buttons

        public void PlayerAction()
        {
            player.DoAction();
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
                            buttonPanel.IsShown = false;
                            buttonText.text = "Wait";
                            break;
                        case PlayerWrapper.MoveStates.Playing:
                            button.interactable = true;
                            buttonPanel.IsShown = true;
                            buttonText.text = "Done";
                            break;
                        case PlayerWrapper.MoveStates.Pass:
                            button.interactable = true;
                            buttonPanel.IsShown = true;
                            buttonText.text = "Pass";
                            break;
                    }
                    break;
                case PlayerWrapper.PlayerStates.Defender:
                    switch (player.MoveState)
                    {
                        case PlayerWrapper.MoveStates.Idle:
                            button.interactable = false;
                            buttonPanel.IsShown = false;
                            buttonText.text = "Wait";
                            break;
                        case PlayerWrapper.MoveStates.Playing:
                            button.interactable = true;
                            buttonPanel.IsShown = true;
                            buttonText.text = "Take";
                            break;
                        case PlayerWrapper.MoveStates.Pass:
                            button.interactable = false;
                            buttonPanel.IsShown = false;
                            break;
                    }
                    break;
                case PlayerWrapper.PlayerStates.Idle:
                    button.interactable = false;
                    buttonPanel.IsShown = false;
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
