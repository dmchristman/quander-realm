using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wrapper
{
    public class RewardSaveObject
    {
        public string Username = Wrapper.Events.GetPlayerResearchCode?.Invoke();
        public string RewardData = string.Empty;

        public void UpdateResearchData(RewardResearchData RRD) {
            RewardData = RRD.RewardRDtoString();
            Debug.Log("Research: " + RewardData);
        }
        
    }
}