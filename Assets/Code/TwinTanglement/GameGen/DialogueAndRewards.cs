using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Labyrinth
{
    public class DialogueAndRewards : MonoBehaviour
    {
        public static DialogueAndRewards Instance;

        public Dictionary<int, bool> levelDialogue = new Dictionary<int, bool>()
            { {-1, false}, {1, false}, {6, false}, {11, false}, {15, false} };

        public Dictionary<int, string> levelRewards = new Dictionary<int, string>()
            { {5, "LA_01"}, {15, "LA_02"}, {3, "LA_03"}, {4, "LA_04"}, 
            {8, "LA_05"}, {9, "LA_06"}, {10, "LA_07"}, {14, "LA_08"} };


        private void Awake() {
            if (Instance != null) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void updateDialogueDict() {
            // Checking if dict different from array in SaveData
            int i = 0;
            foreach(int val in new List<int>(levelDialogue.Keys)) {
                levelDialogue[val] = SaveData.Instance.dialogueSeen[i];
                i++;
            }
        }

        public void doDialogue(int level) {
            if (levelDialogue.ContainsKey(level) && levelDialogue[level] == false) {
                Wrapper.Events.StartDialogueSequence?.Invoke("LA_Level"+level);
                levelDialogue[level] = true;
            }
        }

        public void giveReward(int level) {
            if (levelRewards.ContainsKey(level) &&
            Wrapper.Events.IsRewardUnlocked?.Invoke(levelRewards[level]) == false) {
                Wrapper.Events.CollectAndDisplayReward?.Invoke(Wrapper.Game.Labyrinth, level);
            }
        }
    }
}