using System;
using UnityEngine;
using SysDebug = System.Diagnostics.Debug;
using CakeUtil = Qupcakery.CakeStateUtilities;

namespace Qupcakery
{
    public class NotGate : Gate
    {
        public NotGate() : base(GateType.NOT, 1)
        {
            return;
        }

        // Implement NOT operation
        public override void ExecuteGate(ref Cake cake)
        {
            CakeState cakeState = cake.cakeState;

            switch (cakeState.stateSize)
            {
                case 2:
                    cake.UpdateCakeState(cakeState.probabilities[1], cakeState.probabilities[0],
                        cakeState.phase);
                    break;
                case 4:
                    /* [0.5, 0, 0, 0.5] <-> [0, 0.5, 0.5, 0] */
                    cake.UpdateCakeState(cakeState.probabilities[1], cakeState.probabilities[0],
                        cakeState.probabilities[3], cakeState.probabilities[2], cakeState.phase);
                    break;
                default:
                    throw new ArgumentException("Invalid state size!");
            }

        }
    }
}
