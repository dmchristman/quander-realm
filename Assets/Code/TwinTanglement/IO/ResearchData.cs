using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Firebase research data

namespace Labyrinth {
    public class ResearchData
    {
        public string Username = Wrapper.Events.GetPlayerResearchCode?.Invoke();
        public string SaveData = string.Empty;

        public void UpdateResearchData(SaveData data) {
            SaveData = data.SDtoString();
            Debug.Log("Research: " + SaveData);
        }

    }
}
