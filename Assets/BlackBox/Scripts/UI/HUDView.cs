using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlackBox
{
    public class HUDView : MonoBehaviour
    {
        [SerializeField] private GameObject unitContainer = null;
        [SerializeField] private GameObject unitPrefab = null;
        [SerializeField] private Button wolfieButton = null;

        private Animator HUDAnimator = null;
        private List<Animator> reversedAnimatorList = null;

        private void Awake()
        {
            wolfieButton.onClick.AddListener(() => BBEvents.CheckWinState?.Invoke());
            HUDAnimator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            if (Wrapper.Events.IsDebugEnabled.Invoke()) BBEvents.ClearMarkers += InitEnergyBar; // Debug
            BBEvents.InitEnergyBar += InitEnergyBar;
            BBEvents.DecrementEnergy += DecrementEnergy;
            BBEvents.IndicateEmptyMeter += IndicateEmpty;
            BBEvents.ToggleWolfieButton += ToggleWolfieButton;
            BBEvents.UpdateHUDWolfieLives += UpdateWolfieLives;
            BBEvents.CloseLevel += CloseLevel;
        }

        private void OnDisable()
        {
            if (Wrapper.Events.IsDebugEnabled.Invoke()) BBEvents.ClearMarkers -= InitEnergyBar; // Debug
            BBEvents.InitEnergyBar -= InitEnergyBar;
            BBEvents.IndicateEmptyMeter -= IndicateEmpty;
            BBEvents.DecrementEnergy -= DecrementEnergy;
            BBEvents.ToggleWolfieButton -= ToggleWolfieButton;
            BBEvents.UpdateHUDWolfieLives -= UpdateWolfieLives;
            BBEvents.CloseLevel -= CloseLevel;
        }

        private void UpdateWolfieLives(int livesRemaining)
        {
            HUDAnimator.SetInteger("Lives", livesRemaining);
        }

        private void ToggleWolfieButton(bool isOn)
        {
            wolfieButton.interactable = isOn;
        }

        #region Energy Bar

        private void InitEnergyBar()
        {
            int numUnits = (int)BBEvents.GetNumEnergyUnits?.Invoke();

            foreach (Transform child in unitContainer.transform)
                Destroy(child.gameObject);

            reversedAnimatorList = new List<Animator>();
            for(int i = 0; i < numUnits; i++)
            {
                GameObject unit = Instantiate(unitPrefab, unitContainer.transform);
                reversedAnimatorList.Insert(0, unit.GetComponent<Animator>());
            }
        }

        private void IndicateEmpty()
        {
            HUDAnimator.SetTrigger("EmptyMeter");
            Wrapper.Events.PlaySound?.Invoke("BB_NoBattery");
        }

        private void DecrementEnergy()
        {
            foreach(Animator animator in reversedAnimatorList)
            {
                if (animator.GetBool("On"))
                {
                    animator.SetBool("On", false);
                    break;
                }
            }
        }

        void CloseLevel()
        {
            HUDAnimator.SetBool("On", false);
            BeauRoutine.Routine.Start(DelayClose());
        }

        System.Collections.IEnumerator DelayClose()
        {
            yield return 0.6F;
            BBEvents.OpenLevelSelect?.Invoke(true);
        }

        #endregion
    }
}
