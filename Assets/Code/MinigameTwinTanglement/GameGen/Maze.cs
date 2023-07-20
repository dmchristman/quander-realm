using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Labyrinth 
{ 
    public class Maze : MonoBehaviour
    {
        private int size;
        private int deg; 

        private MazeCell[,] maze;
        private MazeCell[,] maze1;
        private MazeCell[,] maze2;

        public TM map1;
        public TM map2;
        // public TM map3;

        public Tile goalTile;
        public Tile startTile;
        // public Tile bottomTile;
        public Tile[] bottomTiles;
        public Tile[] wallTiles;
        public Tile overlayTile;

        private Vector3 currGoal;

        private GameBehavior gb;
        private PlayerMovement pm;

        private Dictionary<string, string> opposites = new Dictionary<string, string>()
        { {"N", "S"}, {"W", "E"}, {"E", "W"}, {"S", "N"} };


    // ~~~~~~~~~~~~~~~ INITIALIZING ~~~~~~~~~~~~~~~

        public void StartMaze() {
            gb = GameObject.Find("GameManagerLocal").GetComponent<GameBehavior>();
            pm = GameObject.Find("Players").GetComponent<PlayerMovement>();

            deg = gb.degree;
            size = gb.size;

            generateMazes(); //(size, size);
            renderMazes();

            int[] goalcoors = GetGoalPosition();
        }

        public MazeCell[,] MazeObj(int size)
        {
            MazeCell[,] maze = new MazeCell[size, size];
            for (int j = 0; j < size; j++) {
                for (int i = 0; i < size; i++) {
                    maze[i, j] = new MazeCell(i, j);
                }
            }
            return maze;
        }

        public MazeCell[,] CloneMaze(MazeCell[,] maze) {
            MazeCell[,] newMaze = new MazeCell[size, size];
            for (int x=0; x < size; x++) {
                for (int y=0; y < size; y++) {
                    newMaze[x,y] = new MazeCell(x,y);
                    // newMaze[x,y](x,y);
                    newMaze[x,y].walls["N"] = maze[x,y].walls["N"];
                    newMaze[x,y].walls["S"] = maze[x,y].walls["S"];
                    newMaze[x,y].walls["E"] = maze[x,y].walls["E"];
                    newMaze[x,y].walls["W"] = maze[x,y].walls["W"];
                }
            }
            return newMaze;
        }




    // ~~~~~~~~~~~~~~~ GENERATING ~~~~~~~~~~~~~~~

        public void makeBaseMaze(MazeCell[,] maze)
        {
            int randx = Random.Range(0, size);
            int randy = Random.Range(0, size);
            MazeCell startTile = maze[randx, randy];
            List<object[]> wallList = new List<object[]>();
            wallList.Add(new object[] { randx, randy, "N" });
            wallList.Add(new object[] { randx, randy, "S" });
            wallList.Add(new object[] { randx, randy, "E" });
            wallList.Add(new object[] { randx, randy, "W" });
            startTile.visited = true;
            
            while (wallList.Count > 0)
            {
                int randIndex = Random.Range(0, wallList.Count);
                int tempx = (int)wallList[randIndex][0];
                int tempy = (int)wallList[randIndex][1];
                MazeCell oldTile = maze[tempx, tempy];
                
                string dir = (string)wallList[randIndex][2];

                int[] coors = GetNeighbor(maze, oldTile, dir);

                if (coors[0] >= 0 && coors[1]!=-1) {
                    MazeCell newTile = maze[coors[0], coors[1]];

                    if (newTile.visited == false) {
                        newTile.visited = true;
                        oldTile.walls[dir] = false;
                        newTile.walls[opposites[dir]] = false;

                        wallList.Add(new object[] { newTile.getX, newTile.getY, "N" });
                        wallList.Add(new object[] { newTile.getX, newTile.getY, "S" });
                        wallList.Add(new object[] { newTile.getX, newTile.getY, "E" });
                        wallList.Add(new object[] { newTile.getX, newTile.getY, "W" });
                    }
                }
                wallList.RemoveAt(randIndex);
            }
        }

        public void generateMazes() {
            maze = MazeObj(size);
            makeBaseMaze(maze);
            maze1 = CloneMaze(maze);
            maze2 = CloneMaze(maze);
            genMirrorMazes(maze1, maze2);
        }

        public void genMirrorMazes(MazeCell[,] maze1, MazeCell[,] maze2) {
            List<object[]> nonWallList = new List<object[]>();

            for (int x=0; x < size; x++) {
                for (int y=0; y < size; y++) {
                    MazeCell currTile = maze1[x,y];
                    foreach (KeyValuePair<string,bool> direction in currTile.walls) {
                        if (direction.Value == false) {
                            nonWallList.Add(new object[] { currTile.getX, currTile.getY, direction.Key });
                        }
                    }
                }
            }
            //int wallcount = 0;
            while (nonWallList.Count > 0) {
                int randIndex = Random.Range(0, nonWallList.Count);
                int tempx = (int)nonWallList[randIndex][0];
                int tempy = (int)nonWallList[randIndex][1];
                
                MazeCell oldTile1 = maze1[tempx, tempy];
                MazeCell oldTile2 = maze2[tempx, tempy];
                string dir = (string)nonWallList[randIndex][2];
                nonWallList.RemoveAt(randIndex);
                
                int[] coors = GetNeighbor(maze1, oldTile1, dir);
                MazeCell newTile1 = maze1[coors[0],coors[1]];
                MazeCell newTile2 = maze2[coors[0],coors[1]];

                float randFloat = (float)Random.Range(0f,1f);
                if (randFloat < 0.5) {
                    float randFloat2 = (float)Random.Range(0f,1f);
                    if (randFloat2 < gb.wallProb) {
                        if (oldTile1.isCave()==false && newTile1.isCave()==false && 
                        oldTile2.walls[dir] == false && newTile2.walls[opposites[dir]]==false) {
                            oldTile1.walls[dir] = true;
                            newTile1.walls[opposites[dir]] = true;
                            //wallcount++;
                        }
                    }
                    else {
                        if (oldTile2.isCave()==false && newTile2.isCave()==false && 
                        oldTile1.walls[dir] == false && newTile1.walls[opposites[dir]]==false) {
                            oldTile2.walls[dir] = true;
                            newTile2.walls[opposites[dir]] = true;
                            //wallcount++;
                        }
                    }
                }
            }
        }

        public void distributeGoal(MazeCell[,] maze, Player player) {
            maze[0, gb.size-1].toggleStart(true);
            maze[gb.size-1, 0].toggleGoal(true);
            currGoal = new Vector3(gb.size-1, 0, 0);
        }

        public void clearGoal() {
            map1.goal.ClearAllTiles();
            map2.goal.ClearAllTiles();
        }




    // ~~~~~~~~~~~~~~~ PATHFINDING ~~~~~~~~~~~~~~~

        public int[] GetNeighbor(MazeCell[,] maze, MazeCell cell, string dir)
        {
            if (dir == "N") {
                if (cell.getY == 0) {
                    return (new int[] {-1,-1});
                }
                return (new int[] {cell.getX, cell.getY - 1});
            }
            if (dir == "E") {
                if (cell.getX == 0) {
                    return (new int[] {-1,-1});
                }
                return (new int[] {cell.getX - 1, cell.getY});
            }
            if (dir == "W") {
                if (cell.getX == size - 1) {
                    return (new int[] {-1,-1});
                }
                return (new int[] {cell.getX + 1, cell.getY});
            }
            if (dir == "S") {
                if (cell.getY == size - 1) {
                    return (new int[] {-1,-1});
                }
                return (new int[] {cell.getX, cell.getY + 1});
            }
            return (new int[] {-1,-1});
        }

        public string pathfinder(int startx, int starty, int endx, int endy) {

            for (int x=0; x < size; x++) {
                for (int y=0; y < size; y++) {
                    maze[x,y].visited = false;
                }
            }
            List<object[]> pathList = new List<object[]>();
            MazeCell startTile = maze[Mathf.Abs(startx), Mathf.Abs(starty)];
            startTile.visited = true;

            foreach (KeyValuePair<string,bool> direction in startTile.walls) {
                if (direction.Value == false) {
                    pathList.Add(new object[] { startx, starty, direction.Key });
                }
            }

            while (pathList.Count > 0) {
                int tempx = (int)pathList[0][0];
                int tempy = (int)pathList[0][1];

                MazeCell oldTile = maze[Mathf.Abs(tempx), Mathf.Abs(tempy)];
                string dir = (string)pathList[0][2];
                pathList.RemoveAt(0);

                int[] tempCoors = GetNeighbor(maze, oldTile, dir[dir.Length-1].ToString());
                MazeCell newTile = maze[tempCoors[0], tempCoors[1]];

                if (newTile.getX == endx && newTile.getY == endy) {
                    return dir;
                }
                if (newTile.visited == false) {
                    newTile.visited = true;
                    foreach (KeyValuePair<string,bool> direction in newTile.walls) {
                        if (direction.Value == false) {
                            pathList.Add(new object[] { newTile.getX, newTile.getY, dir + direction.Key});
                        }
                    }
                }
            }
            return null;
        }

        public object[] calcPathToGoal() {
            Vector3 start; Vector3 end;
            int degree;
        
            if (pm.player1.current == true) {
            //if (currGoal.z == 1) {
                start = pm.player1.getPloc;
                degree = 0;
            }
            else if (pm.player2.current == true) {
            //else if (currGoal.z == 2) {
                start = pm.player2.getPloc;
                degree = deg;
            }
            else { return new object[] {"X", 0}; }

            end = new Vector3(gb.size-1, 0, 0);

            // int startx = size-1-(int)start.x;
            // int starty = size-1-(int)start.y;
            // int endx = size-1-(int)end.x;
            // int endy = size-1-(int)end.y;

            string hint = pathfinder((int)start.x, (int)start.y, (int)end.x, (int)end.y);
            
            // Debug.Log($"Hint: Start {start}, End {end}, Path {hint}");
            return new object[] {hint, degree};
        }

        public int[] GetGoalPosition() {
            for (int x=0; x < size; x++) {
                for (int y=0; y < size; y++) {
                    if (maze1[x,y].getGoal == true) {
                        return (new int[] {x,y});
                    }
                    else if (maze2[x,y].getGoal == true) {
                        return (new int[] {x,y});
                    }
                }
            }
            return (new int[] {-1,-1});
        }





    // ~~~~~~~~~~~~~~~ RENDERING ~~~~~~~~~~~~~~~

        public void RenderMap(MazeCell[,] maze, TM tm, int deg)
        {
            tm.bottom.ClearAllTiles();
            tm.walls.ClearAllTiles();
            tm.goal.ClearAllTiles();
            tm.overlay.ClearAllTiles();

            float xcent = (size - 1) / 2;
            float ycent = (size - 1) / 2;
            
            for (int x=0; x < size; x++) {
                for (int y=0; y < size; y++) {

                    int tempx = x;
                    int tempy = y;

                    if (deg == 90) {
                        // assuming xcent == ycent because square mazes
                        // tempx = (int)((y - ycent) + xcent);
                        // tempy = (int)(-1*(x - xcent) + ycent);
                        tempx = y;
                        tempy = -x + (size - 1);
                        
                    }
                    else if (deg == 180) {
                        tempx = (-1*x + (size-1));
                        tempy = (-1*y + (size-1));
                    }
                    
                    MazeCell curr = maze[tempx,tempy];
                    curr.setType(deg);

                    tm.bottom.SetTile(new Vector3Int((-x+size/2) + tm.translation(size, xcent), -y+size/2, 0), bottomTiles[Random.Range(0,5)]);
                    tm.walls.SetTile(new Vector3Int((-x+size/2) + tm.translation(size, xcent), -y+size/2, 0), wallTiles[curr.getType]);
                    tm.overlay.SetTile(new Vector3Int((-x+size/2) + tm.translation(size, xcent), -y+size/2, 0), overlayTile);

                    /* if (curr.getPx==0 && curr.getPy==0) {
                        tm.walls.SetTile(new Vector3Int((-x+size/2) + tm.translation(size, xcent), -y+size/2, 0), wallTiles[0]);
                    }
                    else { 
                        tm.walls.SetTile(new Vector3Int((-x+size/2) + tm.translation(size, xcent), -y+size/2, 0), wallTiles[curr.getType]);
                    } */

                    if (x==0 && y==(size-1)) {
                        tm.walls.SetTile(new Vector3Int((-x+size/2) + tm.translation(size, xcent), -(y+1)+size/2, 0), wallTiles[1]);
                        tm.walls.SetTile(new Vector3Int((-(x-1)+size/2) + tm.translation(size, xcent), -y+size/2, 0), wallTiles[2]);
                    }
                    else if (y==(size-1)) {
                        tm.walls.SetTile(new Vector3Int((-x+size/2) + tm.translation(size, xcent), -(y+1)+size/2, 0), wallTiles[1]);
                    }
                    else if (x==0) {
                        tm.walls.SetTile(new Vector3Int((-(x-1)+size/2) + tm.translation(size, xcent), -y+size/2, 0), wallTiles[2]);
                    }

                    if (curr.getGoal == true) {
                        tm.goal.SetTile(new Vector3Int((-x+size/2) + tm.translation(size, xcent), -y+size/2, 0), goalTile);
                    }
                    else if (curr.start == true) {
                        tm.goal.SetTile(new Vector3Int((-x+size/2) + tm.translation(size, xcent), -y+size/2, 0), startTile);
                    }
                }
            }
        }

        public void renderMazes() { 
            for (int x=0; x < size; x++) {
                for (int y=0; y < size; y++) {
                    maze1[x,y].toggleGoal(false);
                    maze2[x,y].toggleGoal(false);
                }
            }
            distributeGoal(maze1, pm.player1);
            distributeGoal(maze2, pm.player2); 
            
            // RenderMap(maze, map3, 0);
            RenderMap(maze1, map1, 0);
            RenderMap(maze2, map2, deg);
        }
    }
}