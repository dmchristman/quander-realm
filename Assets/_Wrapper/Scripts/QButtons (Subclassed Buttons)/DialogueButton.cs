using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Wrapper
{
    public class DialogueButton : QButton
    {
        [SerializeField] public Step step = Step.Forward;

        protected override void Awake()
        {
            if (this.step == Step.Backward)
                Events.TogglePreviousButton += ToggleInteractable;
            else if (step == Step.Skip) Events.EnableSkipButton += () => ToggleInteractable(true);
        }

        protected override void OnDestroy()
        {
            if (this.step == Step.Backward)
                Events.TogglePreviousButton -= ToggleInteractable;
            else if (step == Step.Skip) Events.EnableSkipButton -= () => ToggleInteractable(true);
        }

        private void ToggleInteractable(bool isOn)
        {
            interactable = isOn;
            animator.SetTrigger(interactable ? "Normal" : "Disabled");
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!interactable)
                return;

            base.OnPointerClick(eventData);

            Events.ChangeDialogue?.Invoke((int)step);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            if (step == Step.Forward)
                Events.EnableSkipButton?.Invoke();
        }
    }
}