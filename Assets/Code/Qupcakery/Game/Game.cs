using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * Game info: all avaliable levels  
 */
namespace Qupcakery
{
    public class Game
    {
        public GameStat gameStat;
        public GameData gameData; 
        public int CurrLevelInd { get; private set; } = 0;

        public int MaxLevelCnt { get; } = Constants.MaxLevelCnt;
        public int maxPuzzleCnt { get; } = Constants.MaxPuzzleCnt;
        const int maxGateCnt = Constants.MaxGateTypeCnt; // gate type

        private Level level;

        // Constructor 
        public Game()
        {
            level = new Level(maxPuzzleCnt, maxGateCnt);
            gameStat = new GameStat(MaxLevelCnt);
        }

        // Update level
        public void UpdateLevel(int levelInd)
        {
            level.Update(levelInd);
            CurrLevelInd = levelInd;
        }

        // Get current level
        public Level GetCurrLevel()
        {
            return level;
        }
    }

}
