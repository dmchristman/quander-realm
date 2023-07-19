using System;
using SysDebug = System.Diagnostics.Debug;

namespace Qupcakery
{
    public class SwapGate : Gate
    {
        public SwapGate() : base(GateType.SWAP, 2)
        {
            return;
        }

        // Implements SWAP operation
        public override void ExecuteGate(ref Cake cake0, ref Cake cake1)
        {
            /* If cake0 and cake1 are entangled, then swap doesn't do anything */
            if (cake0.cakeState.stateSize == 4
                && cake1.cakeState.stateSize == 4)
                return;

            CakeState tmp = cake0.cakeState;
            cake0.UpdateCakeState(cake1.cakeState);
            cake1.UpdateCakeState(tmp);
            (cake0.bitInd, cake1.bitInd) = (cake1.bitInd, cake0.bitInd);
        }
    }
}