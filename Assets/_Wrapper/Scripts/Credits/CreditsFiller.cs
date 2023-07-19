using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Wrapper
{
    public class CreditsFiller : MonoBehaviour
    {
        bool loaded = false;

        [SerializeField]
        TextMeshProUGUI[] creditsContainers; // container itself is the tmp object, it contains the line credits
        [SerializeField]
        CreditsLineItem[] creditsLineItems;

        //[SerializeField]
        //TextMeshProUGUI filaContainer;
        //[SerializeField]
        //CreditsLineItem filaLine;
        //[SerializeField]
        //TextMeshProUGUI clientContainer;
        //[SerializeField]
        //CreditsLineItem clientLine;
        [SerializeField]
        UnityEngine.UI.Button backButton;

        private void OnEnable()
        {
            backButton.onClick.AddListener(CloseCredits);
        }

        private void OnDisable()
        {
            backButton.onClick.RemoveListener(CloseCredits);
        }

        public void LoadCredits(Credits credits)
        {
            if (loaded) return;

            int current = -1;
            //TextMeshProUGUI currentOrg = null;
            //CreditsLineItem nameTitlePair = null;

            foreach (var entry in credits.Entries)
            {
                if (string.IsNullOrEmpty(entry.Title))
                {
                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Debug.LogWarning("No data set for this line");
                        continue;
                    }

                    // organization entry
                    current++;

                    if (current >= creditsContainers.Length || creditsContainers[current] == null)
                    {
                        Debug.LogError("Error filling credits, not enough organization categories");
                        return;
                    }

                    creditsContainers[current].text = entry.Name;
                    creditsLineItems[current].gameObject.SetActive(false);
                }
                else
                {
                    // person entry

                    if (current < 0)
                    {
                        Debug.LogError("Cannot load names, need to provide organization first");
                        return;
                    }
                    else if (current >= creditsContainers.Length)
                    {
                        Debug.LogError("Error setting names, invalid setup of creditsLineItems");
                        return;
                    }

                    var newLine = Instantiate(creditsLineItems[current], creditsLineItems[current].transform.parent, false);
                    newLine.gameObject.SetActive(true);
                    newLine.LoadText(entry.Name, entry.Title);
                }
            }
            if (current < creditsContainers.Length - 1)
            {
                for (int i = creditsContainers[current].transform.GetSiblingIndex(); i < creditsContainers[current].transform.parent.childCount; i++)
                {
                    creditsContainers[current].transform.parent.GetChild(i).gameObject.SetActive(false);
                }
            }

            loaded = true;
        }

        void CloseCredits()
        {
            Events.ScreenFadeMidAction?.Invoke(() => gameObject.SetActive(false), 0F);
        }
    }
}