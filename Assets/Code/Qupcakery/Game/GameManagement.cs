using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SysDebug = System.Diagnostics.Debug;

/*
 * Manages static game instance to persist across scenes 
 */
namespace Qupcakery
{
    public class GameManagement : MonoBehaviour
    {
        public static GameManagement Instance;

        public enum GameMode
        {
            Regular, Experiment
        }

        public GameObject customerPrefab, cakePrefab, buttonPrefab, panelPrefab;

        public GameMode gameMode { get; private set; }
        public bool GameIsPaused { get; private set; } = false;

        public Game game { get; set; }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            if (Instance == null)
            {
                Instance = this;

                GameObjectsManagement.CreateGameObjects(customerPrefab, cakePrefab,
        buttonPrefab, panelPrefab);

                /* Load game from database */
                LoadGame.Load();
            }
            else
                Destroy(gameObject);
        }

        private void OnEnable()
        {
            Wrapper.Events.MinigameClosed += GameObjectsManagement.DeleteGameObjects;
            Wrapper.Events.MinigameClosed += OnExitGame;
        }

        private void OnExitGame()
        {
            Wrapper.Events.MinigameClosed -= GameObjectsManagement.DeleteGameObjects;
            Wrapper.Events.MinigameClosed -= OnExitGame;
            Object.Destroy(gameObject);
        }

        public void CreateNewGame()
        {
            game = new Game();
            // Set up tutorial
            TutorialManager.UpdateAvailability();
        }

        // A game instance has been created
        public bool GameInProgress()
        {
            return (game != null);
        }

        // Returns the level object for current level
        public Level GetCurrentLevel()
        {
            return game.GetCurrLevel();
        }


        // Returns the level ind for current level
        public int GetCurrentLevelInd()
        {
            return game.CurrLevelInd;
        }

        public void SetCurrentLevel(int currentLevelInd)
        {
            SysDebug.Assert(game != null);

            game.UpdateLevel(currentLevelInd);
        }

        public int GetTotalLevelCnt()
        {
            SysDebug.Assert(game != null);

            return game.MaxLevelCnt;
        }

        public void SetGameMode(GameMode gm)
        {
            gameMode = gm;
        }
    }
}
