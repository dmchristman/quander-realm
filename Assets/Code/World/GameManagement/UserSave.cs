using System.Collections.Generic;
using UnityEngine;

namespace Wrapper
{
    [System.Serializable]
    public class UserSave
    {
        public string id = string.Empty;
        public string[] minigameSaves;
        public List<string> rewards;
        public bool introDialogueSeen = false;
        public bool rewardDialogueSeen = false;

        public UserSave(string idString = "", string rewardID = "")
        {
            rewards = new List<string>();
            minigameSaves = new string[] { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

            if (!(idString.Equals(string.Empty)))
                id = idString.Trim();

            AddReward(rewardID);
        }

        public bool AddReward(string rewardID)
        {
            if (rewardID.Equals(string.Empty))
                return false;

            string formatted = FormatString(rewardID);
            if (rewards.Contains(formatted))
                return false;

            if (rewards.Count == 0) rewardDialogueSeen = false; // if we haven't seen the reward dialog while having a reward, reset this bool
            rewards.Add(formatted);
            return true;
        }

        public bool IsNewSave()
        {
            if (HasAnyRewards()) return false;
            if (introDialogueSeen || rewardDialogueSeen) return false;
            foreach (var data in minigameSaves) if (!data.Equals(string.Empty)) return false;
            return true;
        }

        public bool HasReward(string rewardID)
        {
            if (rewards == null)
                return false;

            return rewards.Contains(FormatString(rewardID));
        }

        public bool HasAnyRewards()
        {
            if (rewards == null) return false;
            else return rewards.Count > 0;
        }

        public bool FirstRewardFromGame(string gamePrefix)
        {
            // will already have one, check for exactly one
            int counter = 0;
            foreach (var reward in rewards)
                if (reward.Contains(gamePrefix)) counter++;

            return counter == 1;
        }

        public void UpdateMinigameSave(Game game, object data)
        {
            //todo: error catching?
            minigameSaves[(int)game] = JsonUtility.ToJson(data);
        }

        public string GetMinigameSave(Game game)
        {
            //todo: error catching?
            string saveString = minigameSaves[(int)game];
            if (saveString.Equals(string.Empty))
                return null;

            return saveString;
        }

        private string FormatString(string rawString)
        {
            return rawString.Trim().ToLower();
        }
    }
}
