using System;
using UnityEngine;
using SysDebug = System.Diagnostics.Debug;
using CakeUtil = Qupcakery.CakeStateUtilities;

namespace Qupcakery
{
    public class ZGate : Gate
    {
        public ZGate() : base(GateType.Z, 1)
        {
            return;
        }

        // Implements Z operation
        public override void ExecuteGate(ref Cake cake)
        {
            CakeState cakeState = cake.cakeState;

            switch (cakeState.stateSize)
            {
                case 2:
                    if (Math.Abs(cakeState.probabilities[0] - 0.5f) < Constants.FloatCmpMargin)
                        cake.UpdateCakeStateFlipPhase();
                    break;
                case 4:
                    cake.UpdateCakeStateFlipPhase();
                    break;
                default:
                    throw new ArgumentException("Invalid state size!");
            }
        }
    }
}
