using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BeauRoutine;

namespace Wrapper
{
    /// <summary>
    /// Exclusively used for loading after the player choses to reset their game data
    /// </summary>
    public class LoadingPopup : MonoBehaviour
    {
        bool inUse = false;

        [SerializeField]
        GameObject container;
        [SerializeField]
        Animator anim;
        [SerializeField]
        Image fillBar;

        public void OpenPopup()
        {
            if (inUse) return;
            inUse = true;
            container.SetActive(true);
            Routine.Start(PlayLoadingSequence());
        }

        IEnumerator PlayLoadingSequence()
        {
            fillBar.fillAmount = 0F;
            anim.SetTrigger("AreYouSure_On");
            yield return 0.4F;
            yield return fillBar.FillTo(1F, 2F);
            Events.SetNewPlayerStatus?.Invoke(true);
            yield return 0.2F;
            ClosePopup();

        }

        public void ClosePopup()
        {
            anim.SetTrigger("AreYouSure_Off");
            inUse = true;
            Routine.Start(DelayClose());
        }

        public void ForceCloseConfirmation()
        {
            container.SetActive(false);
            inUse = false;
        }

        IEnumerator DelayClose()
        {
            yield return 0.3F;
            container.SetActive(false);
            inUse = false;
        }
    }
}
