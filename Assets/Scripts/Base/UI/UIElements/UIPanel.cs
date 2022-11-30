using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;


namespace UI
{
    public abstract class UIPanel : MonoBehaviour, IUiBehavior
    {
        public string Name = "";

        [Header("Show/Hide Animation Settings")]
        [SerializeField] protected UiAnimationData show;
        [SerializeField] protected UiAnimationData hide;
        [Header("Base Component")]
        [SerializeField] protected Transform panel;

        [SerializeField] private bool checkIAddictive = true;

        protected List<IUiBehavior> animations = new List<IUiBehavior>();
        private List<IAddictive> loopAnimations = new List<IAddictive>();

        public System.Action<bool> OnChangeState { get; set; }

        private Tween tween;

        public bool IsShown
        {
            get
            {
                return isShown;
            }
            set
            {
                if (isShown == value)
                    return;

                tween?.Kill();

                isShown = value;
                gameObject.SetActive(true);
                if (value)
                {
                    OnShow();
                }
                else
                {
                    OnHide();
                }
                foreach (IUiBehavior animation in animations)
                {
                    animation.IsShown = value;
                }
                OnChangeState?.Invoke(value);
            }
        }
        protected bool isShown;
        public bool IsPlaying
        {
            get
            {
                if (animations.Count == 0)
                    return false;
                return animations[0].IsPlaying;
            }
        }

        public Transform Panel => panel;

        public void GoTo(Vector2 position, float time, System.Action onComplete, Ease ease = Ease.InOutSine)
        {
            panel.DOMove(position, time).
                SetEase(ease).
                OnComplete(() => onComplete?.Invoke());
        }

        public void Initilize()
        {
            if (panel && checkIAddictive)
            {
                loopAnimations = new List<IAddictive>(panel.GetComponents<IAddictive>());
            }
            AddAnimations();
            OnInitilize();
        }
        protected abstract void AddAnimations();
        protected virtual void OnInitilize()
        {

        }
        protected virtual void OnShow()
        {

        }
        protected virtual void OnHide()
        {

        }

        private void OnValidate()
        {
            if (panel && Name != panel.name)
            {
                Name = "UI | " + panel.name;
            }
        }
        private void Update()
        {
            foreach (IAddictive addictive in loopAnimations)
            {
                addictive.IsPlaying = !IsPlaying && IsShown;
            }
        }
    }
}