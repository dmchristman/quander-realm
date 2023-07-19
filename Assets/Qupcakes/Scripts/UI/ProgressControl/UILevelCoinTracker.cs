using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Qupcakery
{
    public class UILevelCoinTracker : MonoBehaviour
    {
        public Text text;
        public static UILevelCoinTracker instance { get; private set; }

        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
            UpdateCoinAmount(0);
        }

        public void UpdateCoinAmount(int amount)
        {
            PlayerPrefs.SetInt("LevelEarning", amount);
            text.text = System.Convert.ToString(amount);
        }
    }
}
