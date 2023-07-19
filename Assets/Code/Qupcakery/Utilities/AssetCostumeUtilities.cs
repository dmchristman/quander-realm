using System;
using UnityEngine;
using UnityEngine.U2D.Animation;

/*
 * Utility functions for setting gameobject costume
 */

namespace Qupcakery
{
    public static class AssetCostumeUtilities
    {
        // Change object sprite 
        public static void SetCostume(GameObject obj, string category, string label)
        {
            obj.GetComponent<SpriteResolver>().SetCategoryAndLabel(category, label);
        }

        // Change object sprite to the one with label under category "Basic"
        public static void SetCostume(GameObject obj, string label)
        {
            obj.GetComponent<SpriteResolver>().SetCategoryAndLabel("Basic", label);
        }

        // Set costume for cake box gameobject, called during dispatch and in game
        public static void SetCakeBoxCostume(GameObject cakeBox, Cake cakeSpec)
        {
            CakeState cakeState = cakeSpec.cakeState;

            GameObject entanglementLabel = cakeBox.transform.Find("EntanglementLabel").gameObject;
            entanglementLabel.SetActive(false);

            GameObject box = cakeBox.transform.Find("Box").gameObject;
            switch (cakeState)
            {
                case var _ when cakeState.ParamEquals(0):
                    /* vanilla */
                    SetCostume(box, "CakeBox", "Vanilla");
                    break;
                case var _ when cakeState.ParamEquals(1):
                    /* chocolate */
                    SetCostume(box, "CakeBox", "Chocolate");
                    break;
                case var _ when cakeState.ParamEquals(0.5f, 0.5f, CakeState.Phase.Positive):
                    /* 50-50 positive */
                    SetCostume(box, "CakeBox", "Positive");
                    break;
                case var _ when cakeState.ParamEquals(0.5f, 0.5f, CakeState.Phase.Negative):
                    /* 50-50 negative */
                    SetCostume(box, "CakeBox", "Negative");
                    break;
                case var _ when cakeState.ParamEquals(0.5f, 0, 0, 0.5f, CakeState.Phase.Positive):
                    /* |00> + |11> */
                    entanglementLabel.SetActive(true);
                    SetCostume(entanglementLabel, "Basic", "Same");
                    SetCostume(box, "CakeBox", "Positive");
                    break;
                case var _ when cakeState.ParamEquals(0.5f, 0, 0, 0.5f, CakeState.Phase.Negative):
                    /* |00> - |11> */
                    entanglementLabel.SetActive(true);
                    SetCostume(entanglementLabel, "Basic", "Same");
                    SetCostume(box, "CakeBox", "Negative");
                    break;
                case var _ when cakeState.ParamEquals(0, 0.5f, 0.5f, 0, CakeState.Phase.Positive):
                    /* |01> + |10> */
                    entanglementLabel.SetActive(true);
                    SetCostume(entanglementLabel, "Basic", "Opposite");
                    SetCostume(box, "CakeBox", "Positive");
                    break;
                case var _ when cakeState.ParamEquals(0, 0.5f, 0.5f, 0, CakeState.Phase.Negative):
                    /* |01> - |10> */
                    entanglementLabel.SetActive(true);
                    SetCostume(entanglementLabel, "Basic", "Opposite");
                    SetCostume(box, "CakeBox", "Negative");
                    break;
                default:
                    throw new ArgumentException("Invalid cake state: " + cakeState);
            }
        }

        // Set costume for cake object (used for the cake after removing the box)
        public static void SetCakeCostume(GameObject cake, GameCakeType cakeType)
        {
            switch (cakeType)
            {
                case GameCakeType.Vanilla:
                    SetCostume(cake, "Cake", "Vanilla");
                    break;
                case GameCakeType.Chocolate:
                    SetCostume(cake, "Cake", "Chocolate");
                    break;
                case GameCakeType.Vanilla50_Chocolate50:
                    SetCostume(cake, "Cake", "VC");
                    break;
                case GameCakeType.Vanilla50_Chocolate50_Neg:
                    SetCostume(cake, "Cake", "VC_Neg");
                    break;
                default:
                    throw new ArgumentException("Unrecognized cake type");
            }
        }

