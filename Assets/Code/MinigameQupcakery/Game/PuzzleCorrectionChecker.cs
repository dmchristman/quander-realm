using System;
using UnityEngine;

namespace Qupcakery
{
    public static class PuzzleCorrectionChecker
    {
        public delegate void OnResultCheckedHandler(bool[] result);
        public static OnResultCheckedHandler ResultChecked;

        private static int currLevel, beltCnt;
        private static bool[] PuzzleResult = new bool[] { false, false, false };

        private static LevelManager lm;

        public static void Initialize()
        {
            /* Subscribe to 1st Customer-batch-done event */
            CustomerManager cm = GameObjectsManagement.Customers[0]
                .GetComponent<CustomerManager>();
            cm.CakeReceived += OnFirstCakeReceived;
        }

        public static void UpdateOnNewLevel()
        {
            currLevel = GameManagement.Instance.game.CurrLevelInd;
            beltCnt = GameManagement.Instance.game.GetCurrLevel().TotalBeltCnt;
            lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        }

        private static void ResetResultRecord()
        {
            for (int i = 0; i < 3; i++) PuzzleResult[i] = false;
        }

        /* Check puzzle result */
        private static void OnFirstCakeReceived()
        {
            ResetResultRecord();
            CakeState cakeState;
            int order;

            int j = 0;
            int[] orderEntanglementPair = new int[2];
            bool hasEntanglement = false;

            for (int i = 0; i < beltCnt; i++)
            {
                cakeState = GameObjectsManagement.CakeBoxes[i]
                    .GetComponent<CakeBoxController>().cake.cakeState;
                order = GameObjectsManagement.Customers[i]
                    .GetComponent<CustomerManager>().order;
                //Debug.Log("Order is " + order + " Cakestate is (" + cakeState.probabilities[0] + ","
                //    + cakeState.probabilities[1] + "," + cakeState.probabilities[2] + "," + cakeState.probabilities[3] +
                //    "), phase: " + cakeState.phase);
                switch (order)
                {
                    case 0:
                        if (cakeState.ParamEquals(0)) PuzzleResult[i] = true;
                        break;
                    case 1:
                        if (cakeState.ParamEquals(1)) PuzzleResult[i] = true;
                        break;
                    case 2:
                    case 3:
                        if (cakeState.ParamEquals(0.5f, 0.5f, CakeState.Phase.Negative)
                            || cakeState.ParamEquals(0.5f, 0.5f, CakeState.Phase.Positive))
                            PuzzleResult[i] = true;
                        break;
                    case 4:
                    case 5:
                        /* Same entanglement */
                        if (cakeState.ParamEquals(0.5f, 0, 0, 0.5f, CakeState.Phase.Negative)
                           || cakeState.ParamEquals(0.5f, 0, 0, 0.5f, CakeState.Phase.Positive))
                            PuzzleResult[i] = true;
                        orderEntanglementPair[j] = i;
                        j++;
                        hasEntanglement = true;
                        break;
                    case 6:
                    case 7:
                        /* Opposite entanglement */
                        if (cakeState.ParamEquals(0, 0.5f, 0.5f, 0, CakeState.Phase.Negative)
                           || cakeState.ParamEquals(0, 0.5f, 0.5f, 0, CakeState.Phase.Positive))
                            PuzzleResult[i] = true;
                        orderEntanglementPair[j] = i;
                        j++;
                        hasEntanglement = true;
                        break;
                }
            }

            if (hasEntanglement)
            {
                // Make sure that entangled boxes are only given to entangled orders
                if (PuzzleResult[orderEntanglementPair[0]] !=
                    PuzzleResult[orderEntanglementPair[1]])
                {
                    PuzzleResult[orderEntanglementPair[0]] = false;
                    PuzzleResult[orderEntanglementPair[1]] = false;
                }
            }
            OnResultChecked();
        }

        private static void OnResultChecked()
        {
            bool result = true;
            for (int i = 0; i < beltCnt; i++)
            {
                if (!PuzzleResult[i]) result = false;
            }

            // Update GameStat
            GameManagement.Instance.game.gameStat.SetPuzzleResult(lm.currentBatchNum, result);

            if (ResultChecked != null) ResultChecked(PuzzleResult);
        }
    }
}
