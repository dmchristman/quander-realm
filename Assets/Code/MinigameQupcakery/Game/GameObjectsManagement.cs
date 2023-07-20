using Unity;
using UnityEngine;
using System.Collections;
using System.Linq;

namespace Qupcakery
{
    public static class GameObjectsManagement
    {
        public static GameObject[] Customers = new GameObject[3];
        public static GameObject[] CakeBoxes = new GameObject[3];
        public static GameObject Button;
        //public static GameObject Panel;

        /* Generates all game objects and set them to inactive
         * Called once when the game is first loaded */
        public static void CreateGameObjects(GameObject customerPrefab,
            GameObject cakeboxPrefab, GameObject buttonPrefab,
            GameObject panelPrefab)
        {
            foreach (int i in Enumerable.Range(0, Constants.MaxCustomerPerBatch))
            {
                /* Generates customers outside the game scene */
                Customers[i] = UnityEngine.Object.Instantiate(customerPrefab,
                    new Vector2(15, 5), Quaternion.identity);
                Customers[i].GetComponent<CustomerManager>().SetBeltInd(i);
                Object.DontDestroyOnLoad(Customers[i]);

                /* Generates cake boxes outside the game scene */
                CakeBoxes[i] = UnityEngine.Object.Instantiate(cakeboxPrefab,
                    new Vector2(-15, 5), Quaternion.identity);
                CakeBoxes[i].GetComponent<CakeBoxController>().cake.SetBeltInd(i);
                Object.DontDestroyOnLoad(CakeBoxes[i]);
            }

            Button = Object.Instantiate(buttonPrefab, new Vector3(5f, -4f, 0f),
                Quaternion.identity);
            Object.DontDestroyOnLoad(Button);
            /* Subscribe to 1st customer events */
            Button.GetComponent<ButtonController>()
                .SubscribeToCustomerEvent(Customers[0]);

            //Panel = GeneratePanel(panelPrefab);
            //Object.DontDestroyOnLoad(Panel);

            PuzzleCorrectionChecker.Initialize();

            /* Deactive all static gameobjects */
            DeactiveAllGameObjects();
        }

        public static void DeactiveAllGameObjects()
        {
            foreach (int i in Enumerable.Range(0, Constants.MaxCustomerPerBatch))
            {
                Customers[i].SetActive(false);
                CakeBoxes[i].SetActive(false);
            }
            Button.SetActive(false);
            //Panel.SetActive(false);
        }

        public static void ResetAllGameObjects()
        {
            for (int i = 0; i < Constants.MaxCustomerPerBatch; i++)
            {
                ResetCakeBoxObj(CakeBoxes[i]);
                ResetCustomerObj(Customers[i]);
            }
            ResetButtonObj(Button);
        }

        public static void ResetButtonObj(GameObject button)
        {
            button.GetComponent<ButtonController>().ResetButton();
        }

        public static void ResetCakeBoxObj(GameObject cakeBox)
        {
            GameObject EntanglementLabel = cakeBox.transform.Find("EntanglementLabel").gameObject;
            GameObject box = cakeBox.transform.Find("Box").gameObject;

            box.SetActive(true);
            EntanglementLabel.SetActive(true);

            CakeBoxController cc = cakeBox.GetComponent<CakeBoxController>();
            cc.Reset();

            cakeBox.SetActive(false);
        }

        public static void ResetCustomerObj(GameObject customer)
        {
            GameObject bubble = customer.transform.Find("Bubble").gameObject;
            GameObject reaction = bubble.transform.Find("Reaction").gameObject;
            GameObject patience = customer.transform.Find("PatienceBar").gameObject;

            bubble.SetActive(true);
            patience.SetActive(true);
            reaction.SetActive(false);

            CustomerManager cm = customer.GetComponent<CustomerManager>();
            cm.ResetCustomerManager();

            customer.SetActive(false);
        }

        // Instantiate gate-bank panel (selection panel) at the bottom of the player screen 
        private static GameObject GeneratePanel(GameObject panelPrefab)
        {
            return Object.Instantiate(panelPrefab,
                new Vector3(0f, -4f, 0f), Quaternion.identity);
        }

        public static void DeleteGameObjects()
        {
            foreach (var customer in Customers)
                Object.Destroy(customer);
            foreach (var cakebox in CakeBoxes)
                Object.Destroy(cakebox);
            Object.Destroy(Button);
        }
    }
}

