using UnityEngine;
using UnityEngine.EventSystems;

namespace BlackBox
{
    public class LanternMount : MonoBehaviour, IDropHandler
    {
        private Vector3Int gridPosition = Vector3Int.back;
        private GameObject mountedLantern = null;

        public bool isEmpty = false;
        public GameObject[] colliders;

        [SerializeField] private Canvas canvas = null;
        [SerializeField] private Animator nodeCellAnimator = null;

        private void Start()
        {
            EvaluateEmpty();   
        }

        public void EvaluateEmpty()
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Lantern>() != null)
                {
                    isEmpty = false;
                    break;
                }
            }

            if (!isEmpty)
            {
                foreach (Transform child in transform)
                {
                    if (child.GetComponent<Lantern>() != null)
                    {
                        mountedLantern = child.gameObject;
                        break;
                    }
                }
            }
        }

        public void SetMountedLantern(GameObject lanternGO)
        {
            mountedLantern = lanternGO;
            mountedLantern.GetComponent<Lantern>().SetCanvas(canvas);
            EvaluateEmpty();
        }

        public void SetColliderActive(GridSize gridSize)
        {
            foreach (GameObject GO in colliders)
                GO.SetActive(false);

            colliders[(int)gridSize].SetActive(true);
        }

        public void SetGridPosition(Vector3Int position)
        {
            gridPosition = position;
        }

        public void OnDrop(PointerEventData eventData)
        {
            GameObject lanternGO = eventData.pointerDrag;

            if (isEmpty)
                Flag(lanternGO);
            else
                BBEvents.ReturnLanternHome?.Invoke(lanternGO);
        }

        public void Flag(GameObject lanternGO)
        {
            lanternGO.transform.SetParent(this.transform);
            lanternGO.GetComponent<Lantern>().SetParentMount(this);
            lanternGO.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            isEmpty = false;
            mountedLantern = lanternGO;

            if (gridPosition != Vector3Int.back) // using "back" as default. Z will be 0 if this mount belongs to a cell
                BBEvents.ToggleFlag.Invoke(gridPosition, true);

            if (nodeCellAnimator != null)
                nodeCellAnimator.SetBool("NodeCell/Flagged", true); // animator will be set if this mount belongs to a cell

            BBEvents.CheckWolfieReady?.Invoke();
        }

        public void UnFlag()
        {
            isEmpty = true;
            mountedLantern = null;
            
            if (gridPosition != Vector3Int.back)
                BBEvents.ToggleFlag.Invoke(gridPosition, false);

            if (nodeCellAnimator != null)
                nodeCellAnimator.SetBool("NodeCell/Flagged", false);

            BBEvents.CheckWolfieReady?.Invoke();
        }
    }
}
