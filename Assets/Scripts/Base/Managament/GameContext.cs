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

        [Header("State")]
        [SerializeField] private States currantState;
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
        [Header("Players")]
        [SerializeField] private List<PlayerWrapper> players;
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
                players.Add(new PlayerWrapper(404, enemy, "Local enemy"));
                Debug.LogWarning("Test game due to 1 player");
            }

            table.Initilize(players, deck, discardPile, new DurakCardComparator(trump));
            deck.Initilize(players, cardFactory, trump);
            playerUI.Initilize(Self, Enemy);
            enemyUI.Initilize(Enemy, Self); //Debug


            hostController.OnOtherReadyChanged += OtherReady;
            hostController.OnOtherExit += OtherExit;

            table.OnGameEndedLocal += GameEnded;

            playerUI.OnRematch += Rematch;
            playerUI.OnReady += Ready;
            playerUI.OnSurrender += Surrender;
            playerUI.OnExit += Exit;

            currantState = States.Game;
            hostController.StartGame(isOffline);
        }


        private void GameEnded(Table.MatchData data)
        {
            DataBase.RecordGame(data);
            currantState = States.Ended;


            if (data.Winner.Local)
            {
                playerUI.Victory(data, hostController.isServer);
            }
            else
            {
                playerUI.Defeat(data, hostController.isServer);
            }
        }
        private void RestartGame()
        {

        }
        private void OtherExit(PlayerWrapper other)
        {
            switch (currantState)
            {
                case States.Ended:
                    playerUI.OtherExit();
                    break;
                case States.Game:
                    table.OtherLeave(Self);
                    break;
            }
        }
        private void Exit()
        {
            switch (currantState)
            {
                case States.Game:
                    table.SelfLeave(Self);
                    break;
            }

            hostController.LeaveRoom();
        }


        private void OtherReady(PlayerWrapper other)
        {
            if (!hostController.isServer)
                return;

            Self.Ready = true;
            if (players.Count(x => x.Ready != true) == 0)
            {
                playerUI.AllPlayersReady();
            }
        }
        private void Ready()
        {
            if (Self.Ready)
                return;
            Self.Ready = true;
            hostController.OnPlayerReadyChanged(Self);
            playerUI.MakeReady();
        }
        private void Surrender()
        {
            table.Surrender(Self);
        }
        private void Rematch()
        {
            if (!hostController.isServer)
                return;

            Self.Ready = true;
            if(players.Count(x => x.Ready != true) == 0)
            {
                hostController.RestartGame();
            }
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

            if (Input.GetKeyDown(KeyCode.R))
            {
                hostController.RestartGame();
            }
        }
        private void OnApplicationQuit()
        {
            Exit();
        }

        public enum States { NotStarted, Game, Ended, }
    }
}