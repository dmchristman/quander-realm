using System;
using System.Collections.Generic;
using UnityEngine;
using Costume = Qupcakery.AssetCostumeUtilities;

/*
 * Dispatches puzzle
 */
namespace Qupcakery
{
    public static class DispatchPuzzle
    {
        // Dispatch a puzzle batch
        public static void Dispatch(Puzzle puzzle, GameObject[] customers,
            GameObject[] cakeBoxes)
        {
            for (int i = puzzle.Size - 1; i >= 0; i--)
            {
                int cakeSpec, orderSpec;
                (cakeSpec, orderSpec) = puzzle.entries[i];
                SetCakeBox(cakeSpec, puzzle.Size, i, GameObjectsManagement.CakeBoxes[i]);
                SetCustomer(orderSpec, puzzle.Size, i, GameObjectsManagement.Customers[i]);
            }
        }

        /* Set cakebox to the target type and place it at the beginning of the conveyor belt */
        private static void SetCakeBox(int cakeSpec,
            int puzzleSize, int ind, GameObject cakeBox)
        {
            cakeBox.SetActive(true);
            cakeBox.gameObject.transform.position =
                GameUtilities.GetCakeStartPosition(ind, puzzleSize);
            CakeBoxController cc = cakeBox.GetComponent<CakeBoxController>();

            if (cc.cake.bitInd == 1)
            {
                // Reset cakestate if it was entangled in the last batch
                cc.cake.ResetCakeState();
                cc.cake.bitInd = 0;
            }
            cc.UpdateCakeBoxState(cakeSpec);
        }

        /* Set customer order to the target and put the customer at  
         * the end of the target conveyor belt */
        private static void SetCustomer(int order,
           int puzzleSize, int ind, GameObject customerObj)
        {
            customerObj.SetActive(true);
            customerObj.transform.position =
                GameUtilities.GetCustomerStartPosition(ind, (int)puzzleSize);
            CustomerManager customerManager = customerObj.GetComponent<CustomerManager>();
            customerManager.AssignOrder(order);
            customerManager.SendCustomer();

            Costume.SetCustomerCostume(customerObj, order);
        }
    }
}

