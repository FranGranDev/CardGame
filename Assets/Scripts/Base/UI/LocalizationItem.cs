using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizationItem : MonoBehaviour
    {
        [SerializeField] private string fieldName;

        public string FieldName => fieldName;

        public void SetText(string text)
        {
            TextMeshProUGUI textMeshPro = GetComponent<TextMeshProUGUI>();
            textMeshPro.text = text;
        }
    }
}
