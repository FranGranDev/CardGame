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

        public bool CanBeat(Card defender, Card attacker)
        {
            if (defender.Info.suit == attacker.Info.suit)
            {
                return defender.Info.index > attacker.Info.index;
            }
            else
            {
                return (DurakCard.SuitTypes)defender.Info.suit == trump;
            }
        }
        public bool CanPut(Card attacker, CardPair pair)
        {
            bool canPut = pair.Attacker.Info.index == attacker.Info.index;
            canPut = canPut || pair.Done && pair.Defender.Info.index == attacker.Info.index;
            return canPut;
        }
    }
}
