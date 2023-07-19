using System;
using UnityEngine;

namespace Qupcakery
{
    public class CNotGate : Gate
    {
        public CNotGate() : base(GateType.CNOT, 2)
        {
        }

        // Implementes CNOT operation
        public override void ExecuteGate(ref Cake control, ref Cake target)
        {
            CakeState ctrlCakeState = control.cakeState;
            CakeState targetCakeState = target.cakeState;

            System.Diagnostics.Debug.Assert(ctrlCakeState.stateSize == 2);

            if (Mathf.Approximately(ctrlCakeState.probabilities[1], 1))
            {
                Debug.Log("Control is chocolate");
                /* Ctrl is Chocolate */
                if (Mathf.Approximately(targetCakeState.probabilities[0], 1))
                {
                    /* Target is vanilla */
                    target.UpdateCakeState(0, 1, CakeState.Phase.Positive);
                }
                else if (Mathf.Approximately(targetCakeState.probabilities[1], 1))
                {
                    /* Target is chocolate */
                    target.UpdateCakeState(1, 0, CakeState.Phase.Positive);
                } /* Else: Target is 50-50 superposition do nothing */
            }
            else if (Mathf.Approximately(ctrlCakeState.probabilities[0], 0.5f)
              && Mathf.Approximately(ctrlCakeState.probabilities[1], 0.5f))
            {
                CakeState sharedCakeState;
                /* Ctrl: 50-50 superposition */
                if (Mathf.Approximately(targetCakeState.probabilities[0], 1))
                {
                    /* Target is vanilla */
                    sharedCakeState = new CakeState(0.5f, 0, 0, 0.5f, ctrlCakeState.phase);
                    control.UpdateCakeState(ref sharedCakeState);
                    target.UpdateCakeState(ref sharedCakeState);

                    target.bitInd = 1;
                }
                else if (Mathf.Approximately(targetCakeState.probabilities[1], 1))
                {
                    /* Target is chocolate */
                    sharedCakeState = new CakeState(0, 0.5f, 0.5f, 0, ctrlCakeState.phase);
                    control.UpdateCakeState(ref sharedCakeState);
                    target.UpdateCakeState(ref sharedCakeState);

                    target.bitInd = 1;
                } /* Else: Target is 50-50 superposition do nothing */
            } /* Else: Ctrl is Vanilla do nothing */
        }
    }
}
