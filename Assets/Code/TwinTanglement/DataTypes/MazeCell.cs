using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Labyrinth 
{ 
    public class MazeCell : MonoBehaviour
    {
        private int cx;
        private int cy;
        private int tx;
        private int ty;

        private int type;
        public Dictionary<string, bool> walls = new Dictionary<string, bool>() 
            { {"N", true}, {"S", true}, {"W", true}, {"E", true} };
        public int deg;

        public bool goal;
        public bool start;
        public bool visited;

        private GameBehavior gb = GameObject.Find("GameManagerLocal").GetComponent<GameBehavior>(); 
        // private GameBehavior gb = GameBehavior.Instance;

        public MazeCell(int x, int y) {
            cx = x;
            cy = y;
            type = 0;
            visited = false;
            // walls = new Dictionary<string, bool>() 
            //     { {"N", true}, {"S", true}, {"W", true}, {"E", true} };
            goal = false;
            deg = 0;

            // px = (int)(-x + (gb.size - 1)/2); // player coordinates
            // py = (int)(-y + (gb.size - 1)/2);

            tx = (int)(-x + (gb.size-gb.size/4)/2 + (gb.size-1)/2);
            ty = (int)(-y + (gb.size-gb.size/4)/2 + (gb.size-1)/2);
        }

        public MazeCell initMazeCell(int x, int y) {
            cx = x;
            cy = y;
            type = 0;
            visited = false;
            // walls = new Dictionary<string, bool>() 
            //     { {"N", true}, {"S", true}, {"W", true}, {"E", true} };
            goal = false;
            deg = 0;

            // px = (int)(-x + (gb.size - 1)/2); // player coordinates
            // py = (int)(-y + (gb.size - 1)/2);

            tx = (int)(-x + (gb.size-gb.size/4)/2 + (gb.size-1)/2);
            ty = (int)(-y + (gb.size-gb.size/4)/2 + (gb.size-1)/2);

            return this;
        }

        public int getX {
            get { return cx; }
        }

        public int getY {
            get { return cy; }
        }

        public int getTx {
            get { return tx; }
        }

        public int getTy {
            get { return ty; }
        }

        // public Vector3 getTloc {
        //     get { return ( new Vector3(tx, ty, 0)); }
        // }

        public int getType {
            get { return type; }
        }

        public bool getGoal {
            get { return goal; }
        }
        
        public void setDeg(int degree) {
            deg = degree;
            return;
        }

        public string toString() {
            return $"Tile {cx}, {cy}";
        }

        public string getWalls() {
            return $"N: {walls["N"]}, S: {walls["S"]}, E: {walls["E"]}, W: {walls["W"]}";
        }

        public bool isCave() {
            int count = 0;
            foreach (KeyValuePair<string,bool> dir in walls) {
                if (dir.Value == true) {
                    count++;
                }
            }
            return (count==3);
        }

        public void toggleGoal(bool val) {
            goal = val;
        }

        public void toggleStart(bool val) {
            start = val;
        }

        // now with 4 wall types instead of 16
        public void setType(int degree) {
            if (this.getWalls() == "N: True, S: True, E: True, W: True") {
                type = 0; }     //NSEW
            else if (this.getWalls() == "N: True, S: True, E: True, W: False") {
                if (degree == 0) { type = 1; }   //NSE
                else if (degree == 90) { type = 0; }
                else if (degree == 180) { type = 0; }
            }
            else if (this.getWalls() == "N: True, S: True, E: False, W: True") {
                if (degree == 0) { type = 0; }     //NSW
                else if (degree == 90) { type = 2; }
                else if (degree == 180) { type = 1; }
            }   
            else if (this.getWalls() == "N: True, S: True, E: False, W: False") {
                if (degree == 0) { type = 1; }     //NS
                else if (degree == 90) { type = 2; }
                else if (degree == 180) { type = 1; }
            }
            else if (this.getWalls() == "N: True, S: False, E: True, W: True") {
                if (degree == 0) { type = 0; }     //NEW
                else if (degree == 90) { type = 0; }
                else if (degree == 180) { type = 2; } //DOUBLE CHECK
            }
            else if (this.getWalls() == "N: True, S: False, E: True, W: False") {
                if (degree == 0) { type = 1; }     //NE
                else if (degree == 90) { type = 0; }
                else if (degree == 180) { type = 2; }
            }
            else if (this.getWalls() == "N: True, S: False, E: False, W: True") {
                if (degree == 0) { type = 0; }     //NW
                else if (degree == 90) { type = 2; }
                else if (degree == 180) { type = 3; }
            }
            else if (this.getWalls() == "N: True, S: False, E: False, W: False") {
                if (degree == 0) { type = 1; }     //N
                else if (degree == 90) { type = 2; }
                else if (degree == 180) { type = 3; }
            }
            else if (this.getWalls() == "N: False, S: True, E: True, W: True") {
                if (degree == 0) { type = 2; }     //SEW
                else if (degree == 90) { type = 1; }
                else if (degree == 180) { type = 0; }
            }
            else if (this.getWalls() == "N: False, S: True, E: True, W: False") {
                if (degree == 0) { type = 3; }     //SE
                else if (degree == 90) { type = 1; }
                else if (degree == 180) { type = 0; }
            }
            else if (this.getWalls() == "N: False, S: True, E: False, W: True") {
                if (degree == 0) { type = 2; }    //SW
                else if (degree == 90) { type = 3; }
                else if (degree == 180) { type = 1; }
            }
            else if (this.getWalls() == "N: False, S: True, E: False, W: False") {
                if (degree == 0) { type = 3; }    //S
                else if (degree == 90) { type = 3; }
                else if (degree == 180) { type = 1; }
            }
            else if (this.getWalls() == "N: False, S: False, E: True, W: True") {
                if (degree == 0) { type = 2; }    //EW
                else if (degree == 90) { type = 1; }
                else if (degree == 180) { type = 2; }
            }
            else if (this.getWalls() == "N: False, S: False, E: True, W: False") {
                if (degree == 0) { type = 3; }    //E
                else if (degree == 90) { type = 1; }
                else if (degree == 180) { type = 2; }
            }
            else if (this.getWalls() == "N: False, S: False, E: False, W: True") {
                if (degree == 0) { type = 2; }    //W
                else if (degree == 90) { type = 3; }
                else if (degree == 180) { type = 3; }
            }
            else if (this.getWalls() == "N: False, S: False, E: False, W: False") {
                type = 3; }    //none
            else {
                type = 0; }
            return;
        }


    }
}