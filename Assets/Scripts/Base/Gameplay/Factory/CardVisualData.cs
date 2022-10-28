using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cards.Data
{
    [CreateAssetMenu(fileName = "CardSkinData", menuName = "CardSkinData", order = 0)]
    public class CardVisualData : ScriptableObject
    {
        private Dictionary<int, string> indexToName = new Dictionary<int, string>()
        {
            {0, "6"},
            {1, "7"},
            {2, "8"},
            {3, "9"},
            {4, "10"},
            {5, "J"},
            {6, "Q"},
            {7, "K"},
            {8, "A"},
        };
        [SerializeField] private List<CardSkinInfo> cardSkins = new List<CardSkinInfo>();
        [SerializeField] private List<Sprite> suitIcons = new List<Sprite>();
        [SerializeField] private DurakCard prefab;

        public Dictionary<int, string> IndexToName => indexToName;
        public List<CardSkinInfo> CardSkins => cardSkins;
        public List<Sprite> SuitIcons => suitIcons;
        public DurakCard Prefab => prefab;


        private void OnValidate()
        {
            int index = 0;
            foreach(CardSkinInfo info in cardSkins)
            {
                info.index = index;
                info.Name = indexToName[info.index];
                index++;
            }
        }
    }

    [Serializable]
    public class CardSkinInfo
    {
        public string Name;
        public int index;
        
        public Material material;
        public Sprite image;
    }
}