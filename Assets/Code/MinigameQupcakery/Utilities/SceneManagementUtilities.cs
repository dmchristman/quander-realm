using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Utility functions that facilitate scene change
 */

namespace Qupcakery
{
    public static class SceneManagementUtilities
    {

        public static void LoadGameScene()
        {
            SceneManager.LoadScene("QU_Level");
        }

        public static void LoadHomePage()
        {
            SceneManager.LoadScene("QU_QupcakesStartScene");
        }

        public static void LoadModeSelectionScene()
        {
            // SceneManager.LoadScene("ModeSelection");
            SceneManager.LoadScene("QU_LevelSelection");
        }

        //public static void LoadExperimentMode()
        //{
        //    SceneManager.LoadScene("ExperimentMode");
        //}

        public static void LoadLevelSelectionMenu()
        {
            if (GameManagement.Instance.game.gameStat.MaxLevelCompleted
            == GameManagement.Instance.GetTotalLevelCnt() 
            & TutorialManager.OutroPlayed == false) {
                Wrapper.Events.StartDialogueSequence?.Invoke("QU_End");
                TutorialManager.UpdateAvailability(0);
            }

            SceneManager.LoadScene("QU_LevelSelection");
        }

        public static void LoadLoseMenu()
        {
            SceneManager.LoadScene("QU_LoseMenu");
        }

        public static void LoadWinMenu()
        {
            SceneManager.LoadScene("QU_WinMenu");
        }
    }
}
