using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Circuits
{
    [System.Serializable]
	public class Circuits_SaveData
    {
        public Wrapper.Game gameID = Wrapper.Game.Circuits;
        public int currLevel = 0;
        public bool[] completedLevels = new bool[CTConstants.N_LEVELS];
    }
}

