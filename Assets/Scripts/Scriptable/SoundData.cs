using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "Sounds")]
    public class SoundData : ScriptableObject
    {
        [SerializeField] private List<SoundItem> soundItems = new List<SoundItem>();
        [SerializeField] private List<SoundItem> musicItems = new List<SoundItem>();

        public List<SoundItem> Sounds { get { return soundItems; } }
        public List<SoundItem> Music { get { return soundItems; } }
    }
}