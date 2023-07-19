using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Qupcakery
{
    public class PauseButton : MonoBehaviour
    {
        public Sprite pauseSprite;
        public Sprite unpauseSprite;

        Image image;

        private void Awake()
        {
            image = GetComponent<Image>();

            if (GameUtilities.gameIsPaused)
            {
                SetUnpauseSprite();
            }
        }

        public void PauseGame()
        {
            if (GameObject.FindGameObjectsWithTag("InfoPanel").Length > 0)
                return;

            if (GameUtilities.gameIsPaused)
            {
                GameUtilities.UnpauseGame();
                SetPauseSprite();
                //Utilities.DeactiveGamePanel();
            }
            else
            {
                GameUtilities.PauseGame();
                SetUnpauseSprite();
                //Utilities.ActiveGamePanel();
            }
        }

        public void SetPauseSprite()
        {
            image.sprite = pauseSprite;
        }

        public void SetUnpauseSprite()
        {
            image.sprite = unpauseSprite;
        }
    }
}
