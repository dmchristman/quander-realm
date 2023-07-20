using System;
using UnityEngine;
using SysDebug = System.Diagnostics.Debug;
using CakeUtil = Qupcakery.CakeStateUtilities;

/*
 * Cake object for interfacing with Unity
 * Update representation based on cakestate
 * 
 */

namespace Qupcakery
{
    // Cake types for interfacing with game
    public enum GameCakeType : short
    {
        Vanilla = 0, Chocolate = 1,
        Vanilla50_Chocolate50 = 2, Vanilla50_Chocolate50_Neg = 3,
        Unkown
    }

    // Cake types
    public enum CakeType : short
    {
        Vanilla = 0, Chocolate = 1, Unkown
    }

    public class Cake
    {
        public CakeState cakeState { get; private set; } = new CakeState();
        public int beltInd { get; private set; }
        public int bitInd { get; set; }
        public bool IsEntangled { get { return cakeState.stateSize == 4 ? true : false; } }

        public delegate void OnCakeStateUpdatedEventHandler();
        public event OnCakeStateUpdatedEventHandler CakeStateUpdated;

        // Constructor 
        public Cake()
        {
            bitInd = 0;
        }

        public void UpdateCakeState(int cakeSpec)
        {
            bitInd = 0; // assume no entanglement

            switch (cakeSpec)
            {
                case 0:
                    cakeState.UpdateCakeState(1, 0);
                    break;
                case 1:
                    cakeState.UpdateCakeState(0, 1);
                    break;
                case 2:
                    cakeState.UpdateCakeState(0.5f, 0.5f, CakeState.Phase.Positive);
                    break;
                case 3:
                    cakeState.UpdateCakeState(0.5f, 0.5f, CakeState.Phase.Negative);
                    break;
                default:
                    throw new ArgumentException("Invalid cakeSpec index: " + cakeSpec);
            }
            OnCakeStateUpdated();
        }

        // Reset to empty cake state
        public void ResetCakeState()
        {
            cakeState = new CakeState();
        }

        public void UpdateCakeState(ref CakeState newCakeState)
        {
            cakeState = newCakeState;
            OnCakeStateUpdated();
        }

        public void UpdateCakeState(CakeState newCakeState)
        {
            cakeState = newCakeState;
            OnCakeStateUpdated();
        }

        public void UpdateCakeState(float a, float b, CakeState.Phase newPhase)
        {
            cakeState.UpdateCakeState(a, b, newPhase);
            OnCakeStateUpdated();
        }

        public void UpdateCakeState(float a, float b, float c, float d,
            CakeState.Phase newPhase)
        {
            cakeState.UpdateCakeState(a, b, c, d, newPhase);
            OnCakeStateUpdated();
        }

        public void UpdateCakeStateFlipPhase()
        {
            cakeState.FlipPhase();
            OnCakeStateUpdated();
        }

        protected virtual void OnCakeStateUpdated()
        {
            if (CakeStateUpdated != null) CakeStateUpdated();
        }

        public void SetBeltInd(int ind) { beltInd = ind; }

        // Measures the cake, collapses the state, and return the result
        public GameCakeType MeasureCake()
        {
            CakeType cakeType = cakeState.Measure(bitInd);
            switch (cakeType)
            {
                case CakeType.Vanilla:
                    return GameCakeType.Vanilla;
                case CakeType.Chocolate:
                    return GameCakeType.Chocolate;
                default:
                    throw new ArgumentException("Unimplemented cake");
            }
        }
    }
}
