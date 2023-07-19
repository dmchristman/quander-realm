using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelSelectorBehavior : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject content;
    public GameBehavior gameBehavior;
    private LevelButtonBehavior[] buttons;
    // Start is called before the first frame update
    void Start()
    {
        buttons = new LevelButtonBehavior[CTConstants.N_LEVELS];
        for (int i = 0; i < CTConstants.N_LEVELS; i++)
        {
            GameObject newButton = Instantiate(buttonPrefab);
            newButton.transform.SetParent(content.transform);
            newButton.transform.localScale = Vector3.one;
            LevelButtonBehavior lb = newButton.GetComponent<LevelButtonBehavior>();
            lb.init(i, GameData.getCompletedLevels()[i], this);
            buttons[i] = lb;
        }

    }

    public void updateLevels()
    {
        for (int i = 0; i < CTConstants.N_LEVELS; i++)
        {
            buttons[i].init(i, GameData.getCompletedLevels()[i], this);
        }
    }

    public void onLevelSelection(int l)
    {
        GameData.setCurrLevel(l);
        string nextScene = GameData.getNextScene();
        SceneManager.LoadScene(nextScene);


    }

    // Update is called once per frame
    void Update()
    {

    }
}
