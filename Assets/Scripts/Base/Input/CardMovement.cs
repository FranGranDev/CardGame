using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;


namespace TouchInput
{
    [RequireComponent(typeof(ITouchMovement))]
    public class CardMovement : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask cardMask;
        [SerializeField] private LayerMask tableMask;

        private ITouchMovement touchMovement;
        private Dictionary<int, IDragable> currantCards;

        private void Awake()
        {
            Initilize();
        }
        public void Initilize()
        {
            touchMovement = GetComponent<ITouchMovement>();

            touchMovement.OnClick += Click;
            touchMovement.OnDrag += Drag;
            touchMovement.OnStartDrag += StartDrag;
            touchMovement.OnEndDrag += EndDrag;

            currantCards = new Dictionary<int, IDragable>();
        }

        private void Click(TapInfo info)
        {
            Ray ray = Camera.main.ScreenPointToRay(info.endPoint);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 100, cardMask))
            {
                IDragable card = hit.transform.GetComponentInParent<IDragable>();
                if(card != null && card.Takable)
                {
                    Interact(card, new MoveInfo(hit));
                }
            }
        }
        private void StartDrag(TapInfo info)
        {
            Ray ray = Camera.main.ScreenPointToRay(info.endPoint);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500, cardMask))
            {
                IDragable card = hit.transform.GetComponentInParent<IDragable>();
                if (card != null && card.Takable)
                {
                    Take(card, info, new MoveInfo(hit));
                }
            }
        }
        private void EndDrag(TapInfo info)
        {
            if (currantCards.ContainsKey(info.index))
            {
                Ray ray = Camera.main.ScreenPointToRay(info.endPoint);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 500, tableMask))
                {
                    ICardHolder holder = hit.transform.GetComponentInParent<ICardHolder>();
                    if (holder != null)
                    {
                        currantCards[info.index].Drop(holder, new MoveInfo(hit));
                        currantCards.Remove(info.index);
                    }
                }
            }
        }
        private void Drag(TapInfo info)
        {
            if(currantCards.ContainsKey(info.index))
            {
                Ray ray = Camera.main.ScreenPointToRay(info.endPoint);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 500, tableMask))
                {                    
                    currantCards[info.index].Drag(new MoveInfo(hit, 2));
                }
            }
        }


        private void Interact(IDragable card, MoveInfo info)
        {
            card.Interact(info);
        }
        private void Take(IDragable card, TapInfo tapInfo, MoveInfo moveInfo)
        {
            if(currantCards.ContainsKey(tapInfo.index))
            {
                return;
            }

            currantCards.Add(tapInfo.index, card);
            card.Take(moveInfo);
        }
    }
}
