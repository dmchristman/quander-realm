using System;
using UnityEngine;
using SysDebug = System.Diagnostics.Debug;

/*
 * Dispatch puzzles
 */
namespace Qupcakery
{
    public class Dispatcher
    {
        // Event publisher
        public delegate void BatchDoneHandler();
        public BatchDoneHandler BatchDonePublisher;
        public delegate void AllBatchDoneHandler();
        public AllBatchDoneHandler AllBatchDonePublisher;
        public delegate void NewBatchDispatchHandler();
        public NewBatchDispatchHandler NewBatchDispatchedPublisher;

        public int NextBatchInd { get; private set; } = 0;

        private Level level;

        // Constructor
        public Dispatcher(Level currLevel, Timer newTimer)
        {
            level = currLevel;

            /* Subscribe to 1st Customer-batch-done event */
            CustomerManager cm = GameObjectsManagement.Customers[0]
                .GetComponent<CustomerManager>();
            cm.RemoveBatchEndedListeners();
            cm.BatchEnded += OnBatchDone;
        }

        // Subscriber to customer reaction
        // Batch is done once the first customer has left the scene
        public void OnBatchDone()
        {
            OnBatchDonePublisher();

            /* Reset all active customers */
            for (int i = 0; i < level.TotalBeltCnt; i++)
            {
                GameObjectsManagement.ResetCustomerObj(
                    GameObjectsManagement.Customers[i]);
            }

            if (NextBatchInd < level.TotalPuzzleCnt)
            {
                DispatchNewBatch();
            }
            else
            {
                AllBatchDonePublisher();
            }
        }

        // OnBatchDone publisher
        protected virtual void OnBatchDonePublisher()
        {
            if (BatchDonePublisher != null)
            {
                BatchDonePublisher();
            }
        }

        // OnAllBatchDone publisher
        protected virtual void OnAllBatchDonePublisher()
        {
            if (AllBatchDonePublisher != null)
            {
                AllBatchDonePublisher();
            }
        }

        public void DispatchNewBatch()
        {
            DispatchPuzzle.Dispatch(level.Puzzles[NextBatchInd],
                GameObjectsManagement.Customers, GameObjectsManagement.CakeBoxes);

            if (NewBatchDispatchedPublisher != null) NewBatchDispatchedPublisher();
            NextBatchInd++;
        }
    }
}
