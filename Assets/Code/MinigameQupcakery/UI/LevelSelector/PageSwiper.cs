using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Reference: https://www.youtube.com/watch?v=rjFgThTjLso&ab_channel=PressStart

// #TODO: BUG! Swiper is not correctly working with EventSystem removed 

namespace Qupcakery
{
    public class PageSwiper : MonoBehaviour, IDragHandler, IEndDragHandler
    {
        Vector3 panelLocation;
        public float percentThreshold = 0.2f;
        public float easing = 0.5f;
        public int totalPages = 1;
        public int currentPage = 1;

        // Start is called before the first frame update
        void Start()
        {
            panelLocation = transform.position;
        }

        public void OnDrag(PointerEventData data)
        {
            float difference = data.pressPosition.x - data.position.x;
            transform.position = panelLocation - new Vector3(difference, 0, 0);
        }

        public void OnEndDrag(PointerEventData data)
        {
            float percentage = (data.pressPosition.x - data.position.x) / Screen.width;
            if (Mathf.Abs(percentage) >= percentThreshold)
            {
                Vector3 newLocation = panelLocation;
                // Debug.LogFormat("Percentage: {0}, currentPage: {1}, totalPages: {2}", percentage, currentPage, totalPages);
                if (percentage > 0 && currentPage < totalPages)
                {
                    currentPage++;
                    newLocation += new Vector3(-Screen.width, 0, 0);
                }
                else if (percentage < 0 && currentPage > 1)
                {
                    currentPage--;
                    newLocation += new Vector3(Screen.width, 0, 0);
                }
                StartCoroutine(SmoothMove(transform.position, newLocation, easing));
                panelLocation = newLocation;
            }
            else
            {
                StartCoroutine(SmoothMove(transform.position, panelLocation, easing));
            }
        }

        IEnumerator SmoothMove(Vector3 startpos, Vector3 endpos, float seconds)
        {
            // Debug.Log("swipping");
            float t = 0f;
            while (t <= 1.0)
            {
                t += Time.deltaTime / seconds;
                transform.position = Vector3.Lerp(startpos, endpos, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
        }

    }
}
