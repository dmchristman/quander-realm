using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Qupcakery
{
    public class HelpMenuCloseButton : MonoBehaviour
    {
        public GameObject helpPanel;
        public GameObject topBar;
        public GameObject pauseButton;

        public void CloseHelpPanel()
        {
            if (helpPanel != null)
            {
                helpPanel.SetActive(false);

                // Unpause game
                GameUtilities.UnpauseGame();
                pauseButton.GetComponent<PauseButton>().SetPauseSprite();

                // Reactivate top bar
                if (topBar != null)
                    topBar.SetActive(true);
            }
        }
    }
}
