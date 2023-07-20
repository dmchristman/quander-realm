using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Labyrinth 
{ 
    public class ButtonBehavior : MonoBehaviour
    {
        public GameObject winScreen;
        public GameObject gameplayButtons;
        public GameObject gameplayObjects;
        public GameObject progressBar;
        public GameObject[] starsWon;
        public GameObject litePanel;

        public Button[] buttons;

        void Start() {
            if (SceneManager.GetActiveScene().name == "LA_MainMenu") {
                Load.LoadGame();
                DialogueAndRewards.Instance.updateDialogueDict();
            }
            if ((SceneManager.GetActiveScene().name == "LA_MainMenu") 
            && (DialogueAndRewards.Instance.levelDialogue[-1] == false)) {
                Wrapper.Events.StartDialogueSequence?.Invoke("LA_Intro");
                DialogueAndRewards.Instance.levelDialogue[-1] = true;
            }

            if (buttons == null) {
                return;
            }
            /* else if (buttons.Length > 0) {
                for (int i=0; i<15; i++) {
                    if (i < FindObjectOfType<SaveData>().levelUnlocked) {
                        buttons[i].interactable = true;
                    }
                    else {
                        buttons[i].interactable = false;
                    }
                }
            } */
            else if (buttons.Length > 0) {
                for (int i=1; i<=15; i++) {
                    GameObject.Find($"StarMessage{i}").GetComponent<StarMessage>().displayStars();
                }
            }
#if LITE_VERSION
    if (litePanel != null) {
        litePanel.SetActive(true);
    }
#endif
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                LoadMainMenu();
                // LoadLevelSelectMenu();
            }
        }

        public void LoadLevelSelectMenu() {
            Time.timeScale = 1f;
            // Load.LoadGame();
            // DialogueAndRewards.Instance.updateDialogueDict();

            /* if (DialogueAndRewards.Instance.levelDialogue[0] == false) {
                LevelSelect(0);
            }
            else {
                SceneManager.LoadScene("LA_LevelSelect");
            } */
            SceneManager.LoadScene("LA_LevelSelect");
        }

        /* public void InitialLoadLevelSelectMenu() {
            Time.timeScale = 1f;
            Load.LoadGame();
            SceneManager.LoadScene("LevelSelect");
        } */

        public void LoadMainMenu() {
            Time.timeScale = 1f;
            // Load.LoadTTSaveData();
            Load.LoadGame();
            SceneManager.LoadScene("LA_MainMenu");
        }

        public void Exit() {
            // Save.SaveGame();
            
            Application.Quit();
        }

        public void Win(int goalsCollected) {      
            SaveData.Instance.winner = true;
            Save.Instance.SaveGame();
            /* if (SaveData.Instance.CurrentLevel > 0) {
                // Save.SaveTTSaveData();
                Save.Instance.SaveGame();
            } */
    
            winScreen.SetActive(true);
            gameplayButtons.SetActive(false);
            gameplayObjects.SetActive(false);
            progressBar.SetActive(false);

            starsWon[goalsCollected].SetActive(true);
            // Time.timeScale = 0f;

            if ((SaveData.Instance.CurrentLevel % 5 == 0) 
            && (SaveData.Instance.CurrentLevel != 0)) {
                DialogueAndRewards.Instance.doDialogue(SaveData.Instance.CurrentLevel);
            }
            DialogueAndRewards.Instance.giveReward(SaveData.Instance.CurrentLevel);
            // DialogueAndRewards.Instance.updateDialogueDict();
        }

        public void UndoWin(int goalsCollected) {
            SaveData.Instance.winner = false;

            winScreen.SetActive(false);
            gameplayButtons.SetActive(true);
            gameplayObjects.SetActive(true);
            progressBar.SetActive(true);

            starsWon[goalsCollected].SetActive(false);
            Time.timeScale = 1f;
        }

        public void LevelSelect(int sel) {
            string currScene;
            SaveData.Instance.CurrentLevel = sel;

            switch(sel) {
                /* case 0:
                    DialogueAndRewards.Instance.doDialogue(sel);
                    SaveData.Instance.Degree = 0;
                    currScene = "LA_Tutorial";
                    break; */
                case < 3:
                    DialogueAndRewards.Instance.doDialogue(sel);
                    SaveData.Instance.Degree = 0;
                    currScene = "LA_4x4";
                    break;
                case < 5:
                    SaveData.Instance.Degree = 0;
                    currScene = "LA_5x5";
                    break;
                case 5:
                    SaveData.Instance.Degree = 0;
                    currScene = "LA_6x6";
                    break;


                case < 8:
                    DialogueAndRewards.Instance.doDialogue(sel);
                    SaveData.Instance.Degree = 180;
                    currScene = "LA_4x4";
                    break;
                case < 10:
                    SaveData.Instance.Degree = 180;
                    currScene = "LA_5x5";
                    break;
                case 10:
                    SaveData.Instance.Degree = 180;
                    currScene = "LA_6x6";
                    break;


                case < 13:
                    DialogueAndRewards.Instance.doDialogue(sel);
                    SaveData.Instance.Degree = 90;
                    currScene = "LA_4x4";
                    break;
                case < 15:
                    SaveData.Instance.Degree = 90;
                    currScene = "LA_5x5";
                    break;
                case 15:
                    SaveData.Instance.Degree = 90;
                    currScene = "LA_6x6";
                    break;
                

                default:
                    currScene = "LA_LevelSelect";
                    break;
            }
            SceneManager.LoadScene(currScene);
        }

        /* public void doDialogue(int level) {
            if (SaveData.Instance.levelDialogue.ContainsKey(level) &&
            SaveData.Instance.levelDialogue[level] == true) {
                Wrapper.Events.StartDialogueSequence?.Invoke("LA_Level"+level);
                SaveData.Instance.levelDialogue[level] = false;
            }
        }

        public void giveReward(int level) {
            if (Events.IsRewardUnlocked?.Invoke(levelRewards[level]) == false) {
                Wrapper.Events.CollectAndDisplayReward?.Invoke(Wrapper.Game.Labyrinth, level);
            }
        } */

        // ~~~~~~~~~~~~~~~ Enabled in Filament Environment ~~~~~~~~~~~~~~~

        private void OnEnable() {
            Wrapper.Events.MinigameClosed += DestroyDataObject;
        }
        private void OnDisable(){
            Wrapper.Events.MinigameClosed -= DestroyDataObject;
        }
        private void DestroyDataObject() {
            Time.timeScale = 1f;
            Destroy(GameObject.Find("ProfileData"));
        }

    }
}