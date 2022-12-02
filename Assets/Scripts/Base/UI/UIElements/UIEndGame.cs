using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cards;
using TMPro;

namespace UI
{
    public class UIEndGame : UIModifyPanel
    {
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI winnerName;
        [SerializeField] private TextMeshProUGUI looserName;
        [SerializeField] private TextMeshProUGUI moveCount;
        [SerializeField] private TextMeshProUGUI readyButtonText;
        [SerializeField] private TextMeshProUGUI rematchButtonText;
        [Header("Buttons")]
        [SerializeField] private Button readyButton;
        [SerializeField] private Button rematchButton;
        [Header("Panels")]
        [SerializeField] private UIPanel readyButtonPanel;
        [SerializeField] private UIPanel rematchButtonPanel;


        private System.Action onReady;
        private System.Action onRematch;
        private System.Action onExit;

        public void Show(Table.MatchData matchData, States state, System.Action onReady, System.Action onRematch, System.Action onExit)
        {
            this.onReady = onReady;
            this.onRematch = onRematch;
            this.onExit = onExit;

            winnerName.text = matchData.Winner.Name;
            looserName.text = matchData.Looser.Name;
            moveCount.text = matchData.MoveCount.ToString();

            IsShown = true;

            UpdateState(state);
        }
        public void UpdateState(States state)
        {
            switch(state)
            {
                case States.Exit:
                    readyButtonPanel.IsShown = false;
                    rematchButtonPanel.IsShown = false;
                    break;
                case States.NotReady:
                    readyButtonText.text = "I am Ready";
                    readyButton.interactable = true;
                    readyButtonPanel.IsShown = true;
                    rematchButtonPanel.IsShown = false;
                    break;
                case States.IamReady:
                    readyButtonText.text = "Wait others";
                    readyButtonPanel.IsShown = true;
                    readyButton.interactable = false;
                    rematchButtonPanel.IsShown = false;
                    break;
                case States.Rematch:
                    rematchButtonText.text = "Rematch";
                    rematchButton.interactable = true;
                    readyButtonPanel.IsShown = false;
                    rematchButtonPanel.IsShown = true;
                    break;
                case States.WaitForPlayers:
                    rematchButtonText.text = "Wait others";
                    rematchButton.interactable = false;
                    readyButtonPanel.IsShown = false;
                    rematchButtonPanel.IsShown = true;
                    break;
            }
        }

        public void Ready()
        {
            onReady?.Invoke();
        }
        public void Rematch()
        {
            onRematch?.Invoke();
        }

        public void Close()
        {
            onExit?.Invoke();
        }

        public enum States
        {
            Exit,
            NotReady,
            IamReady,
            WaitForPlayers,
            Rematch,
        }
    }
}
