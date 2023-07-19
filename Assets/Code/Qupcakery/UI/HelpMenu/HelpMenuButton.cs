using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Qupcakery
{
    public class HelpMenuButton : MonoBehaviour
    {
        public GameObject helpPanel;
        public GameObject recipePanel, startPanel;
        public GameObject topBar;

        public void OpenPanel()
        {
            if (GameObject.FindGameObjectsWithTag("InfoPanel").Length > 0)
                return;

            if (helpPanel != null)
            {
                recipePanel.SetActive(false);
                helpPanel.SetActive(true);
                startPanel.SetActive(true);

                // pause game
                GameUtilities.PauseGame();

                // Deactivate top bar
                topBar.SetActive(false);
            }
        }
    }
}
