using System;
using System.Collections;
using UnityEngine;
using Costume = Qupcakery.AssetCostumeUtilities;
using UnityEngine.U2D.Animation;

/* 
 * Manages customer reactions after receiving cakes
 * or after patience has run out (expression + tips)
 */
namespace Qupcakery
{
    public class CustomerReactionController : MonoBehaviour
    {
        private enum Expression
        {
            Happy, Sad
        }

        [SerializeField]
        private CustomerManager customerManager;
        [SerializeField]
        private GameObject tipPrefab;

        // Reaction end event
        public delegate void ReactionEndedEventHandler();
        public event ReactionEndedEventHandler ReactionEnded;

        [SerializeField]
        private CustomerReactionData reactionData;
        private GameObject cakeBox;
        private int customerOrder;

        private void Start()
        {
            customerManager = gameObject.GetComponent<CustomerManager>();
            cakeBox = GameObjectsManagement.CakeBoxes[customerManager.BeltInd];
        }

        private void OnEnable()
        {
            PuzzleCorrectionChecker.ResultChecked += OnResultReceived;
        }

        private void OnDisable()
        {
            PuzzleCorrectionChecker.ResultChecked -= OnResultReceived;
        }

        // Opens box, compares order, and react accordingly
        private IEnumerator ProcessReceivedCake(bool success)
        {
            yield return
                new WaitForSeconds(reactionData.WaitTimeAfterReceivingBox);
            GameCakeType cakeType = OpenCakeBox(cakeBox);
            yield return
                new WaitForSeconds(reactionData.WaitTimeAfterOpeningBox);

            /* Remove cakebox from the scene and reset it */
            GameObjectsManagement.ResetCakeBoxObj(cakeBox);

            if (success)
            {
                SetExpression(gameObject, Expression.Happy);
                yield return
                 new WaitForSeconds(reactionData.WaitTimeAfterSpawningExpression);
                PrepareCustomerToLeave(gameObject);
                LeaveTip(gameObject, tipPrefab);
            }
            else
            {
                SetExpression(gameObject, Expression.Sad);
                yield return
                 new WaitForSeconds(reactionData.WaitTimeAfterSpawningExpression);
                PrepareCustomerToLeave(gameObject);
            }

            OnReactionEnded();
        }

        // sad face, ready to leave
        private IEnumerator ProcessPatienceRunOut()
        {
            SetExpression(gameObject, Expression.Sad);
            yield return
                new WaitForSeconds(reactionData.WaitTimeAfterSpawningExpression);
            PrepareCustomerToLeave(gameObject);

            OnReactionEnded();
        }

        // Event listener
        private void OnResultReceived(bool[] results)
        {
            if (!gameObject.activeSelf) return;

            StartCoroutine(ProcessReceivedCake(results[customerManager.BeltInd]));
        }

        // Event listener - once patience runs out
        public void OnPatienceEnded()
        {
            StartCoroutine(ProcessPatienceRunOut());
        }

        // Removes order & sets expression for the customer
        private void SetExpression(GameObject gameObject,
            Expression expression)
        {
            string label = gameObject.GetComponent<SpriteResolver>().GetLabel();
            Costume.SetCostume(gameObject, category: expression.ToString(), label);
        }

        // Open cake box
        private GameCakeType OpenCakeBox(GameObject cakeBox)
        {
            Cake cakeState = cakeBox.GetComponent<CakeBoxController>().cake;
            GameCakeType measuredCakeType = cakeState.MeasureCake();

            cakeBox.transform.Find("Box").gameObject.SetActive(false);

            GameObject cake = cakeBox.transform.Find("ActualCake").gameObject;
            cakeBox.GetComponent<CakeBoxController>()
                .SetMeasuredCakeType(measuredCakeType);
            return measuredCakeType;
        }

        // Deactive patience object, bubble
        // Flip customer-sprite facing direction
        private void PrepareCustomerToLeave(GameObject customer)
        {
            customer.transform.Find("PatienceBar").gameObject.SetActive(false);
            customer.transform.Find("Bubble").gameObject.SetActive(false);

            customer.GetComponent<SpriteRenderer>().flipX = true;
        }

        // Leaves tip
        private void LeaveTip(GameObject gameObject, GameObject tipPrefab)
        {
            Patience p = customerManager.Patience;
            float ratio = p.RemainingPatience / p.MaxPatience;
            int maxTip = (int)customerManager.MaxTip;
            float tipAmount = ((float)maxTip) * ratio;

            GameObject tipObj = Instantiate(tipPrefab,
                gameObject.transform.position, Quaternion.identity);
            tipObj.GetComponent<TipController>().SetTipAmount((int)tipAmount,
                (int)customerManager.BasePay);
        }

        // Notifies all reaction subscribers
        protected virtual void OnReactionEnded()
        {
            if (ReactionEnded != null) ReactionEnded();
        }

    }
}
