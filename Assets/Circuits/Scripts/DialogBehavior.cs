using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class DialogBehavior : MonoBehaviour
{
    public string[] text;
    public Text textbox;
    public string nextScene;
    private int curr_line = 0;
    private int curr_char = 0;
    private string curr_text;

    public void charTimeout() {
        curr_char++;
        curr_line = Mathf.Min(curr_line, text.Length - 1);
        if(curr_char <= text[curr_line].Length) {
            textbox.text = text[curr_line].Substring(0, curr_char);
            
	    }
        else {
            textbox.text = text[curr_line];
	    }
    }

    public void onSkipClick() {
        nextScene = GameData.getNextScene();
        SceneManager.LoadScene(nextScene);
    }

    public void onNextClick() { 
        if(curr_line == text.Length - 1 )
        {
            if (curr_char < text[curr_line].Length)
            {
                curr_char = text[curr_line].Length;
            }
            else
            {
                SceneManager.LoadScene(nextScene);
            }
        }
        else { 
	        if( curr_char >= text[curr_line].Length) {
                curr_char = 0;
                curr_line++;
	        }
            else {
                curr_char = text[curr_line].Length;
	        }
	    }

    }
}
