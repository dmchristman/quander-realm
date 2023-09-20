using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LevelSelectorBehavior : MonoBehaviour
{
    // public GameData gameData;
    public GameObject buttonPrefab;
    public GameObject content;
    public GameBehavior gameBehavior;
    private LevelButtonBehavior[] buttons;

    // Start is called before the first frame update
    // After clicking the "Play Button" on the TanglesLair Title Screen, Start is called

    void Start()
    {
        Debug.Log("LevelSelectorBehavior.cs -> Start");
        buttons = new LevelButtonBehavior[CTConstants.N_LEVELS];

        GameData.InitCircuitsSaveData();
        
        Debug.Log("LevelSelectorBehavior.cs -> Looping through N_LEVELS");
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
        Debug.Log("LevelSelectorBehavior.cs -> updateLevels");
        for (int i = 0; i < CTConstants.N_LEVELS; i++)
        {
            buttons[i].init(i, GameData.getCompletedLevels()[i], this);
        }
    }

    public void onLevelSelection(int level)
    {
        Debug.Log("LevelSelectorBehavior.cs -> onLevelSelection");
        GameData.setCurrLevel(level);
        string nextScene = GameData.getNextScene();
        SceneManager.LoadScene(nextScene);


    }

    // Update is called once per frame
    void Update()
    {

    }
}
