using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Qupcakery
{
    public static class RewardManager
    {
        public static Dictionary<int, string> rewardsInd = new Dictionary<int, string>
        {
            { 1, "QU_03" },
            { 5, "QU_05" },
            { 9, "QU_01" },
            { 13, "QU_06" },
            { 15, "QU_08" },
            { 20, "QU_07" },
            { 23, "QU_04" },
            { 27, "QU_02" }
        };
    }
}
