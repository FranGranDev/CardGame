using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TouchInput
{
    [RequireComponent(typeof(ITouchMovement))]
    public class CardMovement : MonoBehaviour
    {
        private ITouchMovement touchMovement;

        private void Awake()
        {
            
        }
    }
}
