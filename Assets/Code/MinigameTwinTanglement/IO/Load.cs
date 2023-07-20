using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.InteropServices;


namespace Labyrinth 
{ 
    public class Load : MonoBehaviour
    {
        // public static TTSaveData saveData;

        public static void LoadGame() {
        // public static void LoadTTSaveData() {

            try {
                string saveString = Wrapper.Events.GetMinigameSaveData?.Invoke(Wrapper.Game.Labyrinth);
                Data.Instance.ttSaveData = JsonUtility.FromJson<TTSaveData>(saveString);
            }
            catch (Exception e) {
                Debug.LogError(e.Message);
            }

            if (Data.Instance.ttSaveData == null) {
                Data.Instance.ttSaveData = new TTSaveData();
            }
            else {
                for (int i=0; i<15; i++) {
                    SaveData.Instance.starsPerLevel[i] = Data.Instance.ttSaveData.MaxStarsPerLevel[i];
                }
                for (int i=0; i<5; i++) {
                    SaveData.Instance.dialogueSeen[i] = Data.Instance.ttSaveData.DialogueSeen[i];
                }
            }
            Data.Instance.researchData = new ResearchData();
            
        }



        // [DllImport("__Internal")]
        // private static extern void TwinTanglementLoad(string callback);

        /* private void Start() {
            #if UNITY_WEBGL == true && UNITY_EDITOR == false
                TwinTanglementLoad("loadData");
            #endif
        } */

        /* public static void LoadGame() {
            #if UNITY_WEBGL == true && UNITY_EDITOR == false
                TwinTanglementLoad ("loadData");
            #endif
        }

        public void loadData(string data) {
            Debug.Log("I just got this data:");
            Debug.Log(data);
            //TODO Load Data into memory

            if (data.Length > 0) {
                Data tempData = JsonUtility.FromJson<Data>(data);
                
                for (int i=0; i<15; i++) {
                    SaveData.Instance.starsPerLevel[i] = tempData.MaxStarsPerLevel[i];
                }
            }
        } */
    }
}