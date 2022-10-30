using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class PlayerWrapper
    {
        public PlayerWrapper(int id, Hand hands)
        {
            Id = id;
            Hands = hands;
            Hands.Initilize(this);
        }

        public int Id { get; private set; }
        public Hand Hands { get; private set; }
    }
}
