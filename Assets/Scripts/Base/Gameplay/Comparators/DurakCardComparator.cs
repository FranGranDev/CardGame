using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class DurakCardComparator : ICardComparator
    {
        public DurakCardComparator(DurakCard.SuitTypes trump)
        {
            this.trump = trump;
        }

        private DurakCard.SuitTypes trump;

        public bool Compare(Card card1, Card card2)
        {
            try
            {
                DurakCard defender = card1 as DurakCard;
                DurakCard attacker = card2 as DurakCard;

                if(attacker.Suit == defender.Suit)
                {
                    return attacker.Index > defender.Index;
                }
                else
                {
                    return attacker.Suit == trump;
                }
            }
            catch
            {
                Debug.LogError("Cards is not a durak type!");
            }
            return false;
        }
    }
}
