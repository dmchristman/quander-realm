using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Wrapper
{
    public class ConfirmationPopup : MonoBehaviour
    {
        bool closing = false;

        [SerializeField]
        GameObject confirmation;
        [SerializeField]
        Animator anim;
        [SerializeField]
        Button yesButton;
        [SerializeField]
        Button noButton;

        public void SetConfirmationData(System.Action yesAction, System.Action noAction)
        {
            yesButton.onClick.RemoveAllListeners();
            if (yesAction != null) yesButton.onClick.AddListener(() => yesAction());
            if (noButton != null)
            {
                noButton.onClick.RemoveAllListeners();
                if (noAction != null) noButton.onClick.AddListener(() => noAction());
            }
        }

        public void OpenConfirmation()
        {
            if (closing) return;
            confirmation.SetActive(true);
            anim.SetTrigger("AreYouSure_On");
            if (yesButton != null) yesButton.onClick.AddListener(CloseConfirmation);
            if (noButton != null) noButton.onClick.AddListener(CloseConfirmation);
        }

        public void CloseConfirmation()
        {
            anim.SetTrigger("AreYouSure_Off");
            closing = true;
            BeauRoutine.Routine.Start(DelayClose());
        }

        public void ForceCloseConfirmation()
        {
            confirmation.SetActive(false);
            closing = false;
        }

        IEnumerator DelayClose()
        {
            yield return 0.3F;
            confirmation.SetActive(false);
            closing = false;
        }
    }
}
