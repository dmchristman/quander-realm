using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Qupcakery
{
    public class HomeButton : MonoBehaviour
    {
        public void GoToHomePage()
        {
            SceneManagementUtilities.LoadHomePage();
        }
    }
}
