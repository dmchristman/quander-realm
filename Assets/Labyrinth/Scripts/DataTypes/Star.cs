using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth 
{ 
    public class Star : MonoBehaviour
    {
        public GameObject uncollected;
        public GameObject collected;
        public int initPath;

        private Maze maze;
        /* private PlayerMovement pm;
        private GameBehavior gb;

        public void Start() {
            maze = GameObject.Find("MazeGen").GetComponent<Maze>();
            pm = GameObject.Find("Players").GetComponent<PlayerMovement>();
            gb = GameObject.Find("GameManagerLocal").GetComponent<GameBehavior>();
        } */

        public void toggleVisibility() {
            if (uncollected.GetComponent<SpriteRenderer>().enabled == true) {
                collected.GetComponent<SpriteRenderer>().enabled = true;
                uncollected.GetComponent<SpriteRenderer>().enabled = false;
                return;
            }
            else if (collected.GetComponent<SpriteRenderer>().enabled == true) {
                uncollected.GetComponent<SpriteRenderer>().enabled = true;
                collected.GetComponent<SpriteRenderer>().enabled = false;
                return;
            }
        }
        
        public void visibilityOff() {
            uncollected.GetComponent<SpriteRenderer>().enabled = true;
            collected.GetComponent<SpriteRenderer>().enabled = false;
        }

        public void resetStar() {
            collected.GetComponent<SpriteRenderer>().enabled = true;
            uncollected.GetComponent<SpriteRenderer>().enabled = false;
        }

        /* public void calcPathFromStart() {
            maze = GameObject.Find("MazeGen").GetComponent<Maze>();
            string path = maze.calcPathToGoal()[0].ToString();
            initPath = path.Length;
        } */
        

    }
}