using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Wrapper;

namespace BlackBox
{
    public class LevelButton : QButton
    {
        [SerializeField]
        int levelID = -1;
        [SerializeField]
        Animator buttonAnim;

        public void SetButtonState(int currentLevel)
        {
            if (currentLevel >= levelID)
            {
                interactable = true;
                if (currentLevel > levelID) buttonAnim.SetBool("LevelCompleted", true);
            }
            else
            {
                interactable = false;
                buttonAnim.SetBool("LevelLocked", true);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable) return;

            base.OnPointerClick(eventData);

            if (levelID < 1 || levelID > 15)
            {
                Debug.LogError("Invalid level ID: " + levelID);
                return;
            }

            BBEvents.PlayLevel?.Invoke(BBGameManager.ParseLevelID(levelID));
        }
    }
}