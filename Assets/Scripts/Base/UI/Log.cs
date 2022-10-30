using UnityEngine;
using TMPro;


namespace UI
{
    public class Log : MonoBehaviour
    {
        public Log()
        {
            active = this;
        }
        private static Log active;

        [SerializeField] private TextMeshProUGUI text;


        public static void Do(string message)
        {
            active.text.text += (message + '\n');
            Debug.Log(message);
        }
    }
}
