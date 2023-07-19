using System;

// Firebase non-research load/save data

namespace QueueBits
{
    [System.Serializable]
    public class QBSaveData
    {
        public Wrapper.Game gameID = Wrapper.Game.QueueBits;

        // star system testing
        public int[] starSystem = new int[]
        {
            0, // level select
            0, // level 1
            0, // level 2
            0, // level 3
            0, // level 4
            0, // level 5
            0, // level 6
            0, // level 7
            0, // level 8
            0, // level 9
            0, // level 10
            0, // level 11
            0, // level 12
            0, // level 13
            0, // level 14
            0, // level 15
        };

        // dialogue system
        public bool[] dialogueSystem = new bool[]
        {
            true, // level select
            true, // level 1
            true, // level 2
            true, // level 3
            true, // level 4
            true, // level 5
            true, // level 6
            true, // level 7
            true, // level 8
            true, // level 9
            true, // level 10
            true, // level 11
            true, // level 12
            true, // level 13
            true, // level 14
            true, // level 15
        };
    }
}
