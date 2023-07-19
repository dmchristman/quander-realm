using UnityEngine;
using System.Collections;

namespace Qupcakery
{
    public class ChangeGameModeButton : MonoBehaviour
    {
        public void GoToChangeMadeScene()
        {
            SceneManagementUtilities.LoadModeSelectionScene();
        }
    }
}
