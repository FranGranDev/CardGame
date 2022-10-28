using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class PlayerWrapper
    {
        public PlayerWrapper(string name, Hand hands)
        {
            Name = name;
            Hands = hands;
            Hands.Initilize();
        }

        public string Name { get; private set; }
        public Hand Hands { get; private set; }
    }
}
