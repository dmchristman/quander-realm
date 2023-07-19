using System;
using SysDebug = System.Diagnostics.Debug;

/*
 * Timer management
 */
namespace Qupcakery
{
    public class Timer
    {
        // Timer click event
        public delegate void TimerClickedEventHandler(float totalTime, float remainingTime);
        public event TimerClickedEventHandler TimerClicked;

        // Timer end event
        public delegate void TimerEndedEventHandler();
        public event TimerEndedEventHandler TimerEnded;

        // Timer interval-end alert event
        public delegate void TimerIntervalEndedEventHandler(object source, EventArgs args);
        public event TimerIntervalEndedEventHandler TimerIntervalEnded;

        public float TotalTime { get; private set; }
        public float RemainingTime { get; private set; }
        public float ElapsedTime { get { return TotalTime - RemainingTime; } }
        public float AlertInterval { get; set; }

        private float intervalAlertCnt = 0;

        public Timer(float newTotalTime)
        {
            TotalTime = newTotalTime;
            RemainingTime = TotalTime;
            AlertInterval = TotalTime;
        }

        // Ticks down the timer by input amount
        public void Tick(float deltaTime)
        {
            SysDebug.Assert(deltaTime >= 0f);

            RemainingTime -= deltaTime;
            OnTimerClicked(TotalTime, Math.Max(RemainingTime, 0f));

            // If timer has ended
            if (RemainingTime <= 0f)
            {
                RemainingTime = 0f;

                // Raise timer end event
                OnTimerEnded();
            }

            if (Math.Floor(ElapsedTime / AlertInterval) >= intervalAlertCnt)
            {
                intervalAlertCnt++;
                OnTimerIntervalEnded();
            }
        }

        // Notifies all timer subscribers
        protected virtual void OnTimerClicked(float totalTime, float remainingTime)
        {
            if (TimerClicked != null)
                TimerClicked(totalTime, remainingTime);
        }

        // Notifies all timer subscribers
        protected virtual void OnTimerEnded()
        {
            // Checks that there are at least 1 subscriber
            if (TimerEnded != null)
                TimerEnded();
        }

        // Notifies interval subscribers
        protected virtual void OnTimerIntervalEnded()
        {
            if (TimerIntervalEnded != null)
                TimerIntervalEnded(this, EventArgs.Empty);
        }
    }
}
