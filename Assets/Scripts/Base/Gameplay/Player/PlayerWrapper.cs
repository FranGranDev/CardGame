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

        public System.Action<PlayerWrapper> OnStateChanged { get; set; }
        public System.Action<PlayerWrapper> OnAction { get; set; }


        public void DoAction()
        {
            OnAction?.Invoke(this);
        }
        public void Accept(Data data)
        {
            MoveState = (MoveStates)data.moveState;
            PlayerState = (PlayerStates)data.playerState;
        }
        


        public enum MoveStates
        {
            Idle,
            Playing,
            TakingCards,
            ThrowingCards,
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
                moveState = (int)player.moveState;
                playerState = (int)player.playerState;
            }
            public Data(int moveState, int playerState)
            {
                this.moveState = moveState;
                this.playerState = playerState;
            }


            public readonly int moveState;
            public readonly int playerState;
        }
    }
}
