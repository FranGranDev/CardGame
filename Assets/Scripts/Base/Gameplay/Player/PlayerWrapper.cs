using UnityEngine;


namespace Cards
{
    [System.Serializable]
    public class PlayerWrapper
    {
        public PlayerWrapper(int id, Hand hands, bool local = false)
        {
            Id = id;
            Local = local;
            Hands = hands;
            Hands.Initilize(this);

            MoveState = MoveStates.Idle;
            PlayerState = PlayerStates.Idle;
        }

        [SerializeField] private bool local;
        [SerializeField] private int id;
        [SerializeField] private Hand hands;
        [SerializeField] private MoveStates moveState;
        [SerializeField] private PlayerStates playerState;



        public bool Local
        {
            get => local;
            private set => local = value;
        }
        public int Id
        {
            get => id;
            private set => id = value;
        }
        public Hand Hands
        {
            get => hands;
            private set => hands = value;
        }
        public MoveStates MoveState
        {
            get => moveState;
            set
            {
                moveState = value;

                OnStateChanged?.Invoke(this);
            }
        }
        public PlayerStates PlayerState
        {
            get => playerState;
            set
            {
                playerState = value;

                OnStateChanged?.Invoke(this);
            }
        }

        public event System.Action<PlayerWrapper> OnStateChanged;
        public event System.Action<PlayerWrapper> OnAction;


        public void DoAction()
        {
            OnAction?.Invoke(this);
        }
        


        public enum MoveStates
        {
            Idle,
            Playing,
            FirstMove,
            EnemyMove,
            Pass,
        }
        public enum PlayerStates
        {
            Idle,
            Attacker,
            Defender,
        }
        public class Data
        {
            public Data(PlayerWrapper player)
            {
                id = player.id;
                moveState = player.moveState;
                playerState = player.playerState;
            }
            public Data(int id, int moveState, int playerState)
            {
                this.id = id;
                this.moveState = (MoveStates)moveState;
                this.playerState = (PlayerStates)playerState;
            }

            public readonly int id;
            public readonly MoveStates moveState;
            public readonly PlayerStates playerState;
        }
    }
}
