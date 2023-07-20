using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth 
{ 
    public class StarMessage : MonoBehaviour
    {
        public int levelNum;
        public GameObject[] stars;

        public void displayStars() {
            int numStars = SaveData.Instance.starsPerLevel[levelNum-1];
            
            for (int i=0; i < stars.Length; i++) {
                stars[i].SetActive(false);
            }
            stars[numStars].SetActive(true);
        }
    }
}