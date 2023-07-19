using System;
using SysDebug = System.Diagnostics.Debug;

/* 
 * Gate description + execution logic 
 */
namespace Qupcakery
{
    public enum GateType
    {
        None,
        NOT = 0, CNOT = 1, SWAP = 2, H = 3, Z = 4
    }

    public class Gate
    {
        public GateType Type { get; private set; }
        public int Size { get; private set; }

        // Constructor
        public Gate(GateType gateType, int gateSize)
        {
            Type = gateType;
            Size = gateSize;
        }

        // Perform gate (size 1) logic on cake
        public virtual void ExecuteGate(ref Cake cake)
        {
            SysDebug.Assert(Size == 1);
            SysDebug.WriteLine("INFO: gate logic not yet implemented");
            return;
        }

        // Perform gate (size 2) logic on cakes
        public virtual void ExecuteGate(ref Cake cake0, ref Cake cake1)
        {
            SysDebug.Assert(Size == 2);
            SysDebug.WriteLine("INFO: gate logic not yet implemented");
            return;
        }
    }
}
