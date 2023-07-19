using System.Collections.Generic;
using UnityEngine;
using SysDebug = System.Diagnostics.Debug;

/* Keeps track of cake gameobjects on belts, only used for experimental mode*/
namespace Qupcakery
{
    public class CakeOnBeltTracker
    {
        // Publisher for experiment mode
        public delegate void CakesReadyToBeDeliveredHandler();
        public CakesReadyToBeDeliveredHandler CakesReadyToBeDelivered;
        public delegate void CakesRemovedFromBeltHandler();
        public CakesRemovedFromBeltHandler CakesRemovedFromBelt;

        public static CakeOnBeltTracker Instance
        {
            get { return Nested.instance; }
            private set { }
        }

        private class Nested
        {
            static Nested() { }

            internal static CakeOnBeltTracker instance = new CakeOnBeltTracker();
        }

        private int totalBeltCnt;

        private Dictionary<int, GameObject> cakeTracker
             = new Dictionary<int, GameObject>();

        public void InitializeTracker(int beltCnt)
        {
            totalBeltCnt = beltCnt;

            if (GameManagement.Instance.gameMode
                == GameManagement.GameMode.Experiment)
            {
                if (CakesReadyToBeDelivered != null)
                {
                    foreach (CakesReadyToBeDeliveredHandler d
                        in CakesReadyToBeDelivered.GetInvocationList())
                    {
                        CakesReadyToBeDelivered -= d;
                    }
                }

                if (CakesRemovedFromBelt != null)
                {
                    foreach (CakesRemovedFromBeltHandler d
                    in CakesRemovedFromBelt.GetInvocationList())
                    {
                        CakesRemovedFromBelt -= d;
                    }
                }
            }

            for (int i = 0; i < beltCnt; i++)
            {
                cakeTracker[i] = null;
            }
        }

        public void AddCakeToBelt(GameObject cake, int beltInd)
        {
            SysDebug.Assert(cakeTracker[beltInd] == null);

            cakeTracker[beltInd] = cake;

            /* In experiment mode, notify button that it can be pressed
            if every belt has a cake */
            if (GameManagement.Instance.gameMode == GameManagement.GameMode.Experiment)
            {
                for (int i = 0; i < totalBeltCnt; i++)
                {
                    if (cakeTracker[i] == null)
                        return;
                }
                OnCakesReadyToBeDelivered();
            }
        }

        public GameObject GetCakeFromBelt(int beltInd)
        {
            // Debug.Log("Getting cake " + cakeTracker[beltInd] + " from belt "+beltInd);
            return GameObjectsManagement.CakeBoxes[beltInd];
        }

        public void RemoveCakeFromBelt(int beltInd)
        {
            cakeTracker[beltInd] = null;
            if (GameManagement.Instance.gameMode == GameManagement.GameMode.Experiment)
            {
                OnCakesRemovedFromBelt();
            }
        }

        public void RemoveCakesFromBelt()
        {
            for (int i = 0; i < totalBeltCnt; i++)
            {
                cakeTracker[i] = null;

            }
        }

        public List<GameObject> GetCakesInList()
        {
            List<GameObject> cakes = new List<GameObject>();
            foreach (int key in cakeTracker.Keys)
            {
                cakes.Add(cakeTracker[key]);
            }
            return cakes;
        }

        // Event publisher
        protected virtual void OnCakesReadyToBeDelivered()
        {
            if (CakesReadyToBeDelivered != null)
            {
                CakesReadyToBeDelivered();
            }
        }

        // Event publisher
        protected virtual void OnCakesRemovedFromBelt()
        {
            if (CakesRemovedFromBelt != null)
            {
                CakesRemovedFromBelt();
            }
        }
    }
}
