using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BeauRoutine;
using System;

namespace Wrapper
{
    public class Reward : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Button button;

        [Header("Front")]
        [SerializeField] private TextMeshProUGUI cardTypeTextFront;
        [SerializeField] private Image cardTypeColorFront;
        [SerializeField] private TextMeshProUGUI flavorText;
        [SerializeField] public TextMeshProUGUI titleFront;
        // [SerializeField] private TextMeshProUGUI titleFront;
        [SerializeField] private Image image;

        [Header("Back")]
        [SerializeField] private TextMeshProUGUI cardTypeTextBack;
        [SerializeField] private Image cardTypeColorBack;
        [SerializeField] private TextMeshProUGUI titleBack;
        [SerializeField] private TextMeshProUGUI backText;

        private string id;
        public DisplayType displayType;
        // private DisplayType displayType;
        private const string stateDisabled = "Disabled";
        private const string stateSelected = "Selected";
        private const string triggerFlip = "Flip";
        
        public Game game;

        private void OnEnable()
        {
            Events.UnselectAllCards += UnselectSelf;
        }

        private void OnDisable()
        {
            Events.UnselectAllCards -= UnselectSelf;
        }

        public void SetContent(RewardAsset rAsset, Color color, DisplayType displayType)
        {
            string cardTypeDisplayName = GetDisplayName(rAsset.cardType.ToString());

            id = rAsset.rewardID;
            game = rAsset.game;
            this.displayType = displayType;

            cardTypeTextFront.text = cardTypeDisplayName;
            cardTypeColorFront.color = color;
            titleFront.text = rAsset.title;
            flavorText.text = rAsset.flavorText;
            SetFrontImage(rAsset);

            cardTypeTextBack.text = cardTypeDisplayName;
            cardTypeColorBack.color = color;
            titleBack.text = rAsset.title;
            backText.text = rAsset.backText;

            SetupButton();
        }

        private void SetupButton()
        {
            switch(displayType)
            {
                case DisplayType.Featured:
                    button.onClick.AddListener(() => animator.SetTrigger(triggerFlip));
                    break;
                case DisplayType.InJournal:
                    button.onClick.AddListener(() => Routine.Start(SelectCard()));
                    break;
                case DisplayType.CardPopup:
                default:
                    button.interactable = false;
                    break;
            }
        }

        private void SetFrontImage(RewardAsset rAsset)
        {
            Sprite cardSprite = Resources.Load<Sprite>(rAsset.imagePath);
            if (cardSprite == null)
            {
                Debug.LogFormat("Cannot find image at path {0}", rAsset.imagePath);
                return;
            }

            image.sprite = cardSprite;
        }

        public IEnumerator UpdateAnimationState()
        {
            if (IsUnlocked())
            {
                while (!(this.gameObject.activeInHierarchy))
                    yield return null;

                animator.SetBool(stateDisabled, false);
            }
        }

        public bool IsUnlocked()
        {
            return Events.IsRewardUnlocked?.Invoke(id) ?? false;
        }

        public IEnumerator SelectCard()
        {
            yield return UpdateAnimationState();

            if (!IsUnlocked())
                yield break;

            Events.UnselectAllCards?.Invoke();
            animator.SetBool(stateSelected, true);
            Events.FeatureCard?.Invoke(id.Trim().ToLower());
            button.interactable = false;
        }

        private void UnselectSelf()
        {
            animator.SetBool(stateSelected, false);
            button.interactable = true;
        }

        private string GetDisplayName(string input)
        {
            return input.Replace("_", " ");
        }
    }
}
