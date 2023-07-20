using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Utilities functions for LevelManager */
namespace Qupcakery
{
    public static class GameUtilities
    {
        public static bool gameIsPaused { get; set; } = false;

        // Called to erase all previous data and start game anew, keep previously loaded resources (sprite/prefab)
        public static void CreateNewGame()
        {
            GameManagement.Instance.CreateNewGame();
        }

        public static void PauseGame()
        {
            Time.timeScale = 0;
            gameIsPaused = true;
        }

        public static void UnpauseGame()
        {
            Time.timeScale = 1;
            gameIsPaused = false;
        }

        public static int CalculateLevelResult(float completionRate)
        {
            int starCnt = 0;
            if (completionRate > 0.99f)
                starCnt = 3;
            else if (completionRate > 0.8f)
                starCnt = 2;
            else if (completionRate > 0.6f)
                starCnt = 1;

            return starCnt;
        }

        public static void SaveLevelResult(int levelInd, int starCnt)
        {
            Debug.Log("Saving level result: " + levelInd + " starcnt: "+starCnt);
            string level = "Level" + (levelInd).ToString();

            if (levelInd < GameManagement.Instance.game.gameStat.MaxLevelCompleted)
            {
                int prevCnt = GameManagement.Instance.game.gameStat.GetLevelPerformance(levelInd);
                if (prevCnt < starCnt)
                {
                    GameManagement.Instance.game.gameStat.
                        SetLevelPerformance((int)levelInd, starCnt);
                }
            }
            else
            {
                GameManagement.Instance.game.gameStat.
                        SetLevelPerformance((int)levelInd, starCnt);
            }
            //string dataJson = JsonUtility.ToJson(GameManagement.Instance.game.gameStat);
            //Debug.Log(dataJson);
        }

        // At the end of the level, update total earning tracker
        public static void UpdateTotalEarning(int currentLevelEarning)
        {
            int BaseEarning = GameManagement.Instance.game.gameStat.TotalEarning;
            int newTotalEarning = BaseEarning + currentLevelEarning;

            GameManagement.Instance.game.gameStat.UpdateTotalEarning(newTotalEarning);
        }

        public static int GetLevelResult(int level)
        {
            return GameManagement.Instance.game.gameStat.GetLevelPerformance(level);
        }

        // Check win/lose condition and load corresponding transition scene
        public static void LoadMenuBasedOnEndResult(int starCnt)
        {
            if (starCnt > 0)
            {
                SceneManager.LoadScene("QU_WinMenu");
            }
            else
                SceneManager.LoadScene("QU_LoseMenu");
        }

        public static int GetTotalEarning()
        {
            return GameManagement.Instance.game.gameStat.TotalEarning;
        }

        public static Vector2 GetCustomerStartPosition(int beltIndex, int beltCnt)
        {
            Vector2 startPosition = SetupUtilities.BeltPosition(beltCnt, beltIndex) + new Vector2(12f, 0.05f);
            return new Vector2(startPosition.x, startPosition.y);
        }

        public static Vector2 GetCakeStartPosition(int beltIndex, int beltCnt)
        {
            float beltY = SetupUtilities.BeltPosition(beltCnt, beltIndex).y;
            return new Vector2(-4f, beltY + 0.2f);
        }

    }
}
