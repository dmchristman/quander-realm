using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth
{
    public class Tutorial : MonoBehaviour
    {
        public TM tutMap1;
        public TM tutMap2;

        public Player p1;
        public Player p2;

        public bool instruction1 = true;
        public bool instruction2 = true;
        public bool instruction3 = true;

        public GameObject instructionPanel;
        public GameObject help1;
        public GameObject help2;
        public GameObject help3;

        private Maze maze;
        private PlayerMovement pm;
        private ButtonBehavior btn;

        public void StartTutorial() {
            maze = GameObject.Find("MazeGen").GetComponent<Maze>();
            pm = GameObject.Find("Players").GetComponent<PlayerMovement>();
            btn = GameObject.Find("GameManagerLocal").GetComponent<ButtonBehavior>();
        }

        void Update() {
            if ((p1.getPloc == new Vector3(0,1,0)) & (instruction1 == true)) {
                // Debug.Log("Here's some help!");
                help1.SetActive(true);
                instruction1 = false;
                Invoke("highlightSwitchButton", 1f);
                Invoke("endHelp1", 3f);
            }
            if ((p1.getPloc == new Vector3(1,0,0)) & (instruction3 == true)) {
                Debug.Log("Here's some helP!!!!");
                help3.SetActive(true);
                instruction3 = false;
                Invoke("endHelp3", 3f);
            }
            if ((p2.getPloc == new Vector3(2,2,0)) & (instruction2 == true)) {
                // Debug.Log("Here's some help! 2");
                help2.SetActive(true);
                instruction2 = false;
                Invoke("highlightSwitchButton", 1f);
                Invoke("endHelp2", 3f);
            }
        }

        public void endHelp1() {
            help1.SetActive(false);
        }

        public void endHelp2() {
            help2.SetActive(false);
        }

        public void endHelp3() {
            help3.SetActive(false);
        }

        public void highlightSwitchButton() {
            GameObject.Find("Canvases/CanvasOver/GameplayButtons/SwitchPlayers").GetComponent<ParticleSystem>().Play();
        }

        public void exitPanel() {
            instructionPanel.SetActive(false);
            GameObject.Find("Canvases/CanvasOver/GameplayButtons").SetActive(true);
        }

    }
}
