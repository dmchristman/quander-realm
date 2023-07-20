using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Qupcakery
{
    public class WinMenuController : MonoBehaviour
    {
        public Image star1, star2, star3;
        public Sprite fullStar;

        void Start()
        {
            int levelInd = GameManagement.Instance.GetCurrentLevelInd();
            int starCnt = GameUtilities.GetLevelResult(levelInd);

            switch (starCnt)
            {
                case 1:
                    star1.sprite = fullStar;
                    break;
                case 2:
                    star1.sprite = fullStar;
                    star2.sprite = fullStar;
                    break;
                case 3:
                    star1.sprite = fullStar;
                    star2.sprite = fullStar;
                    star3.sprite = fullStar;
                    break;
            }

            if (RewardManager.rewardsInd.ContainsKey(levelInd))
            {
                bool isUnlocked = Wrapper.Events.IsRewardUnlocked?
                    .Invoke(RewardManager.rewardsInd[levelInd]) ?? false;
                if (! isUnlocked)
                    Wrapper.Events.CollectAndDisplayReward?.Invoke(Wrapper.Game.Qupcakes, levelInd);
            }
        }
    }
}
