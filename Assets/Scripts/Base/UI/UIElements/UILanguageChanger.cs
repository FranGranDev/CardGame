using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UILanguageChanger : UIClick
    {
        [SerializeField] private Image icon;
        [SerializeField] private Sprite enFlag;
        [SerializeField] private Sprite ruFlag;

        private Dictionary<string, Sprite> languagesSprites;


        public event System.Action OnChanged;


        private void Awake()
        {
            languagesSprites = new Dictionary<string, Sprite>()
            {
                {Localization.AllLanguages[0], enFlag },
                {Localization.AllLanguages[1], ruFlag },
            };

            icon.sprite = languagesSprites[DataBase.Language];
        }

        public void Change()
        {
            Play();

            OnChanged?.Invoke();

            icon.sprite = languagesSprites[DataBase.Language];
        }
    }
}
