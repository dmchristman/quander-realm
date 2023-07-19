using UnityEngine;
using UnityEngine.UI;

namespace BlackBox
{
    public abstract class Cell : MonoBehaviour
    {
        [SerializeField] protected Vector3Int gridPosition = Vector3Int.zero;
        [SerializeField] protected GameObject background = null;
        [SerializeField] protected Animator animator = null;
        [SerializeField] private Button button = null;

        [HideInInspector] public Dir direction = Dir.None;
        [HideInInspector] public CellType cellType = CellType.EdgeNode;

        protected virtual void Start()
        {
            button.onClick.AddListener(Interact);
        }

        protected virtual void OnEnable()
        {
            BBEvents.ToggleLanternHeld += ToggleLanternHeld;
        }

        protected virtual void OnDisable()
        {
            BBEvents.ToggleLanternHeld -= ToggleLanternHeld;
        }

        protected void ToggleLanternHeld(bool isOn)
        {
            animator.SetBool("NodeCell/Lantern", isOn);
        }

        public abstract void Interact();

        public abstract bool HasNode();

        public Cell Create(int xPos, int yPos, CellType cellType, Dir direction = Dir.None)
        {
            gridPosition = new Vector3Int(xPos, yPos, 0);
            this.cellType = cellType;
            this.direction = direction;
            SetDirectionAnim(direction);

            return this;
        }

        void SetDirectionAnim(Dir direction)
        {
            switch (direction)
            {
                case Dir.Top:
                    animator.SetInteger("BatGridLocation", 1);
                    break;
                case Dir.Right:
                    animator.SetInteger("BatGridLocation", 2);
                    break;
                case Dir.Bot:
                    animator.SetInteger("BatGridLocation", 3);
                    break;
                case Dir.Left:
                    animator.SetInteger("BatGridLocation", 4);
                    break;

                default: break;
            }
        }
    }
}