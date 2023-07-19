using System;
using System.Collections.Generic;
using SysDebug = System.Diagnostics.Debug;
using UnityEngine;

/*
 * Keeps track of overall player progress
 * Updated upon completion (including retries) of any level
 */
namespace Qupcakery
{
    [System.Serializable]
    public class GameStat
    {
        public enum LevelResult
        {
            WIN = 1, LOSS = 0, QUIT = -1
        }

        public int TotalEarning;
        public int MaxLevelCompleted; // largest level index of completed levels
        public int LevelJustAttempted;
        public int Result; // loss: 0, win: 1, quit: -1

        // Stores player performance for completed levels
        [SerializeField]
        private int[] levelPerformance;
        [SerializeField]
        private bool[] PuzzleResults; // true: complete success, false: fail at least one order
        [SerializeField]
        private long[] CompressedPuzzleSolutions;
        [SerializeField]
        private int[] CompressedPuzzles; // Compressed version

        // Constructor for a new game
        public GameStat(int totalLevelCnt)
        {
            TotalEarning = 0;
            MaxLevelCompleted = 0;
            LevelJustAttempted = 0;
            levelPerformance = new int[totalLevelCnt];
            PuzzleResults = new bool[Constants.MaxNumberOfPuzzlePerLevel];
            CompressedPuzzleSolutions = new long[Constants.MaxNumberOfPuzzlePerLevel];
            CompressedPuzzles = new int[Constants.MaxNumberOfPuzzlePerLevel];
        }

        public void SetLevelJustAttempted(int levelInd)
        {
            LevelJustAttempted = levelInd;
            // Reset puzzle result record
            for (int i = 0; i < PuzzleResults.Length; i++) PuzzleResults[i] = false;
            ResetOnNewLevel();
        }

        void ResetOnNewLevel()
        {
            Array.Clear(CompressedPuzzleSolutions, 0, CompressedPuzzleSolutions.Length);
            Array.Clear(CompressedPuzzles, 0, CompressedPuzzles.Length);
        }

        public void UpdateTotalEarning(int newTotalEarning)
        {
            TotalEarning = (int)newTotalEarning;
        }

        // Retrieves player performance for a specific level (# of stars earned)
        public int GetLevelPerformance(int levelInd)
        {
            SysDebug.Assert(levelInd <= MaxLevelCompleted);

            return levelPerformance[levelInd - 1];
        }

        // Updates player performance for a specific level
        // Updates TotalCompletedLevels as needed
        public void SetLevelPerformance(int levelInd, int starCnt)
        {
            //Debug.Log("setting level performance for level " + levelInd);

            // If player completes a brand new level
            if (levelInd > MaxLevelCompleted)
                MaxLevelCompleted = levelInd;

            // Updates performance info
            levelPerformance[levelInd - 1] = starCnt;
        }


        public void SetLevelResultAndSave(LevelResult result)
        {
            Result = (int)result;
            /* Save to databse */
            SaveGame.Instance.Save();
        }

        public void SetPuzzleResult(int puzzleInd, bool result)
        {
            PuzzleResults[puzzleInd] = result;
        }

        public void SetCompressedPuzzle(int puzzleInd, Puzzle puzzle)
        {
            CompressedPuzzles[puzzleInd] = DataCompression.CompressPuzzle(puzzle);

            int[,] decompressedPuzzle = DataCompression.DecompressPuzzle(CompressedPuzzles[puzzleInd]);

            //Debug.Log("Compressed puzzle: " + puzzle);
            //Debug.Log("Decompressed puzzle: " + Utilities.GetStringFrom2DIntArray(decompressedPuzzle, puzzle.Size, 2));
        }

        public void SetCompressedPuzzleSolution(int puzzleInd, int[,] solution, int puzzleSize)
        {
            CompressedPuzzleSolutions[puzzleInd] =
                DataCompression.CompressPuzzleSolution(solution, puzzleSize);

            //Debug.Log("Compressed puzzle solution: " + Utilities.GetStringFrom2DIntArray(solution, puzzleSize, 4));

            int[,] sln = DataCompression.DecrompressPuzzleSolution(CompressedPuzzleSolutions[puzzleInd]);
            //Debug.Log("Decompressed puzzle solution: " + Utilities.GetStringFrom2DIntArray(sln, puzzleSize, 4));
        }
    }
}
