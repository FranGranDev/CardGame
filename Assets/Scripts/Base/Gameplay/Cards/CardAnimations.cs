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
        [Header("Fly Animation")]
        [SerializeField] private float topHeight;
        [SerializeField] private AnimationCurve flyCurve;

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


        public void FlyTo(Vector3 position, Quaternion rotation, float time, ICardAnimation.Order order, Action callback = null)
        {
            CacheAnimation(order, FlyToCour(position, rotation, time, callback), nameof(FlyTo), callback);
        }
        private IEnumerator FlyToCour(Vector3 position, Quaternion rotation, float time, Action callback = null)
        {
            var wait = new WaitForFixedUpdate();
            float currantTime = 0;
            Vector3 startPosition = target.position;
            Quaternion startRotation = target.rotation;

            while(currantTime < time)
            {
                float ratio = currantTime / time;

                Vector3 currantPosition = Vector3.Lerp(startPosition, position, ratio);
                Vector3 height = Vector3.up * flyCurve.Evaluate(ratio) * topHeight;
                Quaternion currantRotation = Quaternion.Lerp(startRotation, rotation, ratio * 2);

                target.transform.position = currantPosition + height;
                target.transform.rotation = currantRotation;


                currantTime += Time.fixedDeltaTime;
                yield return wait;
            }
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
                float ratio = currantTime / time;

                Vector3 currantPosition = Vector3.Lerp(startPosition, position, ratio);
                Quaternion currantRotation = Quaternion.Lerp(startRotation, rotation, ratio);

                target.transform.position = currantPosition;
                target.transform.rotation = currantRotation;


                currantTime += Time.fixedDeltaTime;
                yield return wait;
            }
            currantAnimation = null;

            callback?.Invoke();
            yield break;
        }
    }
}
