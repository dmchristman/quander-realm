using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/* Manages gate slots */
namespace Qupcakery
{
    public class GateSlots
    {
        public static GateSlots Instance { get; private set; } =
            new GateSlots();
        public int[,] Solution = new int[Constants.MaxBeltPerBatch, Constants.MaxGatePerBelt]; // 0 -> none, 1 -> NOT,...

        private bool[,] slots = new bool[Constants.MaxBeltPerBatch, Constants.MaxGatePerBelt]; // true -> available

        int maxGatesPerBelt;
        int beltCnt;
        float distanceBetweenBelts;
        float distanceBetweenSlots = 1.75f;
        float slotLeftMostX = -2.5f;

        // Set all slots to be available
        public void InitializeSlots(int newBeltCnt, int newMaxGatesPerBelt)
        {
            beltCnt = newBeltCnt;
            maxGatesPerBelt = newMaxGatesPerBelt;
            if (beltCnt == 1)
                distanceBetweenBelts = 1.5f; // Margin for checking if the gate is placed on belt
            else
                distanceBetweenBelts = SetupUtilities.BeltPosition(beltCnt, 1).y - SetupUtilities.BeltPosition(beltCnt, 0).y;

            for (int i = 0; i < beltCnt; i++)
            {
                for (int j = 0; j < maxGatesPerBelt; j++)
                {
                    slots[i, j] = true;
                    Solution[i, j] = 0;
                }
            }
        }

        // place gate in slot if avaliable
        public bool PlaceGateInSlot(GameObject gateObject,
            Vector2 gatePosition, List<int> beltInd)
        {
            GateOperationController gc = gateObject.
                GetComponent<GateOperationController>();
            int gateSize = gc.gate.Size;
            GateType gt = gc.gate.Type;
            List<(int, int)> targetSlotsIndex = GetSlotIndex(gatePosition, gateSize);
            bool slotsAvailable = SlotsAreAvailable(targetSlotsIndex);

            if (!slotsAvailable)
                return false;
            else
            {
                Vector2 position = new Vector2();
                foreach ((int, int) slotInd in targetSlotsIndex)
                {
                    position += GetSlotPosition(slotInd);
                    beltInd.Add(slotInd.Item1);
                    // Set slot to be unavailable
                    slots[slotInd.Item1, slotInd.Item2] = false;

                    Solution[slotInd.Item1, slotInd.Item2] = (int)gt + 1;
                }

                // Set CNOT ctl to 10
                if (gt == GateType.CNOT)
                {
                    bool ctrlTgtSwapped = gc.ctrlTgtSwapped;
                    int ctrlXInd = ctrlTgtSwapped ?
                        Math.Min(targetSlotsIndex[0].Item1, targetSlotsIndex[1].Item1)
                        : Math.Max(targetSlotsIndex[0].Item1, targetSlotsIndex[1].Item1);
                    Solution[ctrlXInd, targetSlotsIndex[0].Item2] = 7;
                }

                position = new Vector2(position.x / gateSize, position.y / gateSize);
                gateObject.transform.position = position;
                return true;
            }
        }

        // Release occupied slots for current gate
        public void RemoveGateFromSlot(GameObject gateObject,
            Vector2 gatePosition)
        {
            System.Diagnostics.Debug.Assert(gateObject != null);
            int gateSize = gateObject.GetComponent<GateOperationController>().gate.Size;
            foreach ((int, int) slotInd in GetSlotIndex(gatePosition, gateSize))
            {
                slots[slotInd.Item1, slotInd.Item2] = true;
                Solution[slotInd.Item1, slotInd.Item2] = 0;
            }
        }

        // Calculate slot position based on index
        private Vector2 GetSlotPosition((int, int) slotIndex)
        {
            Vector2 position = new Vector2();

            int beltIndex = slotIndex.Item1;
            int slotIndexX = slotIndex.Item2;
            position.x = GetSlotX(slotIndexX);
            position.y = GetSlotY(beltIndex);

            return position;
        }

        // Get slot index for given position
        private List<(int, int)> GetSlotIndex(Vector2 position, int gateSize)
        {
            int colInd = SlotIndex(position.x, distanceBetweenSlots, maxGatesPerBelt);
            List<int> rowInd = SlotBeltIndex(position.y, gateSize, distanceBetweenBelts);

            // If no valid slot is found
            if (colInd == -1 || rowInd.Count == 0)
                return null;
            else
            {
                List<(int, int)> targetSlots = new List<(int, int)>();
                foreach (int row in rowInd)
                {
                    targetSlots.Add((row, colInd));
                }
                return targetSlots;
            }
        }

        // Check whether the given slots are available
        private bool SlotsAreAvailable(List<(int, int)> targetSlots)
        {
            if (targetSlots == null || targetSlots.Count == 0)
                return false;

            foreach ((int, int) slotIndex in targetSlots)
            {
                if (!slots[slotIndex.Item1, slotIndex.Item2])
                    return false;
            }
            return true;
        }

        // Given position.x, return the slot col-index
        // Return -1 if the position doesn't match any index
        private int SlotIndex(float x, float distanceBetweenSlots,
            float maxGatesPerBelt)
        {
            int index = (int)((x - (slotLeftMostX - 0.5f * distanceBetweenSlots)) / distanceBetweenSlots);

            // if index out of range
            if (index < 0 || index >= maxGatesPerBelt)
                return -1;
            return index;
        }

        // Calculate belt indices corresbonding to the y position 
        private List<int> SlotBeltIndex(float y, int GateSize,
            float distanceBetweenBelt)
        {
            List<int> ys = new List<int> { };
            float margin = distanceBetweenBelt / 2f, beltY;

            // Loop through all belts to check whether the y position lies within the belt y-position margin
            for (int i = 0; i < beltCnt; i++)
            {
                // Get y position of belt i
                beltY = SetupUtilities.BeltPosition(beltCnt, i).y;
              
                switch (GateSize)
                {
                    case 1:
                        if (y > beltY - margin && y < beltY + margin)
                        {
                            ys.Add(i);
                        }
                        break;
                    case 2:
                        if (i == beltCnt - 1)
                            break;
                        else
                        {
                            float nextBeltY = SetupUtilities.BeltPosition(beltCnt, i + 1).y;
                            if (y > beltY && y < nextBeltY)
                            {
                                ys.Add(i);
                                ys.Add(i + 1);
                                // Debug.LogFormat("The gate is on belts {0}, {1}", i, i + 1);
                                return ys;
                            }
                        }
                        break;
                    default:
                        throw new System.ArgumentException("SlotBeltIndex: Invaide gate size.");
                }
            }
            return ys;
        }

        float GetSlotX(int slotIndexX)
        {
            return slotLeftMostX + slotIndexX * distanceBetweenSlots;
        }

        float GetSlotY(int beltIndex)
        {
            return SetupUtilities.BeltPosition(beltCnt, beltIndex).y;
        }
    }
}