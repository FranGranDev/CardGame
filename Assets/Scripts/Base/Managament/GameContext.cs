using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards.Data;
using Cards;

namespace Managament
{
    public class GameContext : MonoBehaviour
    {
        public static GameContext Main { get; private set; }
        public GameContext()
        {
            Main = this;
        }


        #region Links

        [Header("Links")]
        [SerializeField] private Table table;
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private Hand player;
        [SerializeField] private Hand enemy;
        [SerializeField] private Deck deck;
        [SerializeField] private CardFactory cardFactory;

        #endregion


        private void Start()
        {
            table.Initilize();
        }
    }
}