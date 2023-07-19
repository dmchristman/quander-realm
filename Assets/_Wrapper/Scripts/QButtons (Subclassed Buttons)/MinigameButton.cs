using UnityEngine.EventSystems;

namespace Wrapper
{
    public class MinigameButton : QButton
    {
        public Minigame minigame;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            Events.ScreenFadeMidAction?.Invoke(() => Events.OpenMinigame?.Invoke(minigame), 0.2F);
        }
    }
}
