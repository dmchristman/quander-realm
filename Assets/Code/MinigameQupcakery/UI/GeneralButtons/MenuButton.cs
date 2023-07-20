using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Qupcakery
{
    public class MenuButton : MonoBehaviour
    {
        public void LoadLevelSelectionMenu()
        {
            GameManagement.Instance.game.gameStat.SetLevelResultAndSave(GameStat.LevelResult.QUIT);

            if (GameObject.FindGameObjectsWithTag("InfoPanel").Length > 0)
                return;

            GameObjectsManagement.ResetAllGameObjects();
            GameObjectsManagement.DeactiveAllGameObjects();

            GameUtilities.UnpauseGame();
            SceneManagementUtilities.LoadLevelSelectionMenu();
        }
    }
}
