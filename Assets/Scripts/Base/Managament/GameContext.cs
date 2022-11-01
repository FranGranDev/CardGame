using Cards;
using UI;
using Cards.Data;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


namespace Managament
{
    public class GameContext : MonoBehaviour
    {
        #region Links

        [Header("Links")]
        [SerializeField] private HostController hostController;
        [Space]
        [SerializeField] private Table table;
        [SerializeField] private PlayerCamera playerCamera;
        [Space]
        [SerializeField] private Hand player;
        [SerializeField] private PlayerUI playerUI;
        [Space]
        [SerializeField] private Hand enemy;
        [SerializeField] private PlayerUI enemyUI; //Only for tests
        [Space]
        [SerializeField] private Deck deck;
        [SerializeField] private DiscardPile discardPile;
        [Space]
        [SerializeField] private CardFactory cardFactory;

        #endregion

        private List<PlayerWrapper> players;
        private bool localPlayer = true;

        private PlayerWrapper Self
        {
            get => players.First(x => x.Local == true);
        }
        private PlayerWrapper Enemy
        {
            get => players.First(x => x.Local != true);
        }



        private void Start()
        {
            hostController.ServerInitilize(InitilizeGame);
        }

        private void InitilizeGame(HostController.Data serverData)
        {            
            DurakCard.SuitTypes trump = serverData.Trump;


            players = hostController.LocalInitilize(table, deck, player, enemy);

            bool isOffline = players.Count < 2;
            if (isOffline)
            {
                players.Add(new PlayerWrapper(404, enemy));
                Debug.LogWarning("Test game due to 1 player");
            }

            table.Initilize(players, deck, discardPile, new DurakCardComparator(trump));
            deck.Initilize(players, cardFactory, trump);
            playerUI.Initilize(Self, Enemy);
            enemyUI.Initilize(Enemy, Self); //Debug

            hostController.StartGame(isOffline);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                localPlayer = !localPlayer;
                playerCamera.Switch();
                playerUI.gameObject.SetActive(localPlayer);                
                enemyUI.gameObject.SetActive(!localPlayer);                
            }
        }
    }
}