        // Set costume for customer gameobject (monster + order)
        public static void SetCustomerCostume(GameObject customer, int orderSpec)
        {
            GameCakeType ct = GetGameCakeTypeFromOrderSpec(orderSpec);
            GameObject bubble = customer.transform.Find("Bubble").gameObject;

            SetBubbleCostume(bubble, orderSpec);

            // Randomly pick a monster costume
            System.Random rd = new System.Random(Guid.NewGuid().GetHashCode());
            int costume = rd.Next(0, 3);
            SetCostume(customer, "Basic", costume.ToString());
            customer.GetComponent<CustomerManager>().animator.SetInteger("CustomerInd", costume);
        }

        private static GameCakeType GetGameCakeTypeFromOrderSpec(int orderSpec)
        {
            switch (orderSpec)
            {
                case 0:
                    return GameCakeType.Vanilla;
                case 1:
                    return GameCakeType.Chocolate;
                case 2:
                    return GameCakeType.Vanilla50_Chocolate50;
                case 3:
                    return GameCakeType.Vanilla50_Chocolate50_Neg;
                case 4:
                case 5:
                case 6:
                    return GameCakeType.Vanilla50_Chocolate50;
                case 7:
                    return GameCakeType.Vanilla50_Chocolate50_Neg;
                default:
                    throw new ArgumentException("Invalid order spec number: " + orderSpec);
            }
        }

        private static void SetBubbleCostume(GameObject bubble, int orderSpec)
        {
            GameObject entanglementLabel = bubble.transform.Find("EntanglementLabel").gameObject;

            entanglementLabel.SetActive(false);
            switch (orderSpec)
            {
                case 0:
                    SetCostume(bubble, "Basic", "Vanilla");
                    break;
                case 1:
                    SetCostume(bubble, "Basic", "Chocolate");
                    break;
                case 2:
                case 3:
                    SetCostume(bubble, "Basic", "Surprise");
                    break;
                case 4:
                case 5:
                    SetCostume(bubble, "Basic", "Surprise");
                    entanglementLabel.SetActive(true);
                    SetCostume(entanglementLabel, "Basic", "Same");
                    break;
                case 6:
                case 7:
                    SetCostume(bubble, "Basic", "Surprise");
                    entanglementLabel.SetActive(true);
                    SetCostume(entanglementLabel, "Basic", "Opposite");
                    break;
            }
        }

        // Deactivate cnt obj for gate
        public static void DeactivateGateCntObj(GameObject gate)
        {
            GameObject gateCnt = gate.transform.Find("GateCount").gameObject;
            gateCnt.SetActive(false);
        }

        // Set costume for gate in bank
        public static void SetGateCostume(GameObject gate, GateType gateType,
            int cnt)
        {
            SetGateCostume(gate, gateType);

            GameObject gateCnt = gate.transform.Find("GateCount").gameObject;
            gateCnt.SetActive(true);
            string cntLabel = cnt.ToString();
            SetCostume(gateCnt, category: "Basic", label: cntLabel);
        }

        // Set costume for gate outside bank
        public static void SetGateCostume(GameObject gate, GateType gateType)
        {
            GameObject gateCnt = gate.transform.Find("GateCount").gameObject;
            gateCnt.SetActive(false);

            switch (gateType)
            {
                case GateType.NOT:
                    SetCostume(gate, category: "Basic", label: "NOT");
                    break;
                case GateType.CNOT:
                    SetCostume(gate, category: "Basic", label: "CNOT");
                    break;
                case GateType.SWAP:
                    SetCostume(gate, category: "Basic", label: "SWAP");
                    break;
                case GateType.H:
                    SetCostume(gate, category: "Basic", label: "H");
                    break;
                case GateType.Z:
                    SetCostume(gate, category: "Basic", label: "Z");
                    break;
                default:
                    throw new ArgumentException("Unsupported gate type : " +
                        gateType);
            }
        }

        // Set costume for gate outside bank
        public static void SetGateCostumeOnBelt(GameObject gate)
        {
            string label = gate.GetComponent<SpriteResolver>().GetLabel();
            SetCostume(gate, category: "OnBelt", label: label);
        }

        // Set costume for gate outside bank
        public static void SetGateCostumeOffBelt(GameObject gate)
        {
            string label = gate.GetComponent<SpriteResolver>().GetLabel();
            SetCostume(gate, category: "Basic", label: label);
        }
    }
}
