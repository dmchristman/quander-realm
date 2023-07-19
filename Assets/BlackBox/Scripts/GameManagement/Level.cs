using UnityEngine;

namespace BlackBox
{
    [CreateAssetMenu(fileName = "Level", menuName = "Level")]
    public class Level : ScriptableObject
    {
        public string levelID;
        public int number;
        public int tutorialNumber;
        public string nextLevelID;

        [Range(10, 25)] public int numEnergyUnits;
        public GridSize gridSize;
        public Vector2Int[] nodePositions;
    }
}
