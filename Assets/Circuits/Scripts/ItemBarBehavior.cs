using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class ItemBarBehavior : MonoBehaviour
{

    public Transform content;

    public ColorBlock selected;
    public ColorBlock unselected;

    public GameObject eventSystem;

    public Transform gameManager;


    private string selection;

    private void Start()
    {
        foreach (Transform child in content)
        {
            Button currButton = child.gameObject.GetComponent<Button>();
            currButton.colors = unselected;
        }
    }

    public void buttonClick(string name)
    {
        if (name == selection)
            selection = "";
        else
            selection = name;
        foreach (Transform child in content)
        {
            Button currButton = child.gameObject.GetComponent<Button>();
            if (child.name == selection)
            {
                currButton.colors = selected;
            }
            else
            {
                currButton.colors = unselected;
            }
        }

	//gameManager.GetComponent<GameBehavior>().setSelected(selection);
    }

}
