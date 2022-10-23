using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Cards
{
    public abstract class Card : MonoBehaviour, IDragable
    {
        [Header("Settings")]
        [SerializeField] private int index;
        [SerializeField] private Types type;

        [Header("Components")]
        [SerializeField] private TextMeshPro number;
        [SerializeField] private Image icon;


        public Transform Body { get => transform; }


        #region Callbacks

        public CardAction OnTaken { get; set; }
        public CardAction OnDropped { get; set; }

        #endregion


        public abstract void Interact(MoveInfo info);
        public abstract void Take(MoveInfo info);
        public abstract void Drop(ICardHolder holder, MoveInfo info);
        public abstract void Drag(MoveInfo info);

        public abstract void Accept(ICardVisitor visitor);

        public enum Types
        {
            Spades,
            Diamonds,
            Clubs,
            Hearts,
        }
    }
}
