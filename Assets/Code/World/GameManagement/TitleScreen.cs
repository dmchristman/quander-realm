using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wrapper
{
    public class TitleScreen : MonoBehaviour
    {
        [Header("Main Components"), SerializeField]
        GameObject mainContainer;
        [SerializeField]
        Button playButton;
        [SerializeField]
        Button continueButton;
        [SerializeField]
        Button newButton;
        [SerializeField]
        Button logoutButton;
        [SerializeField]
        ConfirmationPopup newGameConfirm;
        [SerializeField]
        LoadingPopup loadingPanel;

        [Space, SerializeField]
        Animator titleAnim;
        [SerializeField]
        Animator mapAnim;

        [Space, SerializeField]
        Button creditsButton;
        [SerializeField]
        CreditsFiller creditsPanel;
        [SerializeField]
        Credits creditsData;

        [Space, SerializeField]
        Button learnMoreButton;
        [SerializeField]
        ConfirmationPopup learnMoreConfirm;
        [SerializeField, Header("More Info URL")]
        string moreInfoURL = "https://www.epiqc.cs.uchicago.edu/zines";

        bool newPlayer = true;

        private void Awake()
        {
            // events
            Events.SetNewPlayerStatus += SetNewPlayer;
            Events.ToggleTitleScreen += ToggleTitleScreen;
            SetNewPlayer(false);

            // buttons
            playButton.onClick.AddListener(PlayGame);
            continueButton.onClick.AddListener(PlayGame);
            newButton.onClick.AddListener(OpenNewConfirm);
            logoutButton.onClick.AddListener(LogOut);
            creditsButton.onClick.AddListener(OpenCredits);
            learnMoreButton.onClick.AddListener(OpenLearnConfirm);

            ToggleTitleScreen(false);
        }

        private void OnDestroy()
        {
            // events
            Events.SetNewPlayerStatus -= SetNewPlayer;
            Events.ToggleTitleScreen -= ToggleTitleScreen;

            // buttons
            playButton.onClick.RemoveListener(PlayGame);
            continueButton.onClick.RemoveListener(PlayGame);
            newButton.onClick.RemoveListener(OpenNewConfirm);
            logoutButton.onClick.RemoveListener(LogOut);
            creditsButton.onClick.RemoveListener(OpenCredits);
            learnMoreButton.onClick.RemoveListener(OpenLearnConfirm);
        }

        void ToggleTitleScreen(bool enable)
        {
            mainContainer.SetActive(true);
            creditsPanel.gameObject.SetActive(false);
            newGameConfirm.ForceCloseConfirmation();
            learnMoreConfirm.ForceCloseConfirmation();
            loadingPanel.ForceCloseConfirmation();
            mapAnim.SetTrigger(enable ? "Map_Fly_Out" : "MapFly_In");
            titleAnim.SetTrigger(enable ? "MainMenu_Fade_In" : "MainMenu_Fade_Out");
            if (!enable) BeauRoutine.Routine.Start(DelayTitleClose());
        }

        IEnumerator DelayTitleClose()
        {
            yield return 0.5F;
            mainContainer.SetActive(false);
        }

        void SetNewPlayer(bool isNew)
        {
            newPlayer = isNew;

            if (newPlayer)
            {
                playButton.gameObject.SetActive(true);
                continueButton.gameObject.SetActive(false);
                newButton.gameObject.SetActive(false);
            }
            else
            {
                playButton.gameObject.SetActive(false);
                continueButton.gameObject.SetActive(true);
                newButton.gameObject.SetActive(true);
            }
            // logout button is always active
        }

        void OpenNewConfirm()
        {
            newGameConfirm.SetConfirmationData(NewGame, null);
            newGameConfirm.OpenConfirmation();
        }

        void NewGame()
        {
            // clear data and show loading screen for a bit, then refresh title screen
            loadingPanel.OpenPopup();
            Events.ClearSaveFile?.Invoke();

            // loading screen will cover this when completed
            //Events.SetNewPlayerStatus?.Invoke(true);
        }

        void PlayGame()
        {
            // same as continue with new save data
            Events.ToggleTitleScreen?.Invoke(false);
            Events.PlayIntroDialog?.Invoke();
        }

        void LogOut()
        {
            Events.Logout?.Invoke();
        }

        void OpenCredits()
        {
            Events.ScreenFadeMidAction?.Invoke(() =>
            {
                creditsPanel.LoadCredits(creditsData);
                creditsPanel.gameObject.SetActive(true);
            }, 0.1F);
        }

        void OpenLearnConfirm()
        {
            learnMoreConfirm.SetConfirmationData(OpenLearnMore, null);
            learnMoreConfirm.OpenConfirmation();
        }

        void OpenLearnMore()
        {
            Debug.Log("OPEN URL: " + moreInfoURL);
            Application.OpenURL(moreInfoURL);
        }
    }
}