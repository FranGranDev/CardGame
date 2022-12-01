using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class UIMatchInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI winner;
        [SerializeField] private TextMeshProUGUI looser;
        [SerializeField] private TextMeshProUGUI type;
        [SerializeField] private Image image;

        [SerializeField] private Color winColor;
        [SerializeField] private Color loseColor;

        public void Initilize(string winner, string looser, string type, bool isWin)
        {
            this.winner.text = winner;
            this.looser.text = looser;
            this.type.text = type;

            Color color = isWin ? winColor : loseColor;
            image.color = color;
        }
    }
}
