using System;
using UnityEngine;
namespace Qupcakery
{
    public class GameModeButton : MonoBehaviour
    {
        public void EnterGameMode()
        {
            GameManagement.Instance.SetGameMode(GameManagement.GameMode.Regular);
            SceneManagementUtilities.LoadLevelSelectionMenu();
        }
    }
}
