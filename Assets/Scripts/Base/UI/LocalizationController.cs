using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Data;

namespace UI
{
    public class LocalizationController : MonoBehaviour
    {
        public enum Place { Lobby, Game}


        public void SetLanguage(string language, Place place)
        {
            List<ILocalizationItem> items = new List<ILocalizationItem>();
            Scene scene = SceneManager.GetActiveScene();
            
            foreach (GameObject obj in scene.GetRootGameObjects())
            {
                items.AddRange(obj.GetComponentsInChildren<ILocalizationItem>(true));
            }

            Localization.LoadLanguage(language, this, (data) =>
            {
                foreach (ILocalizationItem item in items)
                {
                    if (item == null)
                        continue;
                    SetupItem(item, data, place);
                }
            });
        }

        private void SetupItem(ILocalizationItem item, LanguageData data, Place place)
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
                Debug.LogError($"No such filed finded by id: {item.FieldName}");
            }
        }
    }
}
