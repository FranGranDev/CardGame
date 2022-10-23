using UnityEngine;
using DG.Tweening;

public class UiMoveBehavior : UiBehavior
{
    private Transform showPoint;
    private Transform hidePoint;

    public UiMoveBehavior(Transform panel, UiAnimationData showData, UiAnimationData hideData, Transform showPoint, Transform hidePoint) : base(panel, showData, hideData)
    {
        this.showPoint = showPoint;
        this.hidePoint = hidePoint;
    }

    public override void Hide()
    {
        if (tween.IsActive())
        {
            tween.Kill();
        }
        IsPlaying = true;
        tween = panel.DOMove(hidePoint.position, hideData.time).
            SetDelay(hideData.delay + Time.fixedDeltaTime).
            SetEase(hideData.ease).
            SetUpdate(true).
            OnComplete(() => { 
                panel.gameObject.SetActive(false);
                IsPlaying = false;
            });
    }

    public override void Show()
    {
        if (!tween.IsActive())
        {
            panel.position = hidePoint.position;
        }
        else if (tween.IsPlaying())
        {
            tween.Kill();
        }

        panel.gameObject.SetActive(true);
        IsPlaying = true;
        tween = panel.DOMove(showPoint.position, showData.time).
            SetDelay(showData.delay + Time.fixedDeltaTime).
            SetEase(showData.ease).
            SetUpdate(true).
            OnComplete(() => IsPlaying = false);
    }
}
