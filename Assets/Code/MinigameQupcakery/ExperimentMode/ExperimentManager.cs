using UnityEngine;
using System.Collections;

namespace Qupcakery
{
    public class ExperimentManager : MonoBehaviour
    {
        public GameObject CakePrefab;
        public GameObject GatePrefab;

        public GameObject ButtonPrefab;
        public GameObject TablePrefab;
        public GameObject PanelPrefab;
        public GameObject BeltPrefab;

        private int levelInd;
        private Level level;

        private void Awake()
        {
            levelInd = GameManagement.Instance.GetCurrentLevelInd();
        }

        // Use this for initialization
        void Start()
        {
            level = GameManagement.Instance.GetCurrentLevel();
            //SetupUtilities.SetupExperimentScene(level, ButtonPrefab, TablePrefab,
            //    GameObjectsManagement.Panel, PanelPrefab, GatePrefab, BeltPrefab, CakePrefab);

            CakeSlots.Instance.InitializeCakeSlots(level.TotalBeltCnt);

            // Subscribe button listener to cake tracker
            CakeOnBeltTracker.Instance.CakesReadyToBeDelivered
                += ExperimentButtonController.Instance.OnCakesReady;
            CakeOnBeltTracker.Instance.CakesRemovedFromBelt
                += ExperimentButtonController.Instance.OnCakesRemovedFromBelt;
        }

        // #TODO: OnLevelIndSet()
    }
}
