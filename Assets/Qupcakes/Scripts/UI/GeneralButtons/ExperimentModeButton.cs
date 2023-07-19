using UnityEngine;
using System.Collections;
namespace Qupcakery
{
    public class ExperimentModeButton : MonoBehaviour
    {
        public void EnterExperimentMode()
        {
            GameManagement.Instance.SetGameMode(GameManagement.GameMode.Experiment);
            SceneManagementUtilities.LoadLevelSelectionMenu();
        }
    }
}