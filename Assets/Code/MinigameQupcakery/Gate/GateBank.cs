using System;
using System.Collections.Generic;
using UnityEngine;

/* Manages the gate bank (back end) */
namespace Qupcakery
{
    public class GateBank
    {
        public enum GateBankOperation
        {
            ADD, SUBTRACT
        }

        // For alerting the UI to update representation
        public delegate void GateBankUpdatedEventHandler(GameObject gate,
            GateBankOperation op);
        public GateBankUpdatedEventHandler GateBankUpdated;
        public delegate void BankAddedNewGateEventHandler(GateType gt,
            int cnt);
        public BankAddedNewGateEventHandler BankAddedNewGate;

        // Singleton instance
        public static GateBank Instance { get; private set; }
            = new GateBank();

        public Dictionary<GateType, int> GateBankTracker { get; private set; }
            = new Dictionary<GateType, int>();

        public void ResetGateBank()
        {
            GateBankTracker.Clear();

        }

        // Decrease gate (of type gt) count by 1 if GateBankTracker[gt] >= 1,
        // and returns the new count. Otherwise do nothing
        // Requirement: gt must already exist in GateBankTracker
        public int SubtractGateFromBank(GameObject gate)
        {
            GateType gt = gate.GetComponent<GateOperationController>()
                .GetGateType();

            int cnt = GateBankTracker[gt];

            if (cnt == 0) return 0;

            GateBankTracker[gt] = cnt - 1;
            // Publish update event
            OnGateBankUpdated(gate, GateBankOperation.SUBTRACT);
            return cnt - 1;
        }

        // Increases gate count by 1, returns target gate position
        public Vector2 AddGateToBank(GameObject gate)
        {
            GateOperationController gc = gate.GetComponent<GateOperationController>();
            GateType gt = gc.GetGateType();
            gc.ResetGateStatus();

            if (GateBankTracker.ContainsKey(gt))
            {
                int newCnt = GateBankTracker[gt] + 1;
                GateBankTracker[gt] = newCnt;
            }
            else
            {
                GateBankTracker[gt] = 1;
            }
            // Debug.Log("New gatebank count is" + GateBankTracker[gt].ToString());
            // Publish update event
            OnGateBankUpdated(gate, GateBankOperation.ADD);

            return GateBankGUI.Instance.GetGatePositionInBank(gt);
        }

        // Sets gate count to cnt, returns the new count
        // Used during level setup
        public int AddGateToBank(GateType gt, int cnt)
        {
            // Debug.Log("Adding " + cnt + "gates of type " + gt);
            GateBankTracker[gt] = cnt;

            // Publishes event
            OnBankAddedNewGate(gt, cnt);

            return cnt;
        }

        // Publisher
        protected virtual void OnGateBankUpdated(GameObject gate,
            GateBankOperation op)
        {
            if (GateBankUpdated != null)
            {
                GateBankUpdated(gate, op);
            }
        }

        // Publisher
        protected virtual void OnBankAddedNewGate(GateType gt,
            int cnt)
        {
            if (BankAddedNewGate != null)
            {
                BankAddedNewGate(gt, cnt);
            }
        }
    }
}
