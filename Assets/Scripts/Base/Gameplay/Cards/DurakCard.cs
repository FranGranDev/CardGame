using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Cards
{
    public class DurakCard : Card
    {
        [Header("Components")]
        [SerializeField] private List<TextMeshPro> numbers;
        [SerializeField] private List<SpriteRenderer> icons;
        [Header("State")]
        [SerializeField] private SuitTypes suit;
        [SerializeField] private int index;


        private bool visible = true;
        private CardInfo cardInfo;


        public override bool Takable
        {
            get;
            set;
        }
        public override bool Visible
        {
            get => visible;
            set
            {
                visible = value;
            }
        }
        public override CardInfo Info
        {
            get => cardInfo;
            protected set
            {
                index = value.index;
                suit = (SuitTypes)value.suit;

                cardInfo = value;
            }
        }
        public override int Comparator
        {
            get
            {
                return ((int)suit + 1) * 10 + index;
            }
        }




        public override void Initilize()
        {
            base.Initilize();
        }
        public void Initilize(int index, SuitTypes suit)
        {
            Info = new CardInfo(index, (int)suit);
        }
        public void SetVisual(string name, Sprite suit, Sprite image, Material material)
        {
            foreach(TextMeshPro text in numbers)
            {
                text.text = name;
            }
            foreach(SpriteRenderer icon in icons)
            {
                icon.sprite = suit;
            }
        }


        public override void Return()
        {
            
        }
        public override void Interact(MoveInfo info)
        {
           
        }
        public override void Take(MoveInfo info)
        {
            OnTaken?.Invoke(this);
        }
        public override void Drop(ICardHolder holder, MoveInfo info)
        {
            OnDropped?.Invoke(this);

            holder.Drop(this, new DropCardData { Sender = DropCardData.SenderTypes.Self});
        }
        public override void Drag(ICardHolder holder, MoveInfo info)
        {
            holder.Drag(this, info);
        }
        public override void Drag(MoveInfo info)
        {
            transform.position = info.position;
            transform.up = info.normal;
        }
        public override void Accept(ICardVisitor visitor, object data = null)
        {
            visitor.Visit(this, data);
        }


        public enum SuitTypes
        {
            Spades,
            Diamonds,
            Clubs,
            Hearts,
        }
    }
}