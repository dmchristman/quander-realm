using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using BeauRoutine;

namespace BlackBox
{
    public class Lantern : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IPointerUpHandler
    {
        private CanvasGroup canvasGroup;
        private LanternMount parentMount;
        private RectTransform rectTransform;

        [SerializeField] private Canvas canvas;
        [SerializeField] private Animator animator;

        private float directionVelocityThreshold = 0.23f;

        [SerializeField]
        float waitTime = 0.2F;
        Routine resetAnim;
        bool isHeld = false;

        private void Awake()
        {
            parentMount = GetParentMount();
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        #region Interface Functions

        public void OnPointerDown(PointerEventData eventData)
        {
            isHeld = true;
            animator.SetBool("Hold", true);
            Wrapper.Events.PlaySound?.Invoke("BB_PickUpLantern");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            canvasGroup.blocksRaycasts = false;
            BBEvents.ToggleLanternHeld?.Invoke(true);

            if (parentMount != null)
                parentMount.UnFlag();

            transform.SetParent(BBEvents.GetFrontMount?.Invoke());
            transform.SetAsLastSibling();
            parentMount = null;
        }

        public void OnDrag(PointerEventData eventData)
        {
            UpdateAnimator(eventData);
            resetAnim.Replace(DelayedAnimReset());
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isHeld = false;
            animator.SetBool("Hold", false);
            animator.SetFloat("Velocity", 0f);
            animator.SetInteger("DragDirection", 0);

            canvasGroup.blocksRaycasts = true;
            Wrapper.Events.PlaySound?.Invoke("BB_PlaceLantern");
            BBEvents.ToggleLanternHeld?.Invoke(false);
            Routine.Start(ReturnHomeIfUnmounted());
        }

        #endregion

        #region Helpers

        private void UpdateAnimator(PointerEventData eventData)
        {
            Vector2 oldPosition = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            Vector2 newPosition = rectTransform.anchoredPosition;

            float currentVelocity = animator.GetFloat("Velocity");

            animator.SetInteger("DragDirection", GetDirection(oldPosition, newPosition));
            animator.SetFloat("Velocity", GetVelocity(oldPosition, newPosition, currentVelocity));
        }

        private float GetVelocity(Vector2 oldPosition, Vector2 newPosition, float currentVelocity)
        {
            float newVelocity = Mathf.Clamp(1 - (1 / (Vector2.Distance(newPosition, oldPosition) / 2)), 0, 1);
            float delta = 1 - (1 / (Vector2.Distance(newPosition, oldPosition) * 4));

            return Mathf.Lerp(currentVelocity, newVelocity, 1 - delta);
        }

        private int GetDirection(Vector2 oldPosition, Vector2 newPosition)
        {
            float newVelocity = 1 - (1 / (Vector2.Distance(newPosition, oldPosition) / 2));
            if (newVelocity <= directionVelocityThreshold)
                return 0;

            return (int)Mathf.Sign(newPosition.x - oldPosition.x);
        }

        private IEnumerator ReturnHomeIfUnmounted()
        {
            yield return null;

            if (GetParentMount() == null)
            {
                BBEvents.ReturnLanternHome?.Invoke(this.gameObject);
            }
        }

        public void SetCanvas(Canvas canvas)
        {
            this.canvas = canvas;
        }

        public void SetParentMount(LanternMount parent)
        {
            parentMount = parent;
        }

        private LanternMount GetParentMount()
        {
            return transform.parent.GetComponent<LanternMount>();
        }

        IEnumerator DelayedAnimReset()
        {
            yield return waitTime;
            // reset
            if (isHeld)
            {
                float current = animator.GetFloat("Velocity");
                float time = 0F;
                while (time < waitTime)
                {
                    animator.SetFloat("Velocity", Mathf.Lerp(current, 0F, time / waitTime));
                    time += Time.deltaTime;
                    yield return null;
                }
                animator.SetInteger("DragDirection", 0);
            }
        }


        #endregion
    }
}
