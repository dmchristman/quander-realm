using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Qupcakery
{
    public class HelpNotButton : MonoBehaviour
    {
        public GameObject recipePanel;
        public GameObject startPanel;
        public Image recipeImage;

        public void ShowNotRecipe()
        {
            startPanel.SetActive(false);
            recipePanel.SetActive(true);
            recipePanel.GetComponentInChildren<Text>().text = "Flavor-inverter (NOT Gate)";

            recipeImage.sprite = Utilities.helpMenuSprites.Where(obj => obj.name == "NOT").SingleOrDefault();
        }
    }
}
