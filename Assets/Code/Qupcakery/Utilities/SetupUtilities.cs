using UnityEngine;
using UnityEngine.U2D.Animation;
using System.Collections.Generic;
using Costume = Qupcakery.AssetCostumeUtilities;

/* 
 * Utility functions for setting up the game scene
 */
namespace Qupcakery
{
    public static class SetupUtilities
    {
        private const int maxGatePerBelt = 4;

        /* Given Level object, set up the scene for the current level */
        public static void SetupGameScene(Level level, GameObject button,
            GameObject tablePrefab,
            GameObject gatePrefab, GameObject beltPrefab)
        {
            button.SetActive(true);
            //panel.SetActive(true);

            CreateTable(tablePrefab, level.TotalBeltCnt);
            CreateBelts(beltPrefab, level.TotalBeltCnt);

            SetupGatePanel(level, gatePrefab);
            // Set up gate slots
            GateSlots.Instance.InitializeSlots(level.TotalBeltCnt, maxGatePerBelt);
        }

        // Instantiate serving table(s) at the end of the delivery belt
        private static void CreateTable(GameObject tablePrefab, int tableCnt)
        {
            GameObject table = Object.Instantiate(tablePrefab, new Vector3(4.5f, -0.4f, 0f), Quaternion.identity);
            switch (tableCnt)
            {
                case 1:
                    table.GetComponent<SpriteResolver>().SetCategoryAndLabel("Basic", "Short");
                    break;
                default:
                    table.GetComponent<SpriteResolver>().SetCategoryAndLabel("Basic", "Long");
                    break;
            }
        }

        // Initialize panel 
        static void SetupGatePanel(Level level, GameObject gatePrefab)
        {
            // Reset gatebank
            GateBank.Instance.ResetGateBank();
            GateBankGUI.Instance.InitiailizesGateBankGUI(gatePrefab);
            // Loop thru all available type of gates in the current level
            GateType gt;
            for (int i = 0; i < level.AvailableGates.Length; i++)
            {
                if (level.AvailableGates[i] > 0)
                {
                    gt = (GateType)i;
                    GateBank.Instance.AddGateToBank(gt, level.AvailableGates[i]);

                }
            }
        }

        // Instantiate delivery belt(s) 
        private static void CreateBelts(GameObject beltPrefab, int beltCnt)
        {
            switch (beltCnt)
            {
                case 1:
                    Object.Instantiate(beltPrefab, BeltPosition(1, 0), Quaternion.identity);
                    break;
                case 2:
                    Object.Instantiate(beltPrefab, BeltPosition(2, 1), Quaternion.identity);
                    Object.Instantiate(beltPrefab, BeltPosition(2, 0), Quaternion.identity);
                    break;
                case 3:
                    Object.Instantiate(beltPrefab, BeltPosition(3, 2), Quaternion.identity);
                    Object.Instantiate(beltPrefab, BeltPosition(3, 1), Quaternion.identity);
                    Object.Instantiate(beltPrefab, BeltPosition(3, 0), Quaternion.identity);
                    break;
            }
        }

        // Calculate belt positions 
        public static Vector2 BeltPosition(int totalBeltCnt, int beltIndex)
        {
            float distanceBetweenBelt = 2f;
            float displacement = 0f;

            if (totalBeltCnt == 2) displacement = -1 * distanceBetweenBelt / 2;
            else if (totalBeltCnt == 3) displacement = -1 * distanceBetweenBelt;

            Vector2 position = new Vector2(-1.2f, displacement + beltIndex * distanceBetweenBelt);
            return position;
        }

        // Given Level object, set up the scene for the experiment level
        public static void SetupExperimentScene(Level level, GameObject button,
            GameObject tablePrefab, GameObject panel, GameObject panelPrefab,
            GameObject gatePrefab, GameObject beltPrefab, GameObject cakePrefab)
        {
            button.SetActive(true);
            panel.SetActive(true);
            CreateTable(tablePrefab, level.TotalBeltCnt);
            CreateBelts(beltPrefab, level.TotalBeltCnt);
            GameObject cakePanel = CreateCakePanel(panelPrefab);

            SetupCakePanel(level, cakePanel, cakePrefab);
            SetupGatePanel(level, gatePrefab);
            // Set up gate slots
            GateSlots.Instance.InitializeSlots(level.TotalBeltCnt, maxGatePerBelt);
            // Set up cake tracker
            CakeOnBeltTracker.Instance.InitializeTracker(level.TotalBeltCnt);
        }


        // Instantiate cake-bank panel (selection panel) at the left side of the player screen 
        private static GameObject CreateCakePanel(GameObject panelPrefab)
        {
            GameObject panel = Object.Instantiate(panelPrefab, new Vector3(-7.6f, -0.05f, 0f), Quaternion.identity);
            panel.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            panel.transform.localScale = new Vector3(0.6f, 1f, 1f);
            return panel;
        }

        // Set up cake panel 
        static void SetupCakePanel(Level level, GameObject cakePanel, GameObject cakePrefab)
        {
            //int levelInd = level.LevelInd;

            //Vector3 panelPosition = cakePanel.transform.position;

            //GameObject vanillaCake =
            //    Object.Instantiate(cakePrefab,
            //    panelPosition + new Vector3(0f, 2.5f, 0f), Quaternion.identity);
            //AssetCostumeUtilities.SetCakeBoxCostume(vanillaCake,
            //    new Cake(GameCakeType.Vanilla));
            //    vanillaCake.GetComponent<ExperimentCakeBoxController>()
            //        .SetCakeBoxState(new Cake(GameCakeType.Vanilla));

            //GameObject chocolateCake =
            //    Object.Instantiate(cakePrefab,
            //    panelPosition + new Vector3(0f, 0.8f, 0f), Quaternion.identity);
            //AssetCostumeUtilities.SetCakeBoxCostume(chocolateCake,
            //    new Cake(GameCakeType.Chocolate));
            //chocolateCake.GetComponent<ExperimentCakeBoxController>()
            //        .SetCakeBoxState(new Cake(GameCakeType.Chocolate));

            //if (levelInd >= 11)
            //{
            //    GameObject vanillaChocolate =
            //    Object.Instantiate(cakePrefab,
            //    panelPosition + new Vector3(0f, -0.9f, 0f), Quaternion.identity);
            //    AssetCostumeUtilities.SetCakeBoxCostume(vanillaChocolate,
            //        new Cake(GameCakeType.Vanilla50_Chocolate50));
            //    vanillaChocolate.GetComponent<ExperimentCakeBoxController>()
            //        .SetCakeBoxState(new Cake(GameCakeType.Vanilla50_Chocolate50));


            //    GameObject chocolateVanilla =
            //        Object.Instantiate(cakePrefab,
            //        panelPosition + new Vector3(0f, -2.5f, 0f), Quaternion.identity);
            //    AssetCostumeUtilities.SetCakeBoxCostume(chocolateVanilla,
            //        new Cake(GameCakeType.Vanilla50_Chocolate50_Neg));
            //    chocolateVanilla.GetComponent<ExperimentCakeBoxController>()
            //        .SetCakeBoxState(new Cake(GameCakeType.Vanilla50_Chocolate50_Neg));

            //}
        }

    }
}
