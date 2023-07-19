using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Runtime.InteropServices;

namespace Labyrinth 
{ 
    public class Save : MonoBehaviour
    {

        public static Save Instance;

        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public int SaveGame() {
            // Save research data
            Data.Instance.researchData.UpdateResearchData(SaveData.Instance);
            Wrapper.Events.SaveMinigameResearchData?.Invoke(Wrapper.Game.Labyrinth, Data.Instance.researchData);

            // Save game data
            Data.Instance.ttSaveData.UpdateTTSaveData(SaveData.Instance);
            Wrapper.Events.UpdateMinigameSaveData?.Invoke(Wrapper.Game.Labyrinth, Data.Instance.ttSaveData);

            return 0;
        }

        /* [DllImport("__Internal")]
        
        private static extern void TwinTanglementSave(string data);

        public static void SaveGame()
        {
            // Data data = new Data(SaveData.Instance.starsPerLevel, SaveData.Instance.timePerLevel, SaveData.Instance.previousSave);
            Data data = new Data(SaveData.Instance);
            string jsonData = JsonUtility.ToJson(data);
            Debug.Log(jsonData);
            
            // call when want to save data
            #if UNITY_WEBGL == true && UNITY_EDITOR == false
                TwinTanglementSave(jsonData);
            #endif
            
            return;
        } */

        /* public static void SaveTTSaveData() {
            // FILL IN RESEARCH DATA LATER

            Load.LoadTTSaveData();
            Load.saveData.updateTTSaveData(SaveData.Instance);
            Debug.Log(Load.saveData.TTSDtoString());
            // Wrapper.Events.UpdateMinigameSaveData?.Invoke(Game.Labyrinth, TTSaveData);
            Wrapper.Events.UpdateMinigameSaveData?.Invoke(Wrapper.Game.Labyrinth, Load.saveData);

        } */
        
    }

}