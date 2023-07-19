using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackBox
{
    public class EndPanelView : MonoBehaviour
    {
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI header = null;
        [SerializeField] private TextMeshProUGUI subHeader = null;

        [Header("Image GameObjects")]
        [SerializeField] private GameObject winImage = null;
        [SerializeField] private GameObject notYetImage = null;
        [SerializeField] private GameObject loseImage = null;

        [Header("Button GameObjects")]
        [SerializeField] private GameObject restartLevelGO = null;
        [SerializeField] private GameObject quitGO = null;
        [SerializeField] private GameObject keepPlayingGO = null;
        [SerializeField] private GameObject nextLevelGO = null;

        private Animator animator = null;
        private int level;

        private void Awake()
        {
            SetupButtonFunctionality();
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            BBEvents.UpdateEndPanel += UpdatePanel;
        }

        private void OnDisable()
        {
            BBEvents.UpdateEndPanel -= UpdatePanel;
        }

        #region Buttons

        private void SetupButtonFunctionality()
        {
            restartLevelGO.GetComponent<Button>().onClick.AddListener(RestartLevel);
            quitGO.GetComponent<Button>().onClick.AddListener(ToLevelSelect);
            keepPlayingGO.GetComponent<Button>().onClick.AddListener(() => TogglePanel(false));
            nextLevelGO.GetComponent<Button>().onClick.AddListener(StartNextLevel);
        }

        private void RestartLevel()
        {
            TogglePanel(false);
            BBEvents.RestartLevel?.Invoke();
        }

        void ToLevelSelect()
        {
            TogglePanel(false);
            BBEvents.CloseLevel?.Invoke();
        }

        private void QuitBlackBox()
        {
            TogglePanel(false);
            BBEvents.QuitBlackBox?.Invoke();
        }

        private void StartNextLevel()
        {
            TogglePanel(false);
            BBEvents.StartNextLevel?.Invoke();
        }

        #endregion

        #region Update Panel

        private void UpdatePanel(WinState winState)
        {
            DisableAllElements();
            TogglePanel(true);

            animator.SetInteger("Lives", winState.livesRemaining);

            if (winState.levelWon)
            {
                level = winState.level;
                SetInfo(
                    winImage,
                    headerText: "WE DID IT!",
                    subHeaderText: "We found all of the items!",
                    winState.end ? new GameObject[] {quitGO} :
                    new GameObject[] { quitGO, nextLevelGO });
                if (winState.end) BBEvents.CompleteBlackBox?.Invoke();
            }

            else
            {
                if (winState.livesRemaining > 0)
                {
                    SetInfo(
                        notYetImage,
                        headerText: "NOT QUITE...",
                        subHeaderText: "We found " + winState.numCorrect + " out of " + winState.numNodes + " items",
                        new GameObject[] { quitGO, keepPlayingGO });
                }
                else
                {
                    SetInfo(
                        loseImage, 
                        headerText: "GAME OVER", 
                        subHeaderText: "You ran out of lives!",
                        new GameObject[] { quitGO, restartLevelGO }); 
                }
            }
        }

        private void SetInfo(GameObject imageGO, string headerText, string subHeaderText, GameObject[] buttons)
        {
            imageGO.SetActive(true);
            header.text = headerText;
            subHeader.text = subHeaderText;

            foreach (GameObject buttonGO in buttons)
                buttonGO.SetActive(true);
        }

        private void DisableAllElements()
        {
            restartLevelGO.SetActive(false);
            quitGO.SetActive(false);
            keepPlayingGO.SetActive(false);
            nextLevelGO.SetActive(false);

            winImage.SetActive(false);
            notYetImage.SetActive(false);
            loseImage.SetActive(false);
        }

        private void TogglePanel(bool isOn)
        {
            animator.SetBool("On", isOn);
        }

        #endregion
    }
}
