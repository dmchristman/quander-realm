using UnityEngine;
using System.Collections.Generic;
using Costume = Qupcakery.AssetCostumeUtilities;

/* Gate drag and drop */
namespace Qupcakery
{
    public enum GateState
    {
        InBank, OnDrag, OnBelt
    }

    public class GatePositionController : MonoBehaviour
    {
        // Gate is put in new slot event
        public delegate void GateInSlotEventHandler(List<int> beltInd);
        public event GateInSlotEventHandler GateIsInNewSlot;
        public int gateSize; // Set by the operation controller

        private GateBank gateBank;

        public GateState gateState { get; private set; }
            = GateState.InBank;

        private void Start()
        {
            gateBank = GateBank.Instance;
            switch (GameManagement.Instance.gameMode)
            {
                case GameManagement.GameMode.Regular:
                    Dispatcher dispatcher = GameObject.Find("LevelManager").
                    GetComponent<LevelManager>().Dispatcher;
                    dispatcher.BatchDonePublisher += OnBatchDone;
                    break;
                default:
                    /* do nothing */
                    break;
            }
        }

        // Drag gate with mouse
        private void OnMouseDrag()
        {
            if (GameUtilities.gameIsPaused)
                return;

            switch (gateState)
            {
                case GateState.InBank:
                    gateBank.SubtractGateFromBank(gameObject);
                    // Update gate state
                    SetGateState(GateState.OnDrag);
                    break;
                case GateState.OnBelt:
                    GateSlots.Instance.RemoveGateFromSlot(gameObject,
                        transform.position);
                    // Update gate state
                    SetGateState(GateState.OnDrag);
                    break;
                case GateState.OnDrag:
                    break;
            }


            // Move gate position with mouse 
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            transform.Translate(mousePosition);
        }

        // Place gate on belt or return it to bank
        private void OnMouseUp()
        {
            if (GameUtilities.gameIsPaused)
                return;

            if (gateState != GateState.OnDrag)
                return;

            Vector2 position = transform.position;
            List<int> beltInd = new List<int>();

            bool success = GateSlots.Instance.PlaceGateInSlot(gameObject,
                position, beltInd);
            if (success)
            {
                SetGateState(GateState.OnBelt);
                OnGateIsInNewSlot(beltInd);
            }
            else // put back to bank
            {
                transform.position = gateBank.AddGateToBank(gameObject);
                SetGateState(GateState.InBank);
            }
        }

        // Set gate state
        public void SetGateState(GateState newGateState)
        {
            gateState = newGateState;
            switch (gateState)
            {
                case GateState.OnBelt:
                case GateState.OnDrag:
                    Costume.SetGateCostumeOnBelt(gameObject);
                    if (gateSize == 2)
                    {
                        transform.localScale = new Vector3(1f, 1.3f, 1f);
                        GetComponent<BoxCollider2D>().size = new Vector2(1f, 1.3f);
                    }
                    break;
                default:
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f); ;

                    Costume.SetGateCostumeOffBelt(gameObject);
                    break;
            }
        }

        // Publisher
        protected virtual void OnGateIsInNewSlot(List<int> beltInd)
        {
            if (GateIsInNewSlot != null)
            {
                GateIsInNewSlot(beltInd);
            }
        }

        // Subscriber on batch is done, reset everything
        public void OnBatchDone()
        {
            if (gateState == GateState.OnBelt)
            {
                // Debug.Log("Gate " + GetComponent<GateOperationController>().gate.Type + "received on batch done notification");

                GateSlots.Instance.RemoveGateFromSlot(gameObject, transform.position);

                // Put gate back to bank
                transform.position = gateBank.AddGateToBank(gameObject);
                SetGateState(GateState.InBank); // #TODO: is this necessary?
            }
        }

        //// #TODO: reset gate position and gate execution status
        //private void resetGate()
        //{


        //}
    }
}
