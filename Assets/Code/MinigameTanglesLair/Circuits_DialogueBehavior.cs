using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Circuits_DialogueBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Circuits_DialogueBehavior.cs -> Dialogue Behavior Start");
        int offset = 0;

        GameData.InitTanglesLairSaveData();
        Debug.Log(GameData.getCurrLevel());

        switch (GameData.getCurrLevel() - offset)
        {
            case 0:
                Wrapper.Events.StartDialogueSequence?.Invoke("CT_Intro");
                break;
            case 3:
                Wrapper.Events.StartDialogueSequence?.Invoke("CT_Level3");
                break;
            case 6:
                Wrapper.Events.StartDialogueSequence?.Invoke("CT_Level6");
                break;
            //    return "DialogC-02";
            // case 9:
            case 7:
                Wrapper.Events.StartDialogueSequence?.Invoke("CT_Level9");
                break;
            case 14:
                Wrapper.Events.StartDialogueSequence?.Invoke("CT_Level14");
                break;
            //    return "DialogC-03B";
            case 18:
                Wrapper.Events.StartDialogueSequence?.Invoke("CT_Level18");
                break;
            //    return "DialogC-03A";
            case 22:
                Wrapper.Events.StartDialogueSequence?.Invoke("CT_Level22");
                break;
            //    return "DialogC-04";
            case 25:
                Wrapper.Events.StartDialogueSequence?.Invoke("CT_Level25");
                break;
            //    return "DialogC-04";
            default:
                SceneManager.LoadScene("CircuitsLevelScene");
                // SceneManager.LoadScene("Circuits_Title");
                break;
	    }
    }

    private void OnEnable()
    {
        Wrapper.Events.DialogueSequenceEnded += UnPauseGame;
    }

    private void OnDisable()
    {
        Wrapper.Events.DialogueSequenceEnded -= UnPauseGame;
    }

    private void UnPauseGame()
    {
        // UnPause code
        //Debug.Log("Unpaused?");

        SceneManager.LoadScene("CircuitsLevelScene");
        // SceneManager.LoadScene("Circuits_Title");
    }

}
