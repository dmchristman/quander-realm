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
        Debug.Log("LevelButtonBehavior.cs -> onClick");
        Debug.Log("level:" + level);
        owner.onLevelSelection(level);
    }

    public void init(int level_param, bool completed, LevelSelectorBehavior owner_param)
    {
        owner = owner_param;
        level = level_param;
        
        Debug.Log("LevelButtonBehavior Log, Level:" + level + " completed:" + completed + " owner:" + owner);
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
