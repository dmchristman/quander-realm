using System;
using UnityEngine;

// Firebase non-research load/save data

namespace Qupcakery
{
    [System.Serializable]
    public class ResearchData
    {
        public string Username = Wrapper.Events.GetPlayerResearchCode?.Invoke();
        public string SaveData = string.Empty;

        public void UpdateResearchData(GameStat stat)
        {
            SaveData = JsonUtility.ToJson(GameManagement.Instance.game.gameStat);
        }
    }
}
