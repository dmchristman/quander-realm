using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Next level button on win menu


namespace Qupcakery
{
    public class NextLevelButton : MonoBehaviour
    {
        public void ContinueToNextLevel()
        {
            int currentLevelInd = GameManagement.Instance.GetCurrentLevelInd();

            // If this is the last level, go to level selection menu
            if (currentLevelInd == GameManagement.Instance.GetTotalLevelCnt())
                SceneManagementUtilities.LoadLevelSelectionMenu();
            else
            {
                GameManagement.Instance.SetCurrentLevel(currentLevelInd + 1);
                SceneManagementUtilities.LoadGameScene();
            }
        }
    }
}
