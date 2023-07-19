using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using BeauRoutine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

namespace Wrapper
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get { return _instance; } }

        private static GameManager _instance;
        private GameObject loadingScreenGO = null;
        private const string introSequenceID = "W_Intro";

        [SerializeField] private float loadingToggleDelay = 0.5f;
        [SerializeField] private DebugScreen debugScreen;
        [SerializeField] private Button debugButton;
        [SerializeField] private SaveManager saveManager;
        [SerializeField] private CardPopup cardPopup;
        [SerializeField] private GameObject loadingScreenPrefab;
        [SerializeField] Button universalBackButton;

        [Header("Reward Card Prefabs")]
        [SerializeField] private GameObject BBRewardPrefab;
        [SerializeField] private GameObject CTRewardPrefab;
        [SerializeField] private GameObject LARewardPrefab;
        [SerializeField] private GameObject QBRewardPrefab;
        [SerializeField] private GameObject QURewardPrefab;

        public readonly string rewardsPath = "_Wrapper/Rewards/RewardAssets/";
        public RewardAsset[] rewardAssets;
        public Dictionary<CardType, Color> colorDict;
        public Dictionary<Game, GameObject> prefabDict;

        [SerializeField, Tooltip("For the first card received in each minigame, if none keep blank")] GameCardDialogPair[] rewardDialogIDs;
        [SerializeField] MinigameTitles minigameTitles;
        Game currentGame = Game.None;

        [System.Serializable]
        struct GameCardDialogPair
        {
            public Game game;
            public string cardDialog;
        }

        #region Unity Functions

        private void Awake()
        {
            InitSingleton();
            InitColorDict();
            InitPrefabDict();
            InitRewardAssetArray();
            //Routine.Start(IntroDialogueRoutine()); //todo: also wait for loadingScreenGO to be null?      -> moved to its own method to call after title screen
            if (debugScreen.DebugEnabled)
            {
                debugButton.gameObject.SetActive(true);
                debugButton.onClick.AddListener(() => debugScreen.gameObject.SetActive(!(debugScreen.gameObject.activeInHierarchy)));
            }
            else debugButton.gameObject.SetActive(false);
            Input.multiTouchEnabled = false;
        }

        private void Start()
        {
            BackToMain();
        }

        private void OnEnable()
        {
            Events.OpenMinigame += OpenMinigame;
            Events.CreatRewardCard += CreateCard;
            if (debugScreen.DebugEnabled) Events.ShowCardPopup += ShowCardPopup; // Debug
            Events.ToggleLoadingScreen += ToggleLoadingScreen;
            Events.CollectAndDisplayReward += CollectAndDisplayReward;
            Events.ToggleBackButton += ToggleBackButton;
            Events.Logout += Logout;
            Events.PlayIntroDialog += PlayIntroDialog;
            Events.MinigameClosed += BackToMain;
            Events.GetMinigameTitle += GetGameTitle;
            Events.GetCurrentGame += GetCurrentGame;
            Events.IsDebugEnabled += () => debugScreen.DebugEnabled;
        }

        private void OnDisable()
        {
            Events.OpenMinigame -= OpenMinigame;
            Events.CreatRewardCard -= CreateCard;
            if (debugScreen.DebugEnabled) Events.ShowCardPopup -= ShowCardPopup; // Debug
            Events.ToggleLoadingScreen -= ToggleLoadingScreen;
            Events.CollectAndDisplayReward -= CollectAndDisplayReward;
            Events.ToggleBackButton -= ToggleBackButton;
            Events.Logout -= Logout;
            Events.PlayIntroDialog -= PlayIntroDialog;
            Events.MinigameClosed -= BackToMain;
            Events.GetMinigameTitle -= GetGameTitle;
            Events.GetCurrentGame -= GetCurrentGame;
            Events.IsDebugEnabled -= () => debugScreen.DebugEnabled;
        }

        #endregion

        private void OpenMinigame(Minigame minigame)
        {
            SceneManager.LoadScene(minigame.StartScene);
            currentGame = minigame.gameValue;
        }

        void BackToMain()
        {
            if (!saveManager.isUserLoggedIn) Events.OpenLoginScreen?.Invoke();
            else
            {
                Events.CloseLoginScreen?.Invoke();
                Events.ToggleTitleScreen?.Invoke(false);
            }
            Events.PlayMusic?.Invoke("W_Music");
            currentGame = Game.None;
        }

        private void ToggleLoadingScreen()
        {
            if (loadingScreenGO == null)
                loadingScreenGO = Instantiate(loadingScreenPrefab);
            else
                Routine.Start(DestroyLoadingScreen()); // todo: debug, delete later
        }

        private void CollectAndDisplayReward(Game game, int level)
        {
            RewardAsset levelReward = Array.Find(rewardAssets, (reward) => reward.game == game && reward.level == level);
            if (levelReward == null)
            {
                Debug.LogFormat("No Reward found for {0} level {1}", game.ToString(), level);
                return;
            }

            bool rewardAdded = Events.AddReward?.Invoke(levelReward.rewardID) ?? false;

            if (rewardAdded)
            {
                Routine.Start(cardPopup.DisplayCard(CreateCard(levelReward.rewardID, cardPopup.GetContainerMount(), DisplayType.CardPopup)));

                // if this is the first reward from this game, display the reward dialog 
                if (Events.GetFirstRewardBool(levelReward.rewardID.Substring(0, 2).ToLower()))
                    Events.StartDialogueSequence?.Invoke(rewardDialogIDs[(int)game].cardDialog);
            }
            ////todo: call a function that creates the card and displays it in the reward card panel
            //Debug.LogFormat("Won Reward {0} in game {1} at level {2}", levelReward.rewardID, game, level);
            //    Routine.Start(DestroyLoadingScreen()); //todo: debug, delete later
        }

        private GameObject CreateCard(string rewardID, GameObject mount, DisplayType displayType)
        {
            RewardAsset rAsset = Resources.Load<RewardAsset>(Path.Combine(rewardsPath, rewardID));
            if (rAsset == null)
            {
                Debug.LogErrorFormat("Could not find card {0} to feature", rewardID);
                return null;
            }

            ClearChildren(mount);
            GameObject rewardGO = Instantiate(prefabDict[rAsset.game], mount.transform);
            rewardGO.GetComponent<Reward>().SetContent(rAsset, colorDict[rAsset.cardType], displayType);

            return rewardGO;
        }

        private GameObject CreateCard(RewardAsset rAsset, GameObject mount, DisplayType displayType)
        {
            GameObject rewardGO = Instantiate(prefabDict[rAsset.game], mount.transform);
            rewardGO.GetComponent<Reward>().SetContent(rAsset, colorDict[rAsset.cardType], displayType);

            return rewardGO;
        }

        void ToggleBackButton(bool show)
        {
            universalBackButton.gameObject.SetActive(show);
        }

        void Logout()
        {
            saveManager.Logout();
            BackToMain();
        }

        #region Helpers

        private void InitSingleton()
        {
            DontDestroyOnLoad(this);

            if (_instance != null && _instance != this)
                Destroy(this.gameObject);
            else
                _instance = this;
        }

        private void InitRewardAssetArray()
        {
            rewardAssets = Resources.LoadAll<RewardAsset>(rewardsPath);
        }

        private void InitColorDict()
        {
            ColorUtility.TryParseHtmlString("#89d7ff", out Color visualColor);
            ColorUtility.TryParseHtmlString("#ffe698", out Color charColor);
            ColorUtility.TryParseHtmlString("#ff8062", out Color conceptColor);
            ColorUtility.TryParseHtmlString("#97fb9b", out Color compPartColor);

            colorDict = new Dictionary<CardType, Color>
            {
                { CardType.Visual, visualColor },
                { CardType.Character, charColor },
                { CardType.Concept, conceptColor },
                { CardType.Computer_Part, compPartColor }
            };
        }

        private void InitPrefabDict()
        {
            prefabDict = new Dictionary<Game, GameObject>
            {
                { Game.BlackBox,  BBRewardPrefab },
                { Game.Circuits,  CTRewardPrefab },
                { Game.Labyrinth, LARewardPrefab },
                { Game.QueueBits, QBRewardPrefab },
                { Game.Qupcakes,  QURewardPrefab }
            };
        }

        void PlayIntroDialog()
        {
            Routine.Start(IntroDialogueRoutine());
        }

        private IEnumerator IntroDialogueRoutine()
        {
            while (!(saveManager.isUserLoggedIn) || !(saveManager.currentUserSave != null))
                yield return null;

            if (saveManager.HasPlayerSeenIntroDialogue())
                yield break;

            Events.StartDialogueSequence?.Invoke(introSequenceID);
            saveManager.ToggleIntroDialogueSeen(true);
            Events.SetNewPlayerStatus?.Invoke(false);
        }

        // todo: debug, delete later
        private IEnumerator DestroyLoadingScreen()
        {
            yield return new WaitForSeconds(loadingToggleDelay);
            Destroy(loadingScreenGO);
            loadingScreenGO = null;
        }

        private void ShowCardPopup(string rewardID)
        {
            RewardAsset rAsset = Resources.Load<RewardAsset>(Path.Combine(rewardsPath, rewardID));
            if (rAsset == null)
            {
                Debug.LogFormat("Could not find card {0} to display", rewardID);
                return;
            }

            Routine.Start(cardPopup.DisplayCard(CreateCard(rewardID, cardPopup.GetContainerMount(), DisplayType.CardPopup)));
        }

        //todo: Utils class?
        private void ClearChildren(GameObject mount)
        {
            foreach (Transform child in mount.transform)
                Destroy(child.gameObject);
        }

        string GetGameTitle(Game game)
        {
            return minigameTitles.Entries[(int)game];
        }

        Game GetCurrentGame()
        {
            return currentGame;
        }

        #endregion
    }
}