using Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Cards
{
    public class Table : MonoBehaviour, ICardHolder, ICardVisitor
    {
        private List<PlayerWrapper> players;
        private int currantMove;

        private List<CardPair> cardPairs;
        private ICardComparator comparator;

        [SerializeField] private Hand player1;
        [SerializeField] private Hand player2;


        public int CurrantMove
        {
            get => currantMove;
            set
            {
                currantMove = value;
                if(currantMove >= players.Count)
                {
                    currantMove = 0;
                }
                else if(currantMove < 0)
                {
                    currantMove = 0;
                }
            }
        }
        public PlayerWrapper GetPlayer(Card card) 
        {
            return card.Owner;
        } //fix
        public PlayerWrapper Attacker
        {
            get => players[currantMove];
        }
        public PlayerWrapper Defender
        {
            get => players[currantMove + 1];
        }

    
        private void Start()
        {
            Initilize();
        }
        private void Initilize()
        {
            cardPairs = new List<CardPair>();
            comparator = new DurakCardComparator(DurakCard.SuitTypes.Hearts);
        }


        public void Drop(IDragable card)
        {
            card.Accept(this);
        }
        public void Visit(Card card)
        {
            
        }


        public void NextMove()
        {
            CurrantMove++;
        }

    }
}
