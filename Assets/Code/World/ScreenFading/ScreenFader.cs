using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeauRoutine;

namespace Wrapper
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField]
        Animator anim;
        Routine fade;

        private void OnEnable()
        {
            Cancel();
            Events.ScreenFadeMidAction += ScreenFadeWithMiddleAction;
            Events.StopScreenFade += Cancel;
        }

        private void OnDisable()
        {
            Events.ScreenFadeMidAction -= ScreenFadeWithMiddleAction;
            Events.StopScreenFade -= Cancel;
        }

        void Cancel()
        {
            if (fade.Exists()) fade.Stop();
            anim.gameObject.SetActive(false);
        }

        void ScreenFadeWithMiddleAction(Action action, float time)
        {
            fade.Replace(ScreenFadeMiddleAction(action, time));
        }

        IEnumerator ScreenFadeMiddleAction(Action action, float waitTime)
        {
            anim.gameObject.SetActive(true);
            anim.SetTrigger("ScreenFader_In");
            yield return 0.4F;

            if (action != null) action();
            yield return waitTime;

            anim.SetTrigger("ScreenFader_Out");
            yield return 0.4F;
            anim.gameObject.SetActive(false);
        }
    }
}