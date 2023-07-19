using UnityEngine;
using System.Collections;

namespace Qupcakery
{
    public class TipController : MonoBehaviour
    {
        public delegate void OnCoinsCollectedEventHandler(int tipAmount);
        public event OnCoinsCollectedEventHandler CoinsCollected;

        public int TotalPay { get; private set; }

        private void Start()
        {
            CoinsCollected += UIProgressBar.Instance.OnCoinsCollected;
        }

        public void SetTipAmount(int tipAmount, int basePay)
        {
            TotalPay = tipAmount + basePay;
            StartCoroutine(CollectTip());
        }

        // Start moving the tip to the progress bar icon
        private IEnumerator CollectTip()
        {
            // Debug.Log("Start colleting tip!");
            //yield return new WaitForSeconds(0.7f);

            //Vector3 startPos = gameObject.GetComponent<Transform>().position;
            //float time = 0f, speed = 2f;
            //while (time <= 1f)
            //{
            //    time += speed * Time.deltaTime;
            //    //gameObject.GetComponent<Transform>().position =
            //    //    Vector2.Lerp(startPos, UICoinIconPosition, Mathf.SmoothStep(0f, 1f, time));
            //    yield return new WaitForEndOfFrame();
            //}
            yield return new WaitForSeconds(0.5f);

            OnCoinsCollected(TotalPay);
            Destroy(gameObject);
        }

        protected virtual void OnCoinsCollected(int amount)
        {
            if (CoinsCollected != null)
            {
                CoinsCollected(amount);
            }
        }
    }
}
