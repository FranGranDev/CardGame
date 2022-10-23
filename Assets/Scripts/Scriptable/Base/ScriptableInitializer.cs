using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class ScriptableInitializer : MonoBehaviour
    {
        [SerializeField] private List<DataHolder> dataHolders;


        private void Awake()
        {
            foreach (DataHolder data in dataHolders)
            {
                data.Init();
            }
        }
    }
}