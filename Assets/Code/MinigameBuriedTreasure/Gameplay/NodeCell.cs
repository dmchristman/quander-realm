using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BlackBox
{
    public class NodeCell : Cell
    {
        private bool hasNode = false;
        private bool hasFlag = false;
        private bool debug = false; // Debug

        [SerializeField] private LanternMount lanternMount = null;
        [SerializeField] private GameObject nodeObj = null;
        [SerializeField] private List<Button> buttons = null; // Debug
        [SerializeField] private TextMeshProUGUI text = null;

        protected override void Start()
        {
            base.Start();
            lanternMount.SetGridPosition(gridPosition);
            lanternMount.EvaluateEmpty();

            if (Wrapper.Events.IsDebugEnabled.Invoke()) SetupDebug();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (Wrapper.Events.IsDebugEnabled.Invoke()) BBEvents.ToggleDebug += ToggleDebug;
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            if (Wrapper.Events.IsDebugEnabled.Invoke()) BBEvents.ToggleDebug -= ToggleDebug;
        }
        public override void Interact()
        {
            if (cellType == CellType.EdgeNode)
                return;

            // Debug
            if (debug)
            {
                hasNode = !hasNode;
                nodeObj.SetActive(hasNode);
            }
        }

        public void SetNode()
        {
            hasNode = !hasNode;
        }

        //todo: make these properties?
        public override bool HasNode()
        {
            return hasNode;
        }

        public bool HasFlag()
        {
            return hasFlag;
        }

        public void ToggleFlag(bool isOn)
        {
            hasFlag = isOn;
        }

        public bool HasLantern()
        {
            return !lanternMount.isEmpty;
        }

        #region Debug

        private void SetupDebug()
        {
            debug = (bool)BBEvents.IsDebug?.Invoke();

            text.text = gridPosition.x.ToString() + ", " + gridPosition.y.ToString();

            if (debug)
                ToggleDebug();
        }

        private void ToggleDebug()
        {
            debug = !(text.gameObject.activeInHierarchy);
            text.gameObject.SetActive(debug);

            if (!hasNode)
                return;

            nodeObj.SetActive(debug);
        }

        #endregion
    }
}
