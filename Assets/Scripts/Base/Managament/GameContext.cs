using Cards;
using UI;
using Cards.Data;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;


namespace Managament
{
    public class GameContext : MonoBehaviour
    {
        #region Links

        [Header("States")]
        [SerializeField] private States state;
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


            state = States.Game;

            table.OnGameEnded += GameEnded;
            playerUI.OnLeave += Leave;
            playerUI.OnExit += Exit;


            hostController.StartGame(isOffline);
        }

        private void GameEnded(Table.MatchData data)
        {
            DataBase.RecordGame(data);

            if(data.Winner.Local)
            {
                playerUI.Victory(data);
            }
            else
            {
                playerUI.Defeat(data);
            }
        }
        private void Leave()
        {
            table.Surrender(Self);
        }
        private void Exit()
        {
            SceneManager.LoadScene(0);
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


        public enum States { NotStarted, Game, Ended, }
    }
}