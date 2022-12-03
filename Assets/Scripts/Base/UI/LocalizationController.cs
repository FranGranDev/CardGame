using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace UI
{
    public class LocalizationController : MonoBehaviour
    {
        public enum Place { Lobby, Game}

        public void SetLanguage(string language, Place place)
        {
            List<LocalizationItem> items = new List<LocalizationItem>(GetComponentsInChildren<LocalizationItem>(true));

            Debug.Log($"Set language: {language} | {items.Count}");

            Localization.LoadLanguage(language, this, (data) =>
            {
                foreach (LocalizationItem item in items)
                {
                    if (item == null)
                        continue;
                    SetupItem(item, data, place);
                }
            });
        }

        private void SetupItem(LocalizationItem item, LanguageData data, Place place)
        {
            try
            {
                switch (place)
                {
                    case Place.Lobby:
                        item.SetText((string)data.Lobby.GetType().GetField(item.FieldName).GetValue(data.Lobby));
                        break;
                    case Place.Game:
                        item.SetText((string)data.Game.GetType().GetField(item.FieldName).GetValue(data.Game));
                        break;
                }
            }
            catch
            {
                Debug.LogError($"No such filed finded by id: {item.FieldName}", item.gameObject);
            }
        }
    }
}
