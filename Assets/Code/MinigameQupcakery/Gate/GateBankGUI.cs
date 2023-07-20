using System.Collections.Generic;
using UnityEngine;
using Costume = Qupcakery.AssetCostumeUtilities;

/*
 * Manages gameobjects in gatebank (front end)
 */
namespace Qupcakery
{
    public class GateBankGUI
    {
        // Singleton instance
        public static GateBankGUI Instance { get; private set; }
            = new GateBankGUI();

        private GameObject gatePrefab;
        private int gateTypeInd;
        private Dictionary<GateType, (int, GameObject)> GatesInBank
            = new Dictionary<GateType, (int, GameObject)>();

        private bool Initialized = false;

        // Initializes/updates instance
        public void InitiailizesGateBankGUI(GameObject newGatePrefab)
        {
            GatesInBank.Clear();

            gateTypeInd = 0;

            if (!Initialized)
            {
                gatePrefab = newGatePrefab;
                GateBank.Instance.BankAddedNewGate += OnBankAddedNewGate;
                GateBank.Instance.GateBankUpdated += OnGateBankUpdated;
                Initialized = true;
            }
        }

        public Vector2 GetGatePositionInBank(GateType gt)
        {
            int gateTypeInd = GatesInBank[gt].Item1;
            return new Vector2(-4.83f + gateTypeInd * 1.7f, -4.05f);
        }

        // Calculates gate position in bank
        private Vector2 GetGatePositionInBank(int currGateTypeInd)
        {
            // calculate position
            Vector2 position = new Vector2(-4.83f + currGateTypeInd * 1.7f, -4.05f);
            return position;
        }

        // Listener
        public void OnBankAddedNewGate(GateType gt, int cnt)
        {
            Vector2 position = GetGatePositionInBank(gateTypeInd);

            GameObject gate = UnityEngine.Object.Instantiate(gatePrefab,
                position, Quaternion.identity);

            gate.GetComponent<GateOperationController>().SetGateType(gt);

            GatesInBank[gt] = (gateTypeInd, gate);
            gateTypeInd++;

            // Set gate costume
            Costume.SetGateCostume(gate, gt, cnt);
        }

        // Listener
        public void OnGateBankUpdated(GameObject gate,
            GateBank.GateBankOperation op)
        {
            GateType gt = gate.GetComponent<GateOperationController>().gate.Type;
            int currGateTypeInd = GatesInBank[gt].Item1;
            GameObject currGateObj = GatesInBank[gt].Item2;

            switch (op)
            {
                case GateBank.GateBankOperation.ADD:
                    if (currGateObj != null)
                        UnityEngine.GameObject.Destroy(currGateObj);
                    Costume.SetGateCostume(gate, gt,
                        GateBank.Instance.GateBankTracker[gt]);

                    // update gatebank
                    GatesInBank[gt] = (currGateTypeInd, gate);
                    break;
                case GateBank.GateBankOperation.SUBTRACT:
                    Costume.DeactivateGateCntObj(gate);

                    // Instantiate new gate in bank
                    if (GateBank.Instance.GateBankTracker[gt] > 0)
                    {
                        Vector2 position = GetGatePositionInBank(currGateTypeInd);
                        GameObject newGate = UnityEngine.Object.Instantiate(gatePrefab,
                            position, Quaternion.identity);
                        Costume.SetGateCostume(newGate, gt,
                            GateBank.Instance.GateBankTracker[gt]);
                        newGate.GetComponent<GateOperationController>().SetGateType(gt);
                        // update gatebank
                        GatesInBank[gt] = (currGateTypeInd, newGate);
                    }
                    else
                    {
                        GatesInBank[gt] = (currGateTypeInd, null);
                    }
                    break;
            }
        }
    }
}
