using Cards;
using Cards.Data;
using System.Collections.Generic;
using UnityEngine;


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
        [SerializeField] private HostController hostController;
        [SerializeField] private Table table;
        [SerializeField] private PlayerCamera playerCamera;
        [SerializeField] private Hand player;
        [SerializeField] private Hand enemy;
        [SerializeField] private Deck deck;
        [SerializeField] private DiscardPile discardPile;
        [SerializeField] private CardFactory cardFactory;

        #endregion

        #region States

        private List<PlayerWrapper> players;

        #endregion

        private void Start()
        {
            InitilizeGame();
        }

        private void InitilizeGame()
        {
            players = hostController.Initilize(table, deck, player, enemy);

            bool isOffline = players.Count < 2;
            if (isOffline)
            {
                players.Add(new PlayerWrapper(404, enemy));
                Debug.LogWarning("Test game due to 1 player");
            }

            DurakCard.SuitTypes trump = DurakCard.SuitTypes.Hearts;

            table.Initilize(players, deck, discardPile, new DurakCardComparator(trump));
            deck.Initilize(players, cardFactory, trump);


            hostController.StartGame(isOffline);
        }
    }
}