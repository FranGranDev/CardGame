using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Cards
{
    public class DurakCard : Card
    {
        [Header("Settings")]
        [SerializeField] private int index;
        [SerializeField] private SuitTypes suit;

        [Header("Components")]
        [SerializeField] private List<TextMeshPro> numbers;
        [SerializeField] private List<SpriteRenderer> icons;

        private bool visible = true;

        public int Index => index;
        public SuitTypes Suit => suit;

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


        public override void Initilize()
        {
            base.Initilize();
        }
        public void Initilize(int index, SuitTypes suit)
        {


            this.index = index;
            this.suit = suit;
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


        public override void Interact(MoveInfo info)
        {
           
        }
        public override void Take(MoveInfo info)
        {
            OnTaken?.Invoke(this);
        }
        public override void Drop(ICardHolder holder, MoveInfo info)
        {
            transform.position = info.position;
            transform.up = info.normal;

            holder.Drop(this, new DropCardData { Sender = DropCardData.SenderTypes.Self});
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