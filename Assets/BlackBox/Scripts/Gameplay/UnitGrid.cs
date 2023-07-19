using UnityEngine;

namespace BlackBox
{
    public class UnitGrid : MonoBehaviour
    {
        [SerializeField] private GameObject navCellPrefab = null;

        private Dir direction = Dir.None;
        private NavCell[,] cellArray = null;

        private void OnEnable()
        {
            BBEvents.MarkUnits += MarkUnit; //todo: creating new grids re-registers
            BBEvents.MarkDetourUnits += MarkLinkedUnits; //todo: creating new grids re-registers
        }

        private void OnDisable()
        {
            BBEvents.MarkUnits -= MarkUnit;
            BBEvents.MarkDetourUnits -= MarkLinkedUnits;            
        }

        public void Create(int width, int height, Dir direction = Dir.None)
        {
            this.direction = direction;
            cellArray = new NavCell[width, height];

            for (int y = 0; y < cellArray.GetLength(1); y++)
            {
                for (int x = 0; x < cellArray.GetLength(0); x++)
                {
                    GameObject navCellObj = Instantiate(navCellPrefab, gameObject.transform);
                    NavCell navCell = navCellObj.GetComponent<NavCell>();

                    navCell.Create(x, y, CellType.Nav, direction);
                    cellArray[x, y] = navCell;
                }
            }

            // set Molly on first slot
            if (direction == Dir.Top) cellArray[0, 0].EnableMolly(false);
        }

        private void MarkUnit(Marker marker, Dir gridDirection, Vector3Int destPosition, bool destination)
        {
            if (direction != gridDirection)
                return;

            if (destination) cellArray[destPosition.x, destPosition.y].SetDelayedValue(marker);
            else cellArray[destPosition.x, destPosition.y].SetValue(marker);
        }

        private void MarkLinkedUnits(Dir entryDirection, Vector3Int entryPosition, Dir exitDirection, Vector3Int exitPosition, int detourPairNumber)
        {
            //BBEvents.DelayReaction?.Invoke(() =>
            //{
                //Entry Cell
                if (entryDirection == direction)
                    cellArray[entryPosition.x, entryPosition.y].SetValue(Marker.Detour, detourPairNumber, exitDirection, exitPosition);

                //Exit Cell
                if (exitDirection == direction)
                    cellArray[exitPosition.x, exitPosition.y].SetDelayedValue(Marker.Detour, detourPairNumber, entryDirection, entryPosition);
            //});
        }
    }
}
