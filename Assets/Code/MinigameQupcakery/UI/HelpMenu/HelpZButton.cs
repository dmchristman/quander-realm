using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Qupcakery
{
    public class HelpZButton : MonoBehaviour
    {
        public GameObject recipePanel;
        public GameObject startPanel;
        public Image recipeImage;

        public void ShowZRecipe()
        {
            startPanel.SetActive(false);
            recipePanel.SetActive(true);
            recipePanel.GetComponentInChildren<Text>().text = "Mystery-inverter (Z Gate)";

            recipeImage.sprite = Utilities.helpMenuSprites.Where(obj => obj.name == "Z").SingleOrDefault();
        }
    }
}
