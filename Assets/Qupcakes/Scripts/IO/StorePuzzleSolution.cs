using UnityEngine;
using System.Collections;

namespace Qupcakery
{
    public class StorePuzzleSolution : MonoBehaviour
    {
        // First two columns store the puzzle, rest stores the sln
        LevelManager lm;
        ButtonController bc;

        // Use this for initialization
        void Start()
        {
            lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
            bc = GameObject.Find("Button(Clone)").GetComponent<ButtonController>();
            bc.ButtonPressed += StoreSolution;
        }

        void StoreSolution()
        {
            int levelInd = lm.level.LevelInd;
            int puzzleInd = lm.currentBatchNum;

            Puzzle puzzle = lm.level.Puzzles[puzzleInd];
            GameManagement.Instance.game.gameStat.
                SetCompressedPuzzle(puzzleInd, puzzle);

            GameManagement.Instance.game.gameStat.
                SetCompressedPuzzleSolution(puzzleInd, GateSlots.Instance.Solution, puzzle.Size);

            //Debug.Log("Saving level " + levelInd + ", puzzle " + puzzleInd + " solution: ");

            //string print = "\n";
            //for (int i = 0; i < 3; i++)
            //{
            //    for (int j = 0; j < 6; j++)
            //    {
            //        print += puzzleAndSolutions[i,j];
            //        print += ", ";
            //    }
            //    print += "\n";
            //}
            //Debug.Log(print);
        }

        private void OnDestroy()
        {
            bc.ButtonPressed -= StoreSolution;
        }
    }
}
