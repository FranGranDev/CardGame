using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    public class UIEndGame : UIModifyPanel
    {
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI winnerName;
        [SerializeField] private TextMeshProUGUI looserName;

        public void Show(string winner, string looser, bool canRematch = false)
        {
            winnerName.text = winner;
            looserName.text = looser;

            IsShown = true;
        }
    }
}
