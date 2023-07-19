using UnityEngine;
using System.Collections;

namespace Qupcakery
{
    public class ExperimentCakeBoxController : CakeBoxController
    {
        CakePositionController positionController;

        private new void Start()
        {
            ExperimentButtonController.Instance.ButtonPressed
               += OnButtonPressed;

            positionController = gameObject.GetComponent<CakePositionController>();
            movement = new HorizontalMovement(initialSpeed: cakeData.Speed);

        }

        private new void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Table")
            {
                moving = false;
                Destroy(gameObject.GetComponent<CakePositionController>());
                Destroy(gameObject.GetComponent<ExperimentCakeBoxController>());
            }
        }

        // Subscriber
        private new void OnButtonPressed()
        {
            if (positionController.status == CakePositionController.CakeStatus.OnBelt)
            {
                // #TODO: make it opaque
                GameObject cakeRef = Instantiate(gameObject, transform.position, Quaternion.identity);
                Destroy(cakeRef.transform.Find("ActualCake").gameObject);
                cakeRef.transform.Find("Box").gameObject.
                    GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.7f);
                cakeRef.transform.Find("CakeLabel").gameObject.
                    GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.7f);
                Destroy(cakeRef.GetComponent<CakePositionController>());
                Destroy(cakeRef.GetComponent<ExperimentCakeBoxController>());

                moving = true;
            }
        }
    }
}
