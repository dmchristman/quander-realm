using System.Collections;
using UnityEngine;
using BeauRoutine;

namespace Wrapper
{
    public class UploadFailurePopup : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject container;
        [SerializeField] private QButton retryButton;
        [SerializeField] private QButton continueButton;

        private void Awake()
        {
            continueButton.onClick.AddListener(() => TogglePopup(false));
            retryButton.onClick.AddListener(() => 
            { 
                animator.SetBool("Loading", true);
                Events.UpdateRemoteSave?.Invoke();
            });
        }

        private void OnEnable()
        {
            Events.ToggleUploadFailurePopup += TogglePopup;
        }

        private void OnDisable()
        {
            Events.ToggleUploadFailurePopup -= TogglePopup;
        }

        private void TogglePopup(bool isOn)
        {
            animator.SetBool("Loading", false);

            if (isOn)
                Routine.Start(ShowRoutine());
            else
                Routine.Start(HideRoutine());
        }

        private IEnumerator ShowRoutine()
        {
            container.SetActive(true);
            while (!container.activeInHierarchy)
                yield return null;

            animator.SetBool("On", true);
            animator.SetTrigger("Error");
        }

        private IEnumerator HideRoutine()
        {
            animator.SetBool("On", false);
            yield return animator.WaitToCompleteAnimation();

            container.SetActive(false);
        }
    }
}