using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System.Runtime.InteropServices;
public class WebLoadBehavior : MonoBehaviour
{
    
    public string nextScene;
    private int timeout = 100;

#if UNITY_WEBGL == true && UNITY_EDITOR == false
    [DllImport("__Internal")]
    private static extern void GameLoaded(string callback);

    private void Start()
    {
        Debug.Log("WebLoadBehavior.cs -> Start");
        GameLoaded ("loadData");
    }
#endif

    public void loadData(string data)
    {
        Debug.Log("I just got this data:");
        Debug.Log(data);
        //TODO Load Data into memory
        SceneManager.LoadScene(nextScene);
    }
    // Update is called once per frame
    void Update()
    {
        timeout -= 1;
        if(timeout < 0)
        {
            loadData("");
        }
    }
}
