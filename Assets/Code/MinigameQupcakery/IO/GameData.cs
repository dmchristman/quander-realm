using System;

// Firebase non-research load/save data

namespace Qupcakery
{
    [System.Serializable]
    public class GameData
    {
        public Wrapper.Game gameID = Wrapper.Game.Qupcakes;
        public int TotalEarning = 0;
        public int MaxLevelCompleted = 0;
        public int[] levelPerformance = new int[Constants.MaxLevelCnt];

        public void UpdateGameData(GameStat stat)
        {
            TotalEarning = stat.TotalEarning;
            MaxLevelCompleted = stat.MaxLevelCompleted;

            for (int i=0; i< MaxLevelCompleted; i++)
            {
                levelPerformance[i] = stat.GetLevelPerformance(i+1);
            }
        }
    }
}
