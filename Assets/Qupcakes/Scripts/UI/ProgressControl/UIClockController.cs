using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/* Controls display of the timer */

namespace Qupcakery
{
    public class UIClockController : MonoBehaviour
    {
        public static UIClockController Instance { get; private set; }

        public Image ClockIndicatorUI;

        void Awake()
        {
            Instance = this;
            SetValue(1f);
        }

        private void SetValue(float value)
        {
            ClockIndicatorUI.fillAmount = value;
        }

        // TimerClicked event subscriber
        public void OnTimerClicked(float totalTime, float remainingTime)
        {
            SetValue(remainingTime / totalTime);
        }
    }
} 