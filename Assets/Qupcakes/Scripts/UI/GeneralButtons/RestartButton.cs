using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Restart level during gameplay

namespace Qupcakery
{
    public class RestartButton : MonoBehaviour
    {
        public void RestartGame()
        {
            GameManagement.Instance.game.gameStat.SetLevelResultAndSave(GameStat.LevelResult.QUIT);

            if (GameObject.FindGameObjectsWithTag("InfoPanel").Length > 0)
                return;

            if (GameUtilities.gameIsPaused) GameUtilities.UnpauseGame();

            switch (GameManagement.Instance.gameMode)
            {
                case GameManagement.GameMode.Regular:
                    GameObjectsManagement.ResetAllGameObjects();
                    SceneManagementUtilities.LoadGameScene();
                    break;
                //case GameManagement.GameMode.Experiment:
                //    SceneManagementUtilities.LoadExperimentMode();
                //    break;
            }
        }
    }
}
