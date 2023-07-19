using System;
using UnityEngine;

namespace BlackBox
{
    public class MainGrid : MonoBehaviour
    {
        [SerializeField] private float debugLineDuration = 3f;
        [SerializeField] private GameObject nodeCellPrefab = null;

        private int width = 0;
        private int height = 0;
        private int energyUnits = 0;
        private Dir direction = Dir.None;
        private Ray ray = null;
        private NodeCell[,] cellArray = null;

        [SerializeField]
        float swapDelayTime = 0.8F;
        [SerializeField]
        float fogDelayTime = 2F;
        [SerializeField]
        float fogReactionDelayTime = 2F;
        BeauRoutine.Routine interactionDelayer;

        private void OnEnable()
        {
            BBEvents.FireRay += FireRay;
            BBEvents.ToggleFlag += ToggleFlag;
            if (Wrapper.Events.IsDebugEnabled.Invoke()) BBEvents.ClearMarkers += ResetEnergy; // Debug
            BBEvents.IsInteractionDelayed += IsDelayed;
            BBEvents.DelayInteraction += DelayInteraction;
            BBEvents.DelayReaction += DelayReaction;
            BBEvents.LanternPlacedCount += GetLanternsOnGridCount;
        }

        private void OnDisable()
        {
            BBEvents.FireRay -= FireRay;
            BBEvents.ToggleFlag -= ToggleFlag;
            if (Wrapper.Events.IsDebugEnabled.Invoke()) BBEvents.ClearMarkers -= ResetEnergy; // Debug
            BBEvents.IsInteractionDelayed -= IsDelayed;
            BBEvents.DelayInteraction -= DelayInteraction;
            BBEvents.DelayReaction -= DelayReaction;
            BBEvents.LanternPlacedCount -= GetLanternsOnGridCount;
        }

        public void Create(int width, int height, int numEnergyUnits)
        {
            this.width = width;
            this.height = height;
            energyUnits = numEnergyUnits;
            cellArray = new NodeCell[width, height];

            for (int y = 0; y < cellArray.GetLength(1); y++)
            {
                for (int x = 0; x < cellArray.GetLength(0); x++)
                {
                    GameObject nodeCellObj = Instantiate(nodeCellPrefab, gameObject.transform);
                    NodeCell nodeCell = nodeCellObj.GetComponent<NodeCell>();

                    nodeCell.Create(x, y, GetCellType(x, y), direction);
                    cellArray[x, y] = nodeCell;
                }
            }
        }

        private CellType GetCellType(int x, int y)
        {
            CellType result = CellType.Node;

            if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                result = CellType.EdgeNode;

            return result;
        }

        private void ToggleFlag(Vector3Int gridPosition, bool toggle)
        {
            cellArray[gridPosition.x, gridPosition.y].ToggleFlag(toggle);
        }

        public void SetNodes(Vector2Int[] nodePositions)
        {
            foreach (Vector2Int position in nodePositions)
                cellArray[position.x, position.y].SetNode();
        }

        public int GetNumCorrect(Vector2Int[] nodePositions)
        {
            int numCorrect = 0;

            foreach(Vector2Int pos in nodePositions)
                if (cellArray[pos.x, pos.y].HasFlag())
                    numCorrect++;

            return numCorrect;
        }

        #region Node and Ray Behaviour

        private void FireRay(Vector3Int rayOrigin, Dir rayDirection)
        {
            if (energyUnits == 0)
            {
                BBEvents.IndicateEmptyMeter?.Invoke();
                return;
            }

            energyUnits--;
            BBEvents.DecrementEnergy?.Invoke();
            ray = new Ray(rayOrigin, rayDirection, width, height);

            while (RayInPlay())
                UpdateRayPosition();

            BBEvents.SendMollyIn?.Invoke();
            BBEvents.DelayInteraction?.Invoke(true);
            ray.AddMarkers();
        }

        private void UpdateRayPosition()
        {
            Vector3Int oldPosition = ray.position;
            GetFrontAndDiagonalCellPositions(out Vector3Int frontMid, out Vector3Int frontRight, out Vector3Int frontLeft);

            //Check if nodes exist and update position accordingly
            if (HasNode(frontMid)) //Hit
                ray.Kill(width); // since the main board is a square, gridLength = width = height
            else if (HasNode(frontRight) && HasNode(frontLeft)) //Reflect
                ray.Flip();
            else if (HasNode(frontRight)) //Detour Left
                ray.TurnLeft();
            else if (HasNode(frontLeft)) //Detour Right
                ray.TurnRight();

            //update position
            ray.Forward();

            //todo: draw debug line
        }

        private void GetFrontAndDiagonalCellPositions(out Vector3Int frontMid, out Vector3Int frontRight, out Vector3Int frontLeft)
        {
            frontMid = ray.position;
            frontRight = ray.position;
            frontLeft = ray.position;

            switch (ray.direction)
            {
                case Dir.Left:
                    frontMid.x++;
                    frontRight.x++; frontRight.y--;
                    frontLeft.x++; frontLeft.y++;
                    break;
                case Dir.Bot:
                    frontMid.y++;
                    frontRight.x++; frontRight.y++;
                    frontLeft.x--; frontLeft.y++;
                    break;
                case Dir.Right:
                    frontMid.x--;
                    frontRight.x--; frontRight.y++;
                    frontLeft.x--; frontLeft.y--;
                    break;
                case Dir.Top:
                    frontMid.y--;
                    frontRight.x--; frontRight.y--;
                    frontLeft.x++; frontLeft.y--;
                    break;
            }
        }

        private bool HasNode(Vector3Int gridPosition)
        {
            //todo: is this check and pattern necessary now that UnitGrids have been extracted from here?
            if (gridPosition.x < 0 || gridPosition.x >= width || gridPosition.y < 0 || gridPosition.y >= height)
                return false;

            NodeCell cell = cellArray[gridPosition.x, gridPosition.y];
            if (cell != null)
                return cell.HasNode();

            return false;
        }

        private bool RayInPlay()
        {
            bool result = true;
            if (ray.position.x < 0 || ray.position.x >= width || ray.position.y < 0 || ray.position.y >= height)
                result = false;

            return result;
        }

        #endregion

        private void ResetEnergy()
        {
            energyUnits = (int)BBEvents.GetNumEnergyUnits?.Invoke();
        }

        void DelayInteraction(bool inFog)
        {
            interactionDelayer.Replace(Delay(inFog));
        }

        System.Collections.IEnumerator Delay(bool inFog)
        {
            if (inFog) yield return new WaitForSeconds(fogDelayTime);
            else yield return new WaitForSeconds(swapDelayTime);
        }

        void DelayReaction(Action reaction)
        {
            interactionDelayer.Replace(ReactionDelay(reaction));
        }

        System.Collections.IEnumerator ReactionDelay(Action delayedReaction)
        {
            yield return new WaitForSeconds(fogReactionDelayTime);
            if (delayedReaction != null) delayedReaction();
            yield return new WaitForSeconds(fogReactionDelayTime);
        }

        bool IsDelayed()
        {
            return interactionDelayer.Exists();
        }

        int GetLanternsOnGridCount()
        {
            int count = 0;
            for (int y = 0; y < cellArray.GetLength(1); y++)
            {
                for (int x = 0; x < cellArray.GetLength(0); x++)
                {
                    if (cellArray[x, y].HasLantern()) count++;
                }
            }

            return count;
        }
    }
}
