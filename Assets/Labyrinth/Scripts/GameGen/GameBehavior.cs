using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Labyrinth 
{ 
    public class GameBehavior : MonoBehaviour
    {
        private int goalsCollected = 0;
        private int numGoals = 1;
        private bool winner = false;
        public int hintsUsed = 0;
        public int numStars = 3;

        public int degree;
        public int size;
        public double wallProb = 0.5;
        
        public int steps = 0;
        public int pathLength;

        public Star[] stars; 

        private float initTime;
        private float endTime;
        public float timePlayed;

        public Maze maze;
        public PlayerMovement pm;
        // public Camera cam;
        public ButtonBehavior btn;
        public ProgressBar pb;

        public static GameBehavior Instance;

        private Dictionary<string, string> rot180 = new Dictionary<string, string>() 
        { {"N", "S"}, {"W", "E"}, {"E", "W"}, {"S", "N"} };

        private Dictionary<string, string> rot90 = new Dictionary<string, string>() 
        { {"N", "W"}, {"W", "S"}, {"E", "N"}, {"S", "E"} };

        private Dictionary<string, string> rotNEG90 = new Dictionary<string, string>() 
        { {"N", "E"}, {"W", "N"}, {"E", "S"}, {"S", "W"} };

        private Dictionary<string, string> goalTextNormal = new Dictionary<string, string>() 
        { {"N", "Up"}, {"W", "Left"}, {"E", "Right"}, {"S", "Down"} };


    // ~~~~~~~~~~~~~~~ INITIALIZING ~~~~~~~~~~~~~~~

        void Start() 
        {
            degree = SaveData.Instance.Degree;

            Time.timeScale = 1f;
            initTime = Time.time;

            pm = GameObject.Find("Players").GetComponent<PlayerMovement>();
            btn = GameObject.Find("GameManagerLocal").GetComponent<ButtonBehavior>();
            maze = GameObject.Find("MazeGen").GetComponent<Maze>();
            pb = GameObject.Find("StarCountdown").GetComponent<ProgressBar>();
            
            pm.StartPM();
            maze.StartMaze();

            if (SaveData.Instance.CurrentLevel == 0) {
                pathLength = 8;
            }
            else {
                string imagePath = $"Canvases/CanvasUnder/LevelNumbers/Level{SaveData.Instance.CurrentLevel}";
                GameObject.Find(imagePath).SetActive(true);
                pathLength = maze.pathfinder(0, size-1, size-1, 0).Length;
            }
            pb.resestBar();
            pb.initializeBar(pathLength);
        }

        void Update() {
            checkNumStars();
            /* if (winner == true) {
                endTime = Time.time;
                timePlayed = endTime - initTime;
                //SaveSystem.SaveGame(this);
                FindObjectOfType<SaveData>().updateSave(this);

                btn.Win(numStars);
            } */
            /* if (Input.GetKeyDown(KeyCode.Escape)) {
                btn.MainMenu();
            } */
            if (Input.GetKeyDown(KeyCode.Space)) {
                pm.SwitchPlayer();
            }
        }

        public void collectGoal() {
            goalsCollected++;
            checkNumStars();

            if (goalsCollected == numGoals) {
                maze.clearGoal();
                winner = true;

                endTime = Time.time;
                timePlayed = endTime - initTime;
                //SaveSystem.SaveGame(this);
                SaveData.Instance.updateSave(this);

                btn.Win(numStars);
            }
            steps = 0;
        }

        public void checkNumStars() {
            pb.detractBar(steps);
            if (steps > (int)(pathLength + 2)) { // 1 mistake
                stars[2].visibilityOff();
                numStars = 2;
            }
            if (steps > (int)(pathLength + 4)) { // 2 mistakes
                stars[1].visibilityOff();
                numStars = 1;
            }
            if (steps > (int)(pathLength + 8)) { // 4 mistakes
                stars[0].visibilityOff();
                numStars = 0;
            }
            return;
        }

        

    // ~~~~~~~~~~~~~~~ BUTTONS ~~~~~~~~~~~~~~~

        public void GiveHint() {
            string hintDir;

            /* if (hintsUsed == 3) {
                hintText = "You used all your hints!";
                return;
            } */

            object[] hint = maze.calcPathToGoal();
            string hintPath = hint[0].ToString();
            int hintDeg = (int)hint[1];


            if (hintDeg == 90) {
                hintDir = goalTextNormal[rot90[hintPath[0].ToString()]];
                // hintText = $"Hint: Try going {goalTextNormal[rot90[hintPath[0].ToString()]]}!";
            }
            else if (hintDeg == 180) {
                hintDir = goalTextNormal[rot180[hintPath[0].ToString()]];
                // hintText = $"Hint: Try going {goalTextNormal[rot180[hintPath[0].ToString()]]}!";
            }
            else { //if (hintDeg == 0) {
                hintDir = goalTextNormal[hintPath[0].ToString()];
                // hintText = $"Hint: Try going {goalTextNormal[hintPath[0].ToString()]}!";
            }
            
            string buttonPath = $"Canvases/CanvasOver/GameplayButtons/MovementButtons/{hintDir}";
            GameObject.Find(buttonPath).GetComponent<ParticleSystem>().Play();
            // Button button = GameObject.Find(hintDir); //.GetComponent<SpriteRenderer>();
            // button.IsHighlighted = true;

            hintsUsed++;
        }

        /* public void highlightMovButton(Button movButton) {
            movButton.color = new Color((1f,1f,1f,Mathf.SmoothStep(0f, 1f, 1f)));
        } */

        public void Restart() {
            // Saving restart data
            SaveData.Instance.winner = false;
            endTime = Time.time;
            timePlayed = endTime - initTime;
            SaveData.Instance.updateSave(this);
            if (SaveData.Instance.CurrentLevel > 0) {
                // Save.SaveTTSaveData();
                Save.Instance.SaveGame();
            }

            // Resetting level
            initTime = Time.time;

            pm.player1.returnPlayer();
            pm.player2.returnPlayer();

            maze.generateMazes();
            maze.renderMazes();

            if (winner == true) {
                winner = false;
                btn.UndoWin(numStars);
            }

            pathLength = maze.pathfinder(0, size-1, size-1, 0).Length;
            goalsCollected = 0;
            numGoals = 1;
            hintsUsed = 0;
            steps = 0;
            numStars = 3;

            for (int i=0; i<3; i++) {
                stars[i].resetStar();
            }

            if (pm.player1.current == false) {
                pm.SwitchPlayer();
            }

            pb.resestBar();
            Debug.Log(pathLength);
            pb.initializeBar(pathLength);
        }

        public void MoveButton(string mov) {
            Vector3 press = pm.getButtonPress(mov);
        }

        public void NextLevel() {
            SaveData.Instance.CurrentLevel += 1;
            btn.LevelSelect(SaveData.Instance.CurrentLevel);
        }
    }
}