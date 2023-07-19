using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wrapper 
{
    public class RewardResearchData : MonoBehaviour
    {
        public string currentCard;
        public string timeClicked;
        public string displayType;

        public static RewardResearchData Instance;

        const string rewardIntroNew = "RW_Intro_0";
        const string rewardIntroCards = "RW_Intro_1";

        private void Awake()
        {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Events.PlayMusic?.Invoke("W_RewardMusic");

            // determine if we have been here before
            var rewardStats = Events.GetRewardDialogStats.Invoke();
            if (rewardStats.Item1) return;
            else
            {
                if (rewardStats.Item2) Events.StartDialogueSequence?.Invoke(rewardIntroCards);
                else Events.StartDialogueSequence?.Invoke(rewardIntroNew);
            }
        }

        public string RewardRDtoString() {
            string jsonData = JsonUtility.ToJson(this);
            return jsonData;
        }

        // DESTROYING INSTANCES WHEN NOT IN USE
        private void OnEnable() {
            Wrapper.Events.MinigameClosed += DestroyDataObject;
            Events.DialogueSequenceEnded += SetTextSeen;
        }
        private void OnDisable(){
            Wrapper.Events.MinigameClosed -= DestroyDataObject;
            Events.DialogueSequenceEnded -= SetTextSeen;
        }
        private void DestroyDataObject() {
            Time.timeScale = 1f;
            Destroy(GameObject.Find("RewardResearchData"));
        }

        void SetTextSeen()
        {
            Events.SetRewardTextSeen?.Invoke(true);
        }
    }
}