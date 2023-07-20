using System;
using UnityEngine;
using SysDebug = System.Diagnostics.Debug;


namespace Qupcakery
{
    public static class CakeStateUtilities
    {
        // Get opposite caketype
        public static CakeType GetOppositeType(CakeType cakeType)
        {
            switch (cakeType)
            {
                case CakeType.Vanilla:
                    return CakeType.Chocolate;
                case CakeType.Chocolate:
                    return CakeType.Vanilla;
                default:
                    throw new ArgumentException("GetOppositeType: Invalid cake type");
            }
        }

        // Entangle two cakes
        public static void EntangleCakes(Cake cake0, Cake cake1)
        {

        }

    }
}
