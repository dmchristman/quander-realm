using System.Collections;
using UnityEngine;

namespace Wrapper
{
    public class CardPopup : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject cardContainer;
        [SerializeField] private QButton okButton;
        [SerializeField] private QButton backgroundButton;

        private void Awake()
        {
            okButton.onClick.AddListener(() => ToggleDisplay(false));
            backgroundButton.onClick.AddListener(() => ToggleDisplay(false));
        }

        public IEnumerator DisplayCard(GameObject rewardGO)
        {
            Events.PlaySound?.Invoke("W_Reward");
            rewardGO.transform.SetParent(cardContainer.transform);
            rewardGO.GetComponent<Transform>().localScale = new Vector3(0.8f, 0.8f, 0.8f);

            ToggleDisplay(true);

            while (!(rewardGO.activeInHierarchy))
                yield return null;
            rewardGO.GetComponent<Animator>().SetBool("Disabled", false);
        }

        public GameObject GetContainerMount()
        {
            return cardContainer;
        }

        private void ToggleDisplay(bool isOn)
        {
            animator.SetBool("PopupOn", isOn);
        }
    }
}
