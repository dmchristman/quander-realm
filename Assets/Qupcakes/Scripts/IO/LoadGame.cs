using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

/*
 * Loads game data from file/database
 */

namespace Qupcakery
{
    public static class LoadGame
    {
        public static void Load()
        {
            GameUtilities.CreateNewGame();

            try
            {
                string saveString = Wrapper.Events.GetMinigameSaveData?.Invoke(Wrapper.Game.Qupcakes);
                Data.gameData = JsonUtility.FromJson<GameData>(saveString);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }

            if (Data.gameData == null)
                Data.gameData = new GameData();
            else // Update gamestat accordingly
            {
                GameManagement.Instance.game.gameStat.TotalEarning = Data.gameData.TotalEarning;
                GameManagement.Instance.game.gameStat.MaxLevelCompleted = Data.gameData.MaxLevelCompleted;
                // Debug.Log("(data received) Max level completed is : " + saveData.MaxLevelCompleted);

                for (int i = 0; i < Data.gameData.MaxLevelCompleted; i++)
                {
                    int starCnt = (int)Data.gameData.levelPerformance[i];
                    GameManagement.Instance.game.gameStat.SetLevelPerformance((int)i+1, (int)starCnt);
                }
            }

            Data.researchData = new ResearchData();
            Utilities.InitializeTutorialAndHelpMenu();
        }
    }
}
