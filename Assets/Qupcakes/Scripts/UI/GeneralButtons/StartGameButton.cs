using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Qupcakery
{
    public class StartGameButton : MonoBehaviour
    {
        public void StartGame()
        {
            if (!GameManagement.Instance.GameInProgress())
            {
                LoadGame.Load();
            }

            SceneManagementUtilities.LoadModeSelectionScene();
        }
    }
}
