using System;

/*
 * Manages customer patience
 */
namespace Qupcakery
{
    public class Patience
    {
        public delegate void OnPatienceEndedHandler();
        public event OnPatienceEndedHandler PatienceEnded;

        public delegate void OnPatienceUpdatedHandler(float remainingRatio);
        public event OnPatienceUpdatedHandler PatienceUpdated;

        public int MaxPatience { get; }
        public float RemainingPatience { get; private set; }
        private int maxFreezeTime;
        private float remainingFreezeTime;

        // Constructor 
        public Patience(int maxPatience, int patienceFreezeTime)
        {
            MaxPatience = maxPatience;
            RemainingPatience = MaxPatience;
            maxFreezeTime = patienceFreezeTime;
            remainingFreezeTime = maxFreezeTime;
        }

        public void ResetPatience()
        {
            RemainingPatience = MaxPatience;
            remainingFreezeTime = maxFreezeTime;
            OnPatienceUpdated((float)RemainingPatience / (float)MaxPatience);
        }

        // Decreases patience by delta amount
        // If freeze time > 0, decrease freeze time first
        public void DecreasePatience(float deltaPatience)
        {
            if (remainingFreezeTime > 0)
            {
                remainingFreezeTime -= deltaPatience;

                if (remainingFreezeTime >= 0)
                    return;
                else
                {
                    RemainingPatience += remainingFreezeTime;
                    remainingFreezeTime = 0;
                    OnPatienceUpdated((float)RemainingPatience / (float)MaxPatience);
                    return;
                }
            }

            // If freeze time run out
            RemainingPatience -= deltaPatience;

            if (RemainingPatience <= 0)
            {
                RemainingPatience = 0;
                OnPatienceUpdated((float)RemainingPatience / (float)MaxPatience);
                OnPatienceEnded();
            }

            OnPatienceUpdated((float)RemainingPatience / (float)MaxPatience);
        }

        //// Increases patience by delta amount 
        //private void IncreasePatience(int deltaPatience)
        //{
        //    RemainingPatience += deltaPatience;

        //    if (RemainingPatience > MaxPatience)
        //        RemainingPatience = MaxPatience;
        //}

        // Notifies all patience subscribers
        protected virtual void OnPatienceEnded()
        {
            if (PatienceEnded != null)
                PatienceEnded();
        }

        // Notifies all patience subscribers
        protected virtual void OnPatienceUpdated(float remainingRatio)
        {
            if (PatienceUpdated != null)
                PatienceUpdated(remainingRatio);
        }
    }
}
