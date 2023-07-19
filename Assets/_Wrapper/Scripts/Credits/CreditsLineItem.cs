using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Wrapper
{
    public class CreditsLineItem : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI nameText;
        [SerializeField]
        TextMeshProUGUI titleText;

        public void LoadText(string name, string title)
        {
            nameText.text = name;
            titleText.text = title;
        }
    }
}
