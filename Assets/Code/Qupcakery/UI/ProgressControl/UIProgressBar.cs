using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Controls display of the level earning */

namespace Qupcakery
{
    public class UIProgressBar : MonoBehaviour
    {
        public static UIProgressBar Instance { get; private set; }

        public Image progressIndicatorUI;

        public Image mask;
        public Image starMask1;
        public Image starMask20, starMask21;
        public Image starMask30, starMask31, starMask32;

        // Level goal
        private int goal = 0;
        private int currentProgress = 0;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            progressIndicatorUI.fillAmount = 0;

            starMask1.rectTransform.localScale = new Vector3(0f, 1f, 1f);
            starMask20.rectTransform.localScale = new Vector3(0f, 1f, 1f);
            starMask21.rectTransform.localScale = new Vector3(0f, 1f, 1f);
            starMask30.rectTransform.localScale = new Vector3(0f, 1f, 1f);
            starMask31.rectTransform.localScale = new Vector3(0f, 1f, 1f);
            starMask32.rectTransform.localScale = new Vector3(0f, 1f, 1f);
        }

        public void SetValue(float value)
        {
            progressIndicatorUI.fillAmount = value;

            if (value > 0.6f)
                starMask1.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            if (value > 0.8f)
            {
                starMask20.rectTransform.localScale = new Vector3(1f, 1f, 1f);
                starMask21.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            }
            if (value > 0.99f)
            {
                starMask30.rectTransform.localScale = new Vector3(1f, 1f, 1f);
                starMask31.rectTransform.localScale = new Vector3(1f, 1f, 1f);
                starMask32.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        // Sets the level goal
        public void SetGoal(int levelGoal)
        {
            goal = levelGoal;
        }

        public float GetCompletionRate()
        {
            return (float)currentProgress / (float)goal;
        }

        public int GetCurrentEarning()
        {
            return currentProgress;
        }

        // CoinCollected event subscriber
        public void OnCoinsCollected(int amount)
        {
            currentProgress += amount;
            SetValue((float)currentProgress / (float)goal);
            UILevelCoinTracker.instance.UpdateCoinAmount(currentProgress);
        }
    }
}