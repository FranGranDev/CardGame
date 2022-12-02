using System;
using UnityEngine;

namespace Cards.Data
{
    public class CardFactory : MonoBehaviour
    {        
        [SerializeField] private CardVisualData data;


        public Sprite GetSuitImage(DurakCard.SuitTypes suit)
        {
            return data.SuitIcons[(int)suit];
        }
        public DurakCard CreateCard(int index, DurakCard.SuitTypes suit)
        {
            DurakCard card = Instantiate(data.Prefab);

            card.Initilize(index, suit);

            CardSkinInfo info = data.CardSkins[index];
            card.SetVisual(info.Name, data.SuitIcons[(int)suit], info.image, info.material);

            return card;
        }
    }
}
