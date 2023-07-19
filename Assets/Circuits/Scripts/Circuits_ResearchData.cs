using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Circuits
{
    [System.Serializable]
    public class Circuits_ResearchData
    {
        public string Username = Wrapper.Events.GetPlayerResearchCode?.Invoke();
        public string SaveData = string.Empty;
    }
}
