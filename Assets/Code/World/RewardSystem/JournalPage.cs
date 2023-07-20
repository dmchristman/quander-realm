using BeauRoutine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wrapper
{
    public class JournalPage
    {
        private Toggle navDot;
        private const int maxListSize = 4;

        public int pageNumber;
        public List<GameObject> cardList;

        public JournalPage(int pageNumber)
        {
            cardList = new List<GameObject>();
            this.pageNumber = pageNumber;

            InitNavDot();
        }

        private void InitNavDot()
        {
            navDot = Events.GetNavDot?.Invoke(pageNumber);
            navDot.onValueChanged.AddListener(OpenPage);
        }

        public void OpenPage(bool isOn)
        {
            if (!(isOn))
                return;

            Events.SwitchPage?.Invoke(this);
            SelectFirstUnlockedCard();
        }

        public void ClickNavDot()
        {
            navDot.isOn = true;
        }

        private void SelectFirstUnlockedCard()
        {
            foreach (GameObject cardGO in cardList)
            {
                Reward reward = cardGO.GetComponent<Reward>();
                if (reward.IsUnlocked())
                {
                    Routine.Start(reward.SelectCard());
                    return;
                }
            }
            // set no cards active
            Events.FeatureCard?.Invoke("none");
        }

        public bool IsFull()
        {
            return cardList.Count == maxListSize;
        }

        public void Add(GameObject rewardGO)
        {
            cardList.Add(rewardGO);
        }
    }
}
