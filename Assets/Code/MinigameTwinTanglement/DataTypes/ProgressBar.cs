using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Labyrinth {
    public class ProgressBar : MonoBehaviour
    {
        private int PL;
        // public HorizontalOrVerticalLayoutGroup HL;
        public GameObject stepObj;
        private string path = "Canvases/CanvasOver/StarCountdown";
        private string[] moveTiles = {"IDEAL", "StepA", "STAR1", "StepB", "STAR2", "StepC", "StepD", "StepE", "STAR3"};

        public void initializeBar(int pathLength) {
            stepObj.GetComponent<HorizontalLayoutGroup>().enabled = true;

            PL = (int)pathLength;

            for (int i=0; i<PL; i++) {
                GameObject.Find($"{path}/Steps/Step{i}").SetActive(true);
            }

            Invoke("horizOff", 1);
        }

        public void resestBar() {
            for (int i=0; i<moveTiles.Length; i++) {
                GameObject.Find($"{path}/Steps/{moveTiles[i]}").SetActive(true);
            }
            for (int j=0; j<16; j++) {
                GameObject.Find($"{path}/Steps/Step{j}").SetActive(false);
            }
        }

        private void horizOff() {
            stepObj.GetComponent<HorizontalLayoutGroup>().enabled = false;
        }

        public void detractBar(int currStep) {
            // stepObj.GetComponent<HorizontalLayoutGroup>().enabled = false;
            
            if (currStep <= PL) {
                GameObject.Find($"{path}/Steps/Step{PL-currStep}").SetActive(false);
            }
            else {
                if ((currStep - PL) > 9) {
                    return;
                }
                GameObject.Find($"{path}/Steps/{moveTiles[currStep-PL-1]}").SetActive(false);
            }
        }
    
    }
}