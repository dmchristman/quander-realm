using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Qupcakery
{
    public class HelpSwapButton : MonoBehaviour
    {
        public GameObject recipePanel;
        public GameObject startPanel;
        public Image recipeImage;

        public void ShowSwapRecipe()
        {
            startPanel.SetActive(false);
            recipePanel.SetActive(true);
            recipePanel.GetComponentInChildren<Text>().text = "Flavor-swapper (SWAP Gate)";

            recipeImage.sprite = Utilities.helpMenuSprites.Where(obj => obj.name == "SWAP").SingleOrDefault();
        }
    }
}
