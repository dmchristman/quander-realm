using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Wrapper
{
    public static class Events
    {
        /// Game Management ///
        public static Action<Minigame> OpenMinigame;
        public static Action MinigameClosed;
        public static Action ToggleLoadingScreen;
        public static Action OpenLoginScreen;
        public static Action CloseLoginScreen;
        public static Action<bool> ToggleBackButton;
        public static Action<bool> ToggleTitleScreen;
        public static Action PlayIntroDialog;
        public static Func<Game> GetCurrentGame;
        public static Func<Game, string> GetMinigameTitle;

        /// Screen Fading ///
        public static Action<Action, float> ScreenFadeMidAction;
        public static Action StopScreenFade;

        /// Save System ///
        public static Action<string> SubmitResearchCode;
        public static Action<LoginStatus> UpdateLoginStatus;
        public static Action<Game, object> UpdateMinigameSaveData;
        public static Func<Game, string> GetMinigameSaveData;
        public static Action UpdateRemoteSave;
        public static Action ClearSaveFile;
        public static Action<bool> ToggleUploadFailurePopup;
        public static Func<string> GetPlayerResearchCode;
        public static Action<Game, object> SaveMinigameResearchData;
        public static Action<bool> SetNewPlayerStatus;
        public static Action Logout;

        /// Dialogue System ///
        public static Action<string> PrintDialogue;
        public static Action SortSequences;
        public static Action<string> StartDialogueSequence;
        public static Action<Dialogue> OpenDialogueView;
        public static Action<Dialogue, int> UpdateDialogueView;
        public static Action CloseDialogueView;
        public static Action DialogueSequenceEnded;
        public static Action<int> ChangeDialogue;
        public static Action<bool> SwitchNextButton;
        public static Action<bool> TogglePreviousButton;
        public static Action EnableSkipButton;

        /// Reward System ///
        public static Func<string, bool> AddReward;
        public static Func<string, bool> IsRewardUnlocked;
        public static Action<JournalPage> OpenJournalPage;
        public static Action<string> FeatureCard;
        public static Action ResetPageNumbers;
        public static Func<int, Toggle> GetNavDot;
        public static Action<JournalPage> SwitchPage;
        public static Action UnselectAllCards;
        public static Action<JournalPage> UpdateTab;
        public static Action<Game, int> CollectAndDisplayReward;
        public static Func<RewardAsset, GameObject, DisplayType, GameObject> CreatRewardCard;
        public static Action<bool> SetRewardTextSeen;
        public static Func<(bool, bool)> GetRewardDialogStats;
        public static Func<string, bool> GetFirstRewardBool;

        /// Audio System ///
        public static Action<string> PlayMusic;
        public static Action StopMusic;
        public static Action<string> PlaySound;

        /// Debug ///
        public static Action<string> BBGotoLevel;
        public static Action BBToggleDebug;
        public static Action BBClearMarkers;
        public static Action<string> ShowCardPopup;
        public static Func<bool> IsDebugEnabled;
    }
}
