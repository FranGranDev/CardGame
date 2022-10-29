using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;

namespace Cards
{
    public class CardAnimations : MonoBehaviour, ICardAnimation
    {
        [Header("Settings")]
        [SerializeField] private Transform target;
        [Header("Move Animation")]
        [SerializeField] private AnimationCurve moveEase;
        [Header("Fly Animation")]
        [SerializeField] private AnimationCurve flyEase;
        [SerializeField] private float flyTopHeight;
        [SerializeField] private AnimationCurve flyCurve;
        [Header("Discard Animation")]
        [SerializeField] private AnimationCurve discardEase;
        [SerializeField] private float discardTopHeight;
        [SerializeField] private AnimationCurve discardCurve;

        private Coroutine currantAnimation;


        private void StopPrevAnimation()
        {
            if (currantAnimation != null)
            {
                StopCoroutine(currantAnimation);
                currantAnimation = null;
            }
        }


        private void CacheAnimation(ICardAnimation.Order order, IEnumerator coroutine, string name, Action onDone)
        {
            switch (order)
            {
                case ICardAnimation.Order.Override:
                    StopPrevAnimation();

                    currantAnimation = StartCoroutine(coroutine);
                    break;
                case ICardAnimation.Order.IfNotPlaying:
                    if(currantAnimation == null)
                    {
                        currantAnimation = StartCoroutine(coroutine);
                    }
                    break;
            }
        }


        public void FlyToHand(Vector3 position, Quaternion rotation, float time, ICardAnimation.Order order, Action callback = null)
        {
            CacheAnimation(order, FlyToHandCour(position, rotation, time, callback), nameof(FlyToHand), callback);
        }
        private IEnumerator FlyToHandCour(Vector3 position, Quaternion rotation, float time, Action callback = null)
        {
            var wait = new WaitForFixedUpdate();
            float currantTime = 0;
            Vector3 startPosition = target.position;
            Quaternion startRotation = target.rotation;

            while(currantTime < time)
            {
                float ratio = flyEase.Evaluate(currantTime / time);

                Vector3 currantPosition = Vector3.Lerp(startPosition, position, ratio);
                Vector3 height = Vector3.up * flyCurve.Evaluate(ratio) * flyTopHeight;
                Quaternion currantRotation = Quaternion.Lerp(startRotation, rotation, ratio * 2);

                target.transform.position = currantPosition + height;
                target.transform.rotation = currantRotation;


                currantTime += Time.fixedDeltaTime;
                yield return wait;
            }

            target.transform.position = position;
            target.transform.rotation = rotation;
            currantAnimation = null;


            callback?.Invoke();
            yield break;
        }

        public void DiscardMove(Vector3 position, Quaternion rotation, float time, ICardAnimation.Order order, Action callback = null)
        {
            CacheAnimation(order, DiscardMoveCour(position, rotation, time, callback), nameof(DiscardMove), callback);
        }
        private IEnumerator DiscardMoveCour(Vector3 position, Quaternion rotation, float time, Action callback = null)
        {
            var wait = new WaitForFixedUpdate();
            float currantTime = 0;
            Vector3 startPosition = target.position;
            Quaternion startRotation = target.rotation;

            while (currantTime < time)
            {
                float ratio = discardEase.Evaluate(currantTime / time);

                Vector3 currantPosition = Vector3.Lerp(startPosition, position, ratio);
                Vector3 height = Vector3.up * discardCurve.Evaluate(ratio) * discardTopHeight;
                Quaternion currantRotation = Quaternion.Lerp(startRotation, rotation, ratio);

                target.transform.position = currantPosition + height;
                target.transform.rotation = currantRotation;


                currantTime += Time.fixedDeltaTime;
                yield return wait;
            }

            target.transform.position = position;
            target.transform.rotation = rotation;
            currantAnimation = null;


            callback?.Invoke();
            yield break;
        }


        public void MoveTo(Vector3 position, Quaternion rotation, float time, ICardAnimation.Order order, Action callback = null)
        {
            CacheAnimation(order, MoveToCour(position, rotation, time, callback), nameof(MoveTo), callback);
        }
        private IEnumerator MoveToCour(Vector3 position, Quaternion rotation, float time, Action callback = null)
        {
            var wait = new WaitForFixedUpdate();
            float currantTime = 0;
            Vector3 startPosition = target.position;
            Quaternion startRotation = target.rotation;


            while (currantTime < time)
            {
                float ratio = moveEase.Evaluate(currantTime / time);

                Vector3 currantPosition = Vector3.Lerp(startPosition, position, ratio);
                Quaternion currantRotation = Quaternion.Lerp(startRotation, rotation, ratio);

                target.position = currantPosition;
                target.rotation = currantRotation;


                currantTime += Time.fixedDeltaTime;
                yield return wait;
            }
            target.position = position;
            target.rotation = rotation;
            currantAnimation = null;

            callback?.Invoke();
            yield break;
        }
    }
}
