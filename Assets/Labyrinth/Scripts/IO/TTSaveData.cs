using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

// Firebase non-research load/save data

namespace Labyrinth 
{ 
    [System.Serializable]
    public class TTSaveData {

        public Wrapper.Game gameID = Wrapper.Game.Labyrinth;
        // public string currentLevelID = string.Empty;

        public int[] MaxStarsPerLevel = new int[15];
        public bool[] DialogueSeen = new bool[7];

        public void UpdateTTSaveData(SaveData save) {
            MaxStarsPerLevel = save.starsPerLevel;
            DialogueSeen = save.dialogueSeen;
            Debug.Log("TTSD: " + TTSDtoString());
        }

        public string TTSDtoString() {
            string jsonData = JsonUtility.ToJson(this);
            return jsonData;
        }

        /* public int[] MaxStarsPerLevel;
        public int Level;
        public int NumStars;
        public float Time;
        public int HintsUsed;
        public bool Winner;
        
        public void updateTTSaveData(SaveData save) {
            MaxStarsPerLevel = save.starsPerLevel;
            // TimePlayed = save.timePerLevel;
            //PreviousSave = save.previousSave;
            Level = save.level;
            NumStars = save.numStars;
            Time = save.time;
            HintsUsed = save.hintsUsed;
            Winner = save.winner;
        } */

        
    }
}