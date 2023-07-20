using UnityEngine;
using System.Collections;

/*
 * Customer animation manager
 */
namespace Qupcakery
{
    public class CustomerManager : MonoBehaviour
    {
        private enum CustomerStatus
        {
            Moving, Waiting, ReactionInProgress, WaitingDeliveryToArrive,
            Inactive
        }

        public Animator animator;

        [SerializeField]
        private CustomerData customerData;
        [SerializeField]
        private CustomerReactionController reactionController;
        public int order { get; private set; }

        [SerializeField]
        private CustomerStatus status = CustomerStatus.Inactive;
        [SerializeField]
        private Direction direction = Direction.Left;
        private HorizontalMovement movement;
        private bool arrivedAtTable = false;

        public int MaxTip { get { return customerData.MaxTip; } }
        public int BasePay { get { return customerData.BasePay; } }
        public Patience Patience { get; private set; }

        public int BeltInd { get; private set; }

        // Cake-received event
        public delegate void CakeReceivedEndedEventHandler();
        public event CakeReceivedEndedEventHandler CakeReceived;

        // Batch end event
        public delegate void BatchEndedEventHandler();
        public event BatchEndedEventHandler BatchEnded;

        // Arrival at table event
        public delegate void ArrivedAtTableEventHandler();
        public event ArrivedAtTableEventHandler ArrivedAtTable;

        public void SetBeltInd(int ind) { BeltInd = ind; }

        // Use this for initialization
        void Awake()
        {
            movement = new HorizontalMovement(initialSpeed: customerData.Speed);

            Patience = new Patience(customerData.MaxPatience,
                customerData.PatienceFreezeTime);
        }

        private void Start()
        {
            // Subscribe to patience-end publisher
            Patience.PatienceEnded += reactionController.OnPatienceEnded;
            Patience.PatienceEnded += this.OnPatienceEnded;
            Patience.PatienceUpdated += transform.Find("PatienceBar").
                GetComponent<PatienceController>().OnPatienceUpdated;

            // Subscribe to reaction publisher
            reactionController.ReactionEnded += OnCustomerReactionDone;

            // Subscribe to button event
            GameObjectsManagement.Button.GetComponent<ButtonController>().
                ButtonPressed += OnCakeInDelivery;
        }

        private void FixedUpdate()
        {
            switch (status)
            {
                case CustomerStatus.Moving:
                    transform.localPosition = movement.UpdatePosition(transform.localPosition,
                    Time.fixedDeltaTime, direction);
                    // If the customer has left scene, publish batchdone event
                    if (CustomerIsOffScene())
                    {
                        OnBatchEnded();
                    }
                    break;
                case CustomerStatus.Waiting:
                    Patience.DecreasePatience(Time.fixedDeltaTime);
                    break;
                case CustomerStatus.ReactionInProgress:
                case CustomerStatus.WaitingDeliveryToArrive:
                case CustomerStatus.Inactive:
                    return;
            }
        }

        public void ResetCustomerManager()
        {
            UpdateCustomerStatus(CustomerStatus.Inactive);
            direction = Direction.Left;
            arrivedAtTable = false;

            Patience.ResetPatience();
            GetComponent<SpriteRenderer>().flipX = false;
        }

        // Customer receives cake 
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "CakeBox")
            {
                UpdateCustomerStatus(CustomerStatus.ReactionInProgress);
                OnCakeReceived();
            }
        }

        // Customer reaches table
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Table"
                && !arrivedAtTable)
            {
                arrivedAtTable = true;
                UpdateCustomerStatus(CustomerStatus.Waiting);
                direction = Direction.Right;
                // Publisher
                OnArrivedAtTable();
            }
        }

        // Event listener - once patience runs out
        public void OnPatienceEnded()
        {
            UpdateCustomerStatus(CustomerStatus.ReactionInProgress);
        }

        // Subscriber: once customer has checked the cake and is ready to leave
        private void OnCustomerReactionDone()
        {
            // Debug.Log("Reaction done subscriber");
            UpdateCustomerStatus(CustomerStatus.Moving);
        }

        // Publisher: notifies all cake-received event subscribers
        protected virtual void OnCakeReceived()
        {
            // Checks that there are at least 1 subscriber
            if (CakeReceived != null)
            {
                CakeReceived();
            }
        }

        // Publisher: notifies all customer arrival at table event subscribers
        protected virtual void OnArrivedAtTable()
        {
            // Checks that there are at least 1 subscriber
            if (ArrivedAtTable != null)
                ArrivedAtTable();
        }

        // Subscriber
        public void OnCakeInDelivery()
        {
            UpdateCustomerStatus(CustomerStatus.WaitingDeliveryToArrive);
        }

        // Checks whether the customer has left the scene
        private bool CustomerIsOffScene()
        {
            if (transform.position.x > 12f)
                return true;
            else
                return false;
        }

        // Assigns order to customer
        public void AssignOrder(int assignedOrder)
        {
            order = assignedOrder;
        }

        public void SendCustomer()
        {
            UpdateCustomerStatus(CustomerStatus.Moving);
        }

        public void ActivateCustomer()
        {
            UpdateCustomerStatus(CustomerStatus.Moving);
        }

        protected virtual void OnBatchEnded()
        {
            if (BatchEnded != null)
            {
                BatchEnded();
            }
        }

        public void RemoveBatchEndedListeners()
        {
            BatchEnded = null;
        }

        void UpdateCustomerStatus(CustomerStatus newStatus)
        {
            status = newStatus;
            if (status == CustomerStatus.Moving)
                animator.enabled = true;
            else
                animator.enabled = false;
        }

    }
}
