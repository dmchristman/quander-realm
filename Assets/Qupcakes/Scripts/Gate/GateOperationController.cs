using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

/* Gate operations on cake */
namespace Qupcakery
{
    public class GateOperationController : MonoBehaviour
    {
        public Gate gate { get; private set; }
        private GatePositionController positionController;

        private List<GameObject> targetCakeBoxes = new List<GameObject>();
        private List<GameObject> controlCakeBoxes = new List<GameObject>();
        [SerializeField]

        private List<int> beltInd = new List<int>(); // Belt indices for the slot the gate occupies
        private bool executionCompleted = false;
        private bool waitingForCake = false;
        public bool ctrlTgtSwapped { get; private set; } = false;

        private void Awake()
        {
            positionController = gameObject.GetComponent<GatePositionController>();
            // Subscribe to gate-is-in-new-slot publisher
            positionController.GateIsInNewSlot += OnGateIsInNewSlot;

            switch (GameManagement.Instance.gameMode)
            {
                case GameManagement.GameMode.Regular:
                    Dispatcher dispatcher = GameObject.Find("LevelManager").
                    GetComponent<LevelManager>().Dispatcher;
                    break;
                default:
                    /* do nothing */
                    break;
            }
        }

        private void Update()
        {
            if (waitingForCake)
            {
                // Debug.Log(gate.Type + ": I am waiting for cake ");
                if (AddCakeFromBelt(beltInd))
                {
                    waitingForCake = false;
                    // Debug.Log(gate.Type + ": I got a cake ");
                }
            }
        }

        // Set gate type
        public void SetGateType(GateType gt)
        {
            switch (gt)
            {
                case GateType.NOT:
                    gate = new NotGate();
                    break;
                case GateType.SWAP:
                    gate = new SwapGate();
                    break;
                case GateType.CNOT:
                    gate = new CNotGate();
                    break;
                case GateType.H:
                    gate = new HGate();
                    break;
                case GateType.Z:
                    gate = new ZGate();
                    break;
                default:
                    throw new ArgumentException("Unsupported gatetype: "
                        + gt.ToString());
            }

            positionController.gateSize = gate.Size;

        }

        // Get gate type
        public GateType GetGateType()
        {
            return gate.Type;
        }

        // Once a cake enters
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (GameManagement.Instance.gameMode
                == GameManagement.GameMode.Experiment)
            {
                if (ExperimentButtonController.Instance.buttonState
                    != ExperimentButtonController.ButtonState.Pressed)
                    return;
            }

            if ((!(positionController.gateState == GateState.OnBelt))
                || executionCompleted)
                return;

            if (collision.gameObject.tag == "CakeBox")
                executeGate();
        }

        // Flip control and target channels for CNOT, CZ
        private void OnMouseDown()
        {
            if (GameUtilities.gameIsPaused)
                return;

            if (GameManagement.Instance.gameMode
                == GameManagement.GameMode.Regular)
            {
                if (GameObjectsManagement.Button.GetComponent<ButtonController>()
                    .buttonState == ButtonController.ButtonState.Pressed)
                    return;
            }


            if (positionController.gateState == GateState.OnBelt)
            {
                SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
                switch (gate.Type)
                {
                    case GateType.CNOT:
                        sr.flipY = sr.flipY ? false : true; // flip target and control
                        ctrlTgtSwapped = ctrlTgtSwapped ? false : true;
                        break;
                }
            }

        }

        private CakeBoxController GetCakeBoxControllerFromCake(GameObject cakeObject)
        {
            switch (GameManagement.Instance.gameMode)
            {
                case GameManagement.GameMode.Regular:
                    return cakeObject.GetComponent<CakeBoxController>();
                case GameManagement.GameMode.Experiment:
                    return cakeObject.GetComponent<ExperimentCakeBoxController>();
            }
            return null;
        }

