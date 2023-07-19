using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Circuits_ButtonBehavior : MonoBehaviour
{
    public void toMenu()
    {
        SceneManager.LoadScene("Circuits_Menu");
    }

    public void toTitle()
    {
        SceneManager.LoadScene("Circuits_Title");
    }
}
