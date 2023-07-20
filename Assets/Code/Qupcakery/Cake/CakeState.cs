using System;
using UnityEngine;
using UnityEngine.Assertions;

/*
 * Parent class for cake states: manages probability and measurements
 * Has nothing to do with representation
 */

namespace Qupcakery
{
    public class CakeState
    {
        public enum Phase
        {
            Positive, Negative
        }

        public int stateSize { get; private set; } // 2 for single state; 4 for 2-qubit state
        public float[] probabilities { get; set; } = new float[4];
        public Phase phase { get; private set; }

        /* Constructor */
        public CakeState()
        {
            stateSize = 0;
        }

        /* Constructor */
        public CakeState(float a, float b)
        {
            Assert.AreApproximatelyEqual(1, a + b);

            stateSize = 2;
            probabilities[0] = a;
            probabilities[1] = b;
            phase = Phase.Positive;
        }

        /* Constructor */
        public CakeState(float a, float b, Phase newPhase)
        {
            Assert.AreApproximatelyEqual(1, a + b);

            stateSize = 2;
            probabilities[0] = a;
            probabilities[1] = b;
            phase = newPhase;
        }

        /* Constructor */
        public CakeState(float a, float b, float c, float d,
            Phase newPhase)
        {
            Assert.AreApproximatelyEqual(1, a + b + c + d);

            stateSize = 4;
            probabilities[0] = a;
            probabilities[1] = b;
            probabilities[2] = c;
            probabilities[3] = d;
            phase = newPhase;
        }

        public void UpdateCakeState(int state)
        {
            stateSize = 2;
            probabilities[state] = 1;
            probabilities[1 - state] = 0;
            probabilities[2] = 0;
            probabilities[3] = 0;
            phase = Phase.Positive;
        }

        public void UpdateCakeState(float a, float b)
        {
            Assert.AreApproximatelyEqual(1, a + b);

            stateSize = 2;
            probabilities[0] = a;
            probabilities[1] = b;
            probabilities[2] = 0;
            probabilities[3] = 0;
            phase = Phase.Positive;
        }

        public void UpdateCakeState(float a, float b, Phase newPhase)
        {
            Assert.AreApproximatelyEqual(1, a + b);

            stateSize = 2;
            probabilities[0] = a;
            probabilities[1] = b;
            probabilities[2] = 0;
            probabilities[3] = 0;
            phase = newPhase;
        }

        public void UpdateCakeState(float a, float b, float c, float d,
            Phase newPhase)
        {
            Assert.AreApproximatelyEqual(1, a + b + c + d);

            stateSize = 4;
            probabilities[0] = a;
            probabilities[1] = b;
            probabilities[2] = c;
            probabilities[3] = d;
            phase = newPhase;
        }

        public void FlipPhase()
        {
            phase = phase == CakeState.Phase.Negative ?
                    CakeState.Phase.Positive : CakeState.Phase.Negative;
        }

        /* Measure the cake and collapse the state, only support bell-states for 
         * 2-qubit system */
        public CakeType Measure(int bitInd)
        {
            int pickedState = MeasureHelper();
            collapseState(pickedState);
            switch (stateSize)
            {
                case 2:
                    return pickedState == 0 ? CakeType.Vanilla : CakeType.Chocolate;
                case 4:
                    if (bitInd == 0)
                    {
                        if (pickedState == 0 || pickedState == 1)
                            return CakeType.Vanilla;
                        else return CakeType.Chocolate;
                    }
                    else // bitInd == 1
                    {
                        if (pickedState == 0 || pickedState == 2)
                            return CakeType.Vanilla;
                        else return CakeType.Chocolate;
                    }
            }

            return CakeType.Unkown;
        }

        /* Get a state based on state probabilities */
        private int MeasureHelper() { return RandomPick.Get(probabilities, stateSize); }

        /* Set state at target index to 1 and all others to 0 */
        private void collapseState(int index)
        {
            for (int i = 0; i < stateSize; i++) probabilities[i] = index == i ? 1 : 0;
            phase = Phase.Positive;
        }

        //public enum Phase
        //{
        //    Positive = 1, Negative = -1, NA = 0
        //}

        //// Number of cakes in entanglement
        //public int Size { get; private set; }
        //// Index of current bit in an entangled state
        //public int bitInd { get; protected set; } = 0; 

        //// Constructor
        //public CakeState(int size)
        //{
        //    Size = size; 
        //}

        //// Updates cake state 
        //public virtual void SetCakeState(CakeType cakeType)
        //{
        //    throw new ArgumentException("SetCakeState: to be implemented");
        //}

        //// Updates cake state 
        //public virtual void SetCakeState(CakeType cakeType0, float cakeType0Prob,
        //    CakeType cakeType1, float cakeType1Prob, Phase phase)
        //{
        //    throw new ArgumentException("SetCakeState: to be implemented");
        //}

        //// Measures the cake state
        //public virtual CakeType Measure()
        //{
        //    return CakeType.Unkown;
        //}

        //// Check if the cake is in entanglement 
        //public virtual bool IsInEntanglement()
        //{
        //    return false;
        //    //throw new ArgumentException("IsInEntanglement: to be implemented");
        //}

        //// Returns the measurement probability of the given cake type
        //public virtual float GetMeasurementProbability(CakeType cakeType)
        //{
        //    throw new ArgumentException("This cake is in entanglement. " +
        //        "Cannot retrieve single cake measurement probability.");
        //}

        //public virtual Phase GetPhase()
        //{
        //    throw new ArgumentException("GetPhase: to be implemented");
        //}

        //public virtual CakeState Clone()
        //{
        //    throw new ArgumentException("Clone: to be implemented");
        //}

        public bool ParamEquals(int state)
        {
            return Mathf.Approximately(probabilities[state], 1);
        }

        public bool ParamEquals(float prob0, float prob1, Phase p)
        {
            return Mathf.Approximately(probabilities[0], prob0)
                && Mathf.Approximately(probabilities[1], prob1)
                && p == phase;
        }

        public bool ParamEquals(float prob0, float prob1,
            float prob2, float prob3, Phase p)
        {
            return Mathf.Approximately(probabilities[0], prob0)
                && Mathf.Approximately(probabilities[1], prob1)
                && Mathf.Approximately(probabilities[2], prob2)
                && Mathf.Approximately(probabilities[3], prob3)
                && p == phase;
        }

        public override bool Equals(object obj)
        {
            CakeState target = (CakeState)obj;
            for (int i = 0; i < 4; i++)
            {
                if (!(Math.Abs(probabilities[i] - target.probabilities[i])
                    < Constants.FloatCmpMargin))
                {
                    return false;
                }
            }
            if (target.phase != phase)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return (int)(probabilities[0] + probabilities[1] * 3
                + probabilities[2] * 9 + probabilities[3] * 27 + (int)phase);
        }

        public override string ToString()
        {
            return "CakeState: (" + probabilities[0] + ", "
                        + probabilities[1] + ", " + probabilities[2] + ", " +
                        probabilities[3] + "), phase: " + phase;
        }
    }
}