using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI
{
    public class GameLocalizationItem : MonoBehaviour, ILocalizationItem
    {
        [SerializeField] private string fieldName;

        public string FieldName => fieldName;

        public void SetText(string text)
        {
            TextMeshPro textMeshPro = GetComponent<TextMeshPro>();
            textMeshPro.text = text;
        }
        
    }
}
