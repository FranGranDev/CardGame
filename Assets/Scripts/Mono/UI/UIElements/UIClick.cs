using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace UI
{
    public class UIClick : MonoBehaviour, IUIClickBehavior
    {
        [SerializeField] private AnimationTypes animationType;
        [SerializeField] private Transform panel;
        [SerializeField] private float value = 0.33f;
        [SerializeField] private float time = 0.5f;
        [SerializeField] private int vibration = 5;
        [SerializeField] private Ease ease = Ease.InOutSine;
        [Space]
        [SerializeField] private bool playSound;
        [SerializeField] private string soundId;
        [SerializeField] private float soundVolume;
        [SerializeField] private float soundDelay;

        [SerializeField] private bool checkIAddictive = true;

        private IUIClickBehavior behavior;
        private List<IAddictive> loopAnimations = new List<IAddictive>();

        public bool IsPlaying
        {
            get => behavior.IsPlaying;
        }


        public void Play()
        {
            behavior.Play();

            if (playSound)
            {
                //SoundManagment.PlaySound(soundId, null, 1, soundDelay);
            }
        }

        private void Start()
        {
            switch (animationType)
            {
                case AnimationTypes.Rotation:
                    behavior = new UIClickRotation(panel, value, time, vibration, ease);
                    break;
                case AnimationTypes.Scale:
                    behavior = new UIClickScale(panel, value, time, vibration, ease);
                    break;
            }
            if (checkIAddictive)
            {
                loopAnimations = new List<IAddictive>(panel.GetComponentsInChildren<IAddictive>(true));
            }
        }
        private void OnValidate()
        {
            if (panel == null)
            {
                panel = transform;
            }
        }

        public enum AnimationTypes
        {
            Scale, Rotation,
        }

        private void Update()
        {
            foreach (IAddictive addictive in loopAnimations)
            {
                addictive.IsPlaying = !IsPlaying;
            }
        }
    }
}