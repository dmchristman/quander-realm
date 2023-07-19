using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Qupcakery
{
    public class ReplayButton : MonoBehaviour
    {
        public void Replay()
        {
            // If help/tutorial panel is active, do nothing
            if (GameObject.FindGameObjectsWithTag("InfoPanel").Length > 0)
                return;
            GameObjectsManagement.ResetAllGameObjects();
            SceneManagementUtilities.LoadGameScene();
        }
    }
}
