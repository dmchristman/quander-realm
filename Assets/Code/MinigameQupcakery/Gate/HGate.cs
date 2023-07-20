using System;

namespace Qupcakery
{
    public class HGate : Gate
    {
        // Constructor
        public HGate() : base(GateType.H, 1)
        {
        }

        public override void ExecuteGate(ref Cake cake)
        {
            CakeState cakeState = cake.cakeState;

            /* forbid operation on states in entanglement */
            System.Diagnostics.Debug.Assert(cakeState.stateSize == 2);

            if (cakeState.probabilities[0] > 1 - Constants.FloatCmpMargin
                && cakeState.probabilities[0] < 1 + Constants.FloatCmpMargin)
            {
                cake.UpdateCakeState(0.5f, 0.5f, CakeState.Phase.Positive);
            }
            else if (cakeState.probabilities[1] > 1 - Constants.FloatCmpMargin
              && cakeState.probabilities[0] < 1 + Constants.FloatCmpMargin)
            {
                cake.UpdateCakeState(0.5f, 0.5f, CakeState.Phase.Negative);
            }
            else
            {
                /* 50-50 superposition */
                if (cakeState.phase == CakeState.Phase.Positive)
                    cake.UpdateCakeState(1, 0, CakeState.Phase.Positive);
                else
                    cake.UpdateCakeState(0, 1, CakeState.Phase.Positive);
            }
        }
    }
}
