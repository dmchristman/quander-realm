using UnityEngine;
using System.Collections;
using Costume = Qupcakery.AssetCostumeUtilities;

/* Manages cake state change and box movement in game */
namespace Qupcakery
{
    public class CakeBoxController : MonoBehaviour
    {
        [SerializeField]
        public Cake cake { get; set; } = new Cake();
        //public Cake entangledCake { get; private set; }

        [SerializeField]
        protected CakeData cakeData;
        protected Direction direction = Direction.Right;
        protected HorizontalMovement movement;
        public bool moving = false;

        private void Awake()
        {
            cake.CakeStateUpdated += UpdateCostume;
        }

        protected void Start()
        {
            /* Subscribe to button-pressed event publisher */
            GameObjectsManagement.Button.GetComponent<ButtonController>().
                ButtonPressed += OnButtonPressed;

            movement = new HorizontalMovement(initialSpeed: cakeData.Speed);
        }

        protected void FixedUpdate()
        {
            if (moving)
            {
                transform.localPosition = movement.UpdatePosition(
                    transform.localPosition, Time.fixedDeltaTime, direction);
            }
        }

        // Reaches customer
        protected void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Customer")
            {
                moving = false;
            }
        }

        // Update cakestate
        public void UpdateCakeBoxState(int CakeSpec)
        {
            //Debug.Log("Updating cakeState to " + cakeState.GetCakeType());
            cake.UpdateCakeState(CakeSpec);
        }

        public void UpdateCostume()
        {
            if (!cake.IsEntangled)
                Costume.SetCakeBoxCostume(gameObject, cake);
            else
            {
                foreach (var cakeObj in GameObjectsManagement.CakeBoxes)
                {
                    if (cakeObj.activeSelf)
                        Costume.SetCakeBoxCostume(
                            cakeObj, cakeObj.GetComponent<CakeBoxController>().cake);
                }
            }
        }

        // Update cake type (post-measurement)
        public void SetMeasuredCakeType(GameCakeType gameCakeType)
        {
            GameObject cakeObj = transform.Find("ActualCake").gameObject;
            Costume.SetCakeCostume(cakeObj, gameCakeType);
        }

        // Subscriber
        protected void OnButtonPressed()
        {
            moving = true;
        }

        //public void SetEntangledCake(Cake cake)
        //{
        //    entangledCake = cake; 
        //}

        public void Reset()
        {
            moving = false;
            //entangledCake = null;
        }
    }
}
