using System;
using System.Collections.Generic;
using UnityEngine;

/* Manages cake slots for experiement mode*/
namespace Qupcakery
{
    public class CakeSlots
    {
        public static CakeSlots Instance { get; private set; } =
            new CakeSlots();

        private int beltCnt;
        private Dictionary<int, bool> slotStatus
            = new Dictionary<int, bool>(); // true -> available

        // Initialize
        public void InitializeCakeSlots(int totalBeltCnt)
        {
            slotStatus.Clear();
            beltCnt = totalBeltCnt;
            for (int i = 0; i < beltCnt; i++)
            {
                slotStatus[i] = true;
            }
        }

        // Place cake in slot
        // if slot available, returns true; otherwise returns false
        public bool PlaceCakeInSlot(GameObject cakeObject,
            Vector2 cakePosition)
        {
            for (int i = 0; i < beltCnt; i++)
            {
                if (slotStatus[i])
                {
                    Vector2 targetPosition =
                        GameUtilities.GetCakeStartPosition(i, (int)beltCnt);
                    if (CakeIsNearTargetSlot(cakePosition, targetPosition))
                    {
                        cakeObject.transform.position = targetPosition;
                        slotStatus[i] = false;
                        CakeOnBeltTracker.Instance.AddCakeToBelt(cakeObject, i);
                        return true;
                    }
                }
            }
            return false;
        }

        // Remove cake from slot and release the slot
        public bool RemoveCakeFromSlot(GameObject cakeObject,
            Vector2 cakePosition)
        {
            for (int i = 0; i < beltCnt; i++)
            {
                if (!slotStatus[i])
                {
                    Vector2 targetPosition =
                        GameUtilities.GetCakeStartPosition(i, (int)beltCnt);
                    if (CakeIsNearTargetSlot(cakePosition, targetPosition))
                    {
                        slotStatus[i] = true;
                        CakeOnBeltTracker.Instance.RemoveCakeFromBelt(i);
                        return true;
                    }
                }
            }
            return false;
        }

        // Check if the cake position is near a slot
        private bool CakeIsNearTargetSlot(Vector2 position, Vector2 slotPosition)
        {
            float dis = Vector2.Distance(position, slotPosition);
            // Debug.Log("Distance is " + dis);
            if (dis < 1f)
                return true;
            else
                return false;
        }
    }
}
