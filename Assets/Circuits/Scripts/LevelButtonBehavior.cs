using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonBehavior : MonoBehaviour
{
    public Text buttonText;
    //public Image star;
    public Sprite starSprite;
    public GameObject panel;
    //public Text scoreText;
    private LevelSelectorBehavior owner;
    private int level;

    public Sprite[] numberSprites;


    public void onClick()
    {
        owner.onLevelSelection(level);
    }

    public void init(int l, bool completed, LevelSelectorBehavior o)
    {
        owner = o;
        level = l;
        buttonText.text = $"{level}";

        Image numberObject = panel.transform.Find("LevelNumber").GetComponent<Image>();
        numberObject.sprite = numberSprites[level];

            GetComponent<Button>().interactable = true;
            //scoreText.text = "";
            if(completed)
            {
                Image starObject = panel.transform.Find($"Star").GetComponent<Image>();
                starObject.sprite = starSprite;
                //scoreText.text += "*";
	        }
    }
}
