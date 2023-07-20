using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wrapper;

namespace BlackBox
{
    public class TutorialsPanelView : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private QButton showButton;
        [SerializeField] private QButton hideButton;
        [SerializeField] private QButton[] tutorialButtons;

        private const string tutorialSequenceID = "BB_Tutorial_";

        void Awake()
        {
            showButton.onClick.AddListener(() => animator.SetBool("Active", true));
            hideButton.onClick.AddListener(() => animator.SetBool("Active", false));
            InitTutorialButtons();
        }

        private void OnEnable()
        {
            BBEvents.ShowTutorial += ShowTutorial;
        }

        private void OnDisable()
        {
            BBEvents.ShowTutorial -= ShowTutorial;
        }

        private void InitTutorialButtons()
        {
            foreach (QButton button in tutorialButtons)
            {
                button.onClick.AddListener(() =>
                {
                    Events.StartDialogueSequence?.Invoke(
                        tutorialSequenceID + Array.IndexOf(tutorialButtons, button).ToString());
                });
            }
        }

        private void ShowTutorial(BBSaveData saveData, Level level)
        {
            try
            {
                bool tutorialSeen = saveData.tutorialsSeen[level.tutorialNumber];
            }
            catch (Exception)
            {
                BBSaveData data = new BBSaveData
                {
                    gameID = saveData.gameID,
                    currentLevelID = saveData.currentLevelID
                };

                saveData = data;
                Events.UpdateMinigameSaveData?.Invoke(Game.BlackBox, saveData);
            }
            finally
            {
                bool tutorialSeen = saveData.tutorialsSeen[level.tutorialNumber];
                if (!(tutorialSeen))
                {
                    Events.StartDialogueSequence?.Invoke(tutorialSequenceID + level.tutorialNumber.ToString());
                    saveData.tutorialsSeen[level.tutorialNumber] = true;
                }

                ToggleTutorialButtons(saveData);
            }
        }

        private void ToggleTutorialButtons(BBSaveData saveData)
        {
            for (int i = 0; i < saveData.tutorialsSeen.Length; i++)
            {
                bool tutorialSeen = saveData.tutorialsSeen[i];
                tutorialButtons[i].gameObject.SetActive(tutorialSeen);
            }
        }
    }
}
