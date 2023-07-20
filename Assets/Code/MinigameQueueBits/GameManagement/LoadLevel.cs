using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace QueueBits
{
    public class LoadLevel : MonoBehaviour
    {
        public void loadlevel (string level)
        {
#if LITE_VERSION
    if (level=="Level9") {
        level = "LevelSelect";
    }
#endif
            SceneManager.LoadScene("QB_" + level);
        }
    }
}