        // Execute gate
        private void executeGate()
        {
            // Debug.Log("Gatetype is "+ gate.Type);
            switch (gate.Type)
            {
                case GateType.NOT:
                case GateType.H:
                case GateType.Z:
                    {
                        // Get controllers
                        CakeBoxController boxController =
                            GetCakeBoxControllerFromCake(targetCakeBoxes[0]);
                        Cake cake = boxController.cake;
                        // Execute gate
                        // Debug.Log("Executing NOT Gate");
                        gate.ExecuteGate(ref cake);
                    }
                    break;
                case GateType.SWAP:
                    {
                        // Get controllers
                        CakeBoxController boxController0 =
                            GetCakeBoxControllerFromCake(targetCakeBoxes[0]);
                        CakeBoxController boxController1 =
                            GetCakeBoxControllerFromCake(targetCakeBoxes[1]);
                        Cake cake0 = boxController0.cake;
                        Cake cake1 = boxController1.cake;
                        // Execute gate
                        gate.ExecuteGate(ref cake0, ref cake1);
                    }
                    break;
                case GateType.CNOT:
                    {
                        // Get controllers
                        CakeBoxController targetBoxController =
                            GetCakeBoxControllerFromCake(targetCakeBoxes[0]);
                        CakeBoxController controlBoxController =
                            GetCakeBoxControllerFromCake(controlCakeBoxes[0]);
                        Cake targetCake = targetBoxController.cake;
                        Cake controlCake = controlBoxController.cake;
                        // Execute gate
                        gate.ExecuteGate(ref controlCake, ref targetCake);
                    }
                    break;
                default:
                    throw new ArgumentException("Unsupported gatetype: "
                        + gate.Type);
            }

            executionCompleted = true;
        }

        // Subscriber
        public void OnGateIsInNewSlot(List<int> newBeltInd)
        {
            beltInd = newBeltInd;
            ResetCakeTracker();

            // Remove previously tracked cakes
            targetCakeBoxes.Clear();
            controlCakeBoxes.Clear();

            // Add cake to controllers
            bool success = AddCakeFromBelt(beltInd);
            if (!success)
                waitingForCake = true;
        }

        private void ResetCakeTracker()
        {
            targetCakeBoxes.Clear();
            controlCakeBoxes.Clear();
        }

        // Adds existing cake on belt to the gate
        // return true on success
        private bool AddCakeFromBelt(List<int> beltInd)
        {
            GateType gt = gate.Type;

            if (gt == GateType.NOT || gt == GateType.SWAP || gt == GateType.H
                || gt == GateType.Z)
            {
                foreach (int ind in beltInd)
                {
                    GameObject cake = CakeOnBeltTracker.Instance.GetCakeFromBelt(ind);
                    if (cake == null)
                        return false;
                    if (targetCakeBoxes.ElementAtOrDefault(ind) != null)
                        targetCakeBoxes[ind] = cake;
                    else
                    {
                        targetCakeBoxes.Add(cake);
                    }
                }
                // Debug.Log("Target cake box is " + targetCakeBoxes[0]);
                return true;
            }
            else if (gt == GateType.CNOT)
            {
                int ind0 = beltInd[0];
                int int1 = beltInd[1];

                int ctl_ind, tgt_ind;
                if (ctrlTgtSwapped)
                {
                    ctl_ind = Math.Min(ind0, int1);
                    tgt_ind = Math.Max(ind0, int1);
                }
                else
                {
                    ctl_ind = Math.Max(ind0, int1);
                    tgt_ind = Math.Min(ind0, int1);
                }

                GameObject ctl_cake = CakeOnBeltTracker.Instance.
                    GetCakeFromBelt(ctl_ind);
                GameObject tgt_cake = CakeOnBeltTracker.Instance.
                    GetCakeFromBelt(tgt_ind);

                if (ctl_cake == null || tgt_cake == null)
                    return false;

                if (targetCakeBoxes.ElementAtOrDefault(0) != null)
                    targetCakeBoxes[0] = tgt_cake;
                else
                    targetCakeBoxes.Add(tgt_cake);

                if (controlCakeBoxes.ElementAtOrDefault(0) != null)
                    controlCakeBoxes[0] = ctl_cake;
                else
                    controlCakeBoxes.Add(ctl_cake);

                return true;

            }
            else
                throw new ArgumentException("Unsupported gate type: " + gt);
        }


        // Reset gate status
        public void ResetGateStatus()
        {
            gameObject.GetComponent<SpriteRenderer>().flipY = false;
            executionCompleted = false;
            ctrlTgtSwapped = false;

            ResetCakeTracker();
        }
    }

}