using UnityEngine;
using System.Collections;
using Costume = Qupcakery.AssetCostumeUtilities;

/* Button manager */
namespace Qupcakery
{
    public class ButtonController : MonoBehaviour
    {
        /* Button pressed event handling */
        public delegate void ButtonPressedHandler();
        public ButtonPressedHandler ButtonPressed;

        public void SubscribeToCustomerEvent(GameObject customer)
        {
            /* Subscribe to 1st customer's events */
            CustomerManager cm = customer.GetComponent<CustomerManager>();
            cm.Patience.PatienceEnded += OnCustomerPatienceEnded;
            cm.CakeReceived += OnCustomerReceivedCake;
            cm.ArrivedAtTable += OnCustomerArrivalAtTable;
        }

        public enum ButtonState
        {
            CanNotBePressed, CanBePressed, Pressed
        }

        public ButtonState buttonState { get; protected set; }

        public void ResetButton()
        {
            UpdateButtonState(ButtonState.CanNotBePressed);
        }

        // On press
        protected void OnMouseDown()
        {
            if (GameUtilities.gameIsPaused)
                return;

            if (buttonState == ButtonState.CanBePressed)
            {
                UpdateButtonState(ButtonState.Pressed);
                OnButtonPressed();
            }
        }

        protected void UpdateButtonState(ButtonState bs)
        {
            buttonState = bs;

            switch (buttonState)
            {
                case ButtonState.CanNotBePressed:
                    Costume.SetCostume(gameObject, "Basic",
                        "CanNotBePressed");
                    break;
                case ButtonState.CanBePressed:
                    Costume.SetCostume(gameObject, "Basic",
                       "CanBePressed");
                    break;
                case ButtonState.Pressed:
                    Costume.SetCostume(gameObject, "Basic",
                       "Pressed");
                    break;
            }
        }

        // Publisher
        protected virtual void OnButtonPressed()
        {
            if (ButtonPressed != null)
            {
                ButtonPressed();
            }
        }

        // Subscriber
        public void OnCustomerReceivedCake()
        {
            if (buttonState == ButtonState.Pressed)
            {
                UpdateButtonState(ButtonState.CanNotBePressed);
            }
        }

        // Subscriber
        public void OnCustomerArrivalAtTable()
        {
            if (buttonState == ButtonState.CanNotBePressed)
                UpdateButtonState(ButtonState.CanBePressed);
        }

        // Subscriber to patience-end for a batch
        public void OnCustomerPatienceEnded()
        {
            if (buttonState != ButtonState.CanNotBePressed)
                UpdateButtonState(ButtonState.CanNotBePressed);
        }

        // Subscriber to CakeOnBeltTracker
        public virtual void OnCakesReady()
        {
            // Debug.Log("OnCakesReady: Not implemented");
        }

        // Subscriber to CakeOnBeltTracker
        public virtual void OnCakesRemovedFromBelt()
        {
            // Debug.Log("OnCakesRemovedFromBelt: Not implemented");

        }
    }
}
