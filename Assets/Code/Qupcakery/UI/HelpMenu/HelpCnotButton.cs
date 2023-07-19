using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


namespace Qupcakery
{
    public class HelpCnotButton : MonoBehaviour
    {
        public GameObject recipePanel;
        public GameObject startPanel;
        public Image recipeImage;

        public void ShowCnotRecipe()
        {
            startPanel.SetActive(false);
            recipePanel.SetActive(true);
            recipePanel.GetComponentInChildren<Text>().text = "Chocolate-powered Flavor-inverter (CNOT Gate)";

            recipeImage.sprite = Utilities.helpMenuSprites.Where(obj => obj.name == "CNOT").SingleOrDefault();
        }
    }
}
