using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameBehavior : MonoBehaviour
{
    public Text textbox;

    //private LevelGenerator levelGen;

    public Color selected;

    public float timeForHint = 5.0f;

    public GameObject gatesObject;
    public GameObject canvasObject;

    //public GameObject[] stars;

    public Camera camera;

    public GameObject sparkPrefab;

    public GameObject linePrefab;


    public Transform linesTransform;
    //private HashSet<LinkedListNode<GateData>> selection = new HashSet<LinkedListNode<GateData>>();

    private float timeToNextLevel = float.MaxValue;
    private float hintTimer = 0;

    private const float TIME_BUFFER = 4f;

    private bool hintProvided = false;

    private int[] levelScores = new int[CTConstants.N_LEVELS];

    private float sceneScale = 1f;

    //public GameObject mainGUI;

    //private bool started = false;

    private bool hintUsed = false;

    private int sortingIndexOffset = 0;
    private List<List<String>> circuit;
    private HashSet<BaseGateBehavior> selection;

    private BaseGateBehavior[,] gateObjects;

    bool simplified = false;

    // public bool titleScene = false;
    public Sprite[] numberSprites;


    private void renderCircuit(List<List<String>> newCircuit)
    {
        foreach (Transform child in gatesObject.transform)
        {
            Destroy(child.gameObject);
        }
        gatesObject.transform.localScale = Vector3.one;
        selection = new HashSet<BaseGateBehavior>();
        circuit = newCircuit;
        int circuitSize = circuit[0].Count;
        int nLines = circuit.Count;
        int nCols = circuit[0].Count;
        //camera.orthographicSize = Math.Max(circuitSize * 2.5f, 25);



        var loadedGate = Resources.Load("Circuits/Prefabs/H_Gate");
        gateObjects = new BaseGateBehavior[circuit.Count, circuit[0].Count];
        Vector3 offset = new Vector3((-nCols / 2) * CTConstants.gridResolution_w, ((-nLines / 2) - .5f) * CTConstants.gridResolution_h);
        for (int i = 0; i < circuit.Count; i++)
        {
            string row = "";
            for (int j = 0; j < circuit[i].Count; j++)
            {
                row += circuit[i][j];
                String gateSelected = circuit[i][j];
                if (circuit[i][j] == null) { gateSelected = ""; }
                gateSelected = gateSelected.ToUpper();
                switch (gateSelected)
                {
                    case "H-0":
                        instantiateGate("Circuits/Prefabs/H_Gate", j, i, nLines, circuit[i].Count);
                        break;
                    case "Z-0":
                        instantiateGate("Circuits/Prefabs/Z_Gate", j, i, nLines, circuit[i].Count);
                        break;
                    case "X-0":
                        instantiateGate("Circuits/Prefabs/NOT_Gate", j, i, nLines, circuit[i].Count);
                        break;
                    case "CX-0":
                        instantiateGate("Circuits/Prefabs/CNOT_Gate", j, i, nLines, circuit[i].Count);
                        break;
                    case "CZ-0":
                        instantiateGate("Circuits/Prefabs/CZ_Gate", j, i, nLines, circuit[i].Count);
                        break;
                    case "SWAP-0":
                        instantiateGate("Circuits/Prefabs/SWAP_Gate", j, i, nLines, circuit[i].Count);
                        break;
                    default:
                        //Debug.Log("No Match for: " + gateSelected);
                        break;
                }
            }
        }

        gatesObject.transform.localScale = Vector3.one * sceneScale;
    }
    private void Start()
    {
        Debug.Log("GameBehavior.cs -> starting");
        // if (titleScene=true) {
        //     titleScene=false;
        //     return;
        // }
        GameData.levelStart();
        selection = new HashSet<BaseGateBehavior>();
        String[] gatesToSample;
        String[] allowedSubstitutions;
        int nLines = 1;
        int nGates;
        int nExpansions;
        List<List<String>> tempCircuit = null;
        try
        {
			Wrapper.Events.CollectAndDisplayReward?.Invoke(Wrapper.Game.Circuits, GameData.getCurrLevel());
        }
        catch (Exception ex)
        {

        }

        Image numberObject = GameObject.Find("Canvas/LevelNumber/Number").GetComponent<Image>();
        if (GameData.getCurrLevel() < CTConstants.N_LEVELS) {
            numberObject.sprite = numberSprites[GameData.getCurrLevel()];
        }

        if (GameData.getCurrLevel() > CTConstants.N_LEVELS)
        {
            SceneManager.LoadScene("Circuits_Menu");
            return; // sorry david :/
        }
        else if (GameData.getCurrLevel() <= 9)
        {
            String[] empty = new String[0];
            int startingSize = 9;
            tempCircuit = new List<List<string>>(3);
            tempCircuit.Add(new List<String>(startingSize));
            List<String> row = tempCircuit[0];
            const string H = "H-0";
            const string X = "X-0";
            const string Z = "Z-0";
            switch (GameData.getCurrLevel())
            {
                case 0:
                    Wrapper.Events.StartDialogueSequence?.Invoke("CT_Intro");
                    row.Add(H);
                    row.Add(H);
                    break;
                case 1:
                    row.Add(H);
                    row.Add(H);
                    row.Add(H);
                    break;
                case 2:
                    row.Add(Z);
                    row.Add(X);
                    row.Add(H);
                    row.Add(H);
                    break;
                case 3:
                    row.Add(H);
                    row.Add(X);
                    row.Add(H);
                    row.Add(Z);
                    break;
                case 4:
                    row.Add(Z);
                    row.Add(H);
                    row.Add(H);
                    row.Add(X);
                    row = new List<string>();
                    tempCircuit.Add(row);
                    row.Add(H);
                    row.Add(X);
                    row.Add(H);
                    break;
                case 5:
                    row.Add(H);
                    row.Add(X);
                    row.Add(H);
                    row.Add(X);
                    row.Add(H);
                    row.Add(X);
                    row.Add(H);
                    row = new List<string>();
                    tempCircuit.Add(row);
                    row.Add(Z);
                    row.Add(X);
                    row.Add(H);
                    row.Add(H);
                    break;
                case 6:
                    row.Add(H);
                    row.Add(X);
                    row.Add(Z);
                    row.Add(H);
                    row.Add(X);
                    row.Add(H);
                    row.Add(Z);
                    row.Add(H);
                    row.Add(Z);
                    row.Add(H);
                    row.Add(Z);
                    row.Add(H);
                    break;
                case 7:
                    row.Add(H);
                    row.Add(Z);
                    row.Add(H);
                    row.Add(X);
                    row.Add(Z);
                    row.Add(H);
                    row.Add(X);
                    row = new List<string>();
                    tempCircuit.Add(row);
                    row.Add(H);
                    row.Add(X);
                    row.Add(H);
                    break;
                case 8:
                    row.Add(H);
                    row.Add(H);
                    row.Add(X);
                    row.Add(Z);
                    row = new List<string>();
                    tempCircuit.Add(row);
                    row.Add(H);
                    row.Add(Z);
                    row.Add(H);
                    row = new List<string>();
                    tempCircuit.Add(row);
                    row.Add(X);
                    row.Add(Z);
                    row.Add(H);
                    row.Add(X);
                    row.Add(H);
                    row.Add(Z);
                    row.Add(X);

                    break;
                case 9:
                    row.Add(H);
                    row.Add(X);
                    row.Add(X);
                    row.Add(H);
                    row.Add(X);
                    row.Add(H);
                    row.Add(Z);
                    row.Add(H);
                    row.Add(X);
                    row.Add(H);
                    row.Add(Z);
                    row.Add(H);
                    row.Add(X);
                    break;

                default:
                    break;
            }

            // Since we are building these circuits by hand we need to make sure that 
            // all rows are the same length
            int circuitLen = 0;
            foreach (var currRow in tempCircuit)
            {
                circuitLen = Math.Max(circuitLen, currRow.Count);
            }

            foreach (var currRow in tempCircuit)
            {
                for (int i = currRow.Count; i < circuitLen; i++)
                {
                    currRow.Add(null);
                }
            }
        }
        else if (GameData.getCurrLevel() < 14)
        {
            System.Random rng = new System.Random();
            nLines = rng.Next(2, 5);
            nGates = GameData.getCurrLevel() + rng.Next(4) + (int)(nLines / 2);
            gatesToSample = new string[] { "X", "Z" };
            allowedSubstitutions = new string[] { "X", "Z" };
            nExpansions = 3 + rng.Next(4);
            tempCircuit = LevelGenerator.GenerateLevel(nLines, nGates, gatesToSample, nExpansions, allowedSubstitutions);
        }
        else if (GameData.getCurrLevel() < 18)
        {
            System.Random rng = new System.Random();
            nLines = rng.Next(4, 5);
            nGates = GameData.getCurrLevel() + rng.Next(4) + (int)(nLines / 2) - 5;
            gatesToSample = new string[] { "X", "Z", "CZ" };
            allowedSubstitutions = new string[] { "X", "Z", "CZ" };
            nExpansions = 4 + rng.Next(4);
            tempCircuit = LevelGenerator.GenerateLevel(nLines, nGates, gatesToSample, nExpansions, allowedSubstitutions);
        }
        else if (GameData.getCurrLevel() < 22)
        {
            System.Random rng = new System.Random();
            nLines = rng.Next(5, 7);
            nGates = GameData.getCurrLevel() + rng.Next(4) + (int)(nLines / 2) - 8;
            gatesToSample = new string[] { "X", "Z", "CZ", "CX" };
            allowedSubstitutions = new string[] { "X", "Z", "CZ", "CX" };
            nExpansions = 5 + rng.Next(4);
            tempCircuit = LevelGenerator.GenerateLevel(nLines, nGates, gatesToSample, nExpansions, allowedSubstitutions);
        }
	    else
        {
            System.Random rng = new System.Random();
            nLines = rng.Next(6, 7);
            nGates = GameData.getCurrLevel() + rng.Next(4) + (int)(nLines / 2) - 10;
            gatesToSample = new string[] { "X", "Z", "CZ", "CX" };
            allowedSubstitutions = new string[] { "X", "Z", "CZ", "CX", "CX2" };
            nExpansions = 5 + rng.Next(4);
            tempCircuit = LevelGenerator.GenerateLevel(nLines, nGates, gatesToSample, nExpansions, allowedSubstitutions);


        }
        sceneScale = Math.Min(Math.Min(1f, 6.5f / tempCircuit[0].Count), 3.5f / nLines);
        renderCircuit(tempCircuit);

        int nCols = circuit[0].Count;
        nLines = circuit.Count;
        Vector3 offset = new Vector3((-nCols / 2) * CTConstants.gridResolution_w, ((-nLines / 2) - .5f) * CTConstants.gridResolution_h * sceneScale);
        for (int i = 0; i < circuit.Count; i++)
        {
            var currLine = Instantiate(linePrefab);
            LineRenderer lr = currLine.GetComponent<LineRenderer>();
            lr.SetWidth(sceneScale, sceneScale);
            float yCord = (nLines - i) * CTConstants.gridResolution_h * sceneScale;

            Vector3[] positions = { new Vector3(-200, yCord) + offset, new Vector3(200, yCord) + offset };
            lr.SetPositions(positions);
            currLine.transform.parent = linesTransform;
        }
    }

    private void instantiateGate(String resPath, int x, int y, int nLines, int nCols)
    {
        var loadedGate = Resources.Load(resPath);
        GameObject currGate = (GameObject)Instantiate(loadedGate);
        BaseGateBehavior gb = currGate.GetComponent<BaseGateBehavior>();
        gb.x = x;
        gb.y = y;
        gb.gameBehavior = this;
        currGate.transform.parent = gatesObject.transform;
        Vector3 offset = new Vector3((-nCols / 2) * CTConstants.gridResolution_w, (-nLines / 2) * CTConstants.gridResolution_h);
        currGate.transform.position = new Vector3(CTConstants.gridResolution_w * x, CTConstants.gridResolution_h * (nLines - y - 1)) + offset;
        gateObjects[y, x] = gb;
    }

    public void goToLevel(int l)
    {
        //GameData.currLevel = l;
        //levelGen = GetComponent<LevelGenerator>();
        //levelGen.genLevel(currLevel);

        //mainGUI.SetActive(false);

    }

    public int getScore(int i)
    {
        return levelScores[i];
    }

    public void onMenuClicked()
    {
        //selection = new HashSet<LinkedListNode<GateData>>();
        //mainGUI.SetActive(true);
    }

    public void toggleGate(BaseGateBehavior gate)
    {
        if (gate.selected)
        {
            selection.Add(gate);
        }
        else
        {
            selection.Remove(gate);
        }
    }



    public void checkSubstitution()
    {
        HashSet<Tuple<int, int>> selectedCords = new HashSet<Tuple<int, int>>();
        if (selection.Count == 0)
        {
            return;
        }
        List<string> subString = new List<string>();
        foreach (var gate in selection)
        {
            Tuple<int, int> cords = new Tuple<int, int>(gate.x, gate.y);
            selectedCords.Add(cords);
            subString.Add(string.Format("({0}:{1},{2})", circuit[gate.y][gate.x], gate.x, gate.y));

        }
        GameData.checkingSub(String.Join("_", subString));
        var simplifiedCircuit = LevelGenerator.checkSubstitution(selectedCords, circuit);
        if (simplifiedCircuit != null)
        {

            GameData.correctSub();
            renderCircuit(simplifiedCircuit);
            Debug.Log("!Valid Substiution!");
        }
        else
        {
            GameData.incorrectSub();
        }
    }


    public void tryRun()
    {
        GameData.levelRun();
        System.Random rng = new System.Random();
        simplified = true;
        for (int y = 0; y < circuit.Count; y++)
        {
            for (int x = 0; x < circuit[y].Count; x++)
            {
                string currGate = circuit[y][x];
                if (currGate != null && currGate[currGate.Length - 1] == '0')
                {
                    var reductions = LevelGenerator.checkGateReduction(x, y, circuit);
                    if (reductions.Count > 0)
                    {
                        simplified = false;
                        break;
                    }
                }
            }
            if (!simplified)
            {
                break;
            }
        }

        float sparkSeparation = CTConstants.gridResolution_w;

        int nLines = circuit.Count;
        int nCols = circuit[0].Count;
        int sparksToSend = 4;

        for (int nSpark = 0; nSpark < sparksToSend; nSpark++)
        {


            Vector3 offset = new Vector3(-nSpark * CTConstants.gridResolution_w, ((-nLines / 2) - .5f) * CTConstants.gridResolution_h) * sceneScale;
            for (int i = 0; i < circuit.Count; i++)
            {
                float sparkOffset = 0;
                float yCord = (nLines - i) * CTConstants.gridResolution_h * sceneScale;
                GameObject spark = Instantiate(sparkPrefab);
                spark.transform.localScale = Vector3.one * sceneScale;
                float circuitLen = 4f * camera.orthographicSize;
                spark.transform.position = new Vector3(-20f, yCord) + offset + new Vector3(sparkOffset, 0);

                float speed = circuitLen / 4f;
                float distance = simplified ? circuitLen * 1.5f : ((float)rng.NextDouble() * .5f + .15f) * (circuitLen / 2);
                spark.GetComponent<SparkBehavior>().runSpark(circuitLen / 3f, distance, GetComponent<TimerManager>());


            }

        }
        if (simplified)
        {
            GTimer nextLevel = GetComponent<GTimer>();
            nextLevel.startTimer();
        }
    }




    private void updateSelection(BaseGateBehavior gate)
    {
        gate.toggle();
    }

    public void flashHint()
    {
        for (int y = 0; y < circuit.Count; y++)
        {
            for (int x = 0; x < circuit[y].Count; x++)
            {
                string currGate = circuit[y][x];
                if (currGate != null && currGate[currGate.Length - 1] == '0')
                {
                    var reductions = LevelGenerator.checkGateReduction(x, y, circuit);
                    if (reductions.Count > 0)
                    {
                        //if (stars[0].active)
                        //{
                        //    stars[0].active = false;
                        //}
                        //else
                        //{
                        //    stars[1].active = false;
                        //}/star


                        GameData.hintRequested();
                        var reduction = reductions[0];
                        foreach (var keyvalue in reduction)
                        {
                            Tuple<int, int> cords = keyvalue.Key;
                            gateObjects[cords.Item2, cords.Item1].highlight();
                        }
                        return;
                    }
                }
            }
        }
    }


    public void toMenu()
    {
        SceneManager.LoadScene("Circuits_Menu");
    }

    public void toTitle()
    {
        SceneManager.LoadScene("Circuits_Title");
    }

    public void loadNextLevel()
    {
        GameData.levelPassed();

        SceneManager.LoadScene(GameData.getNextScene());
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (hit.collider)
            {
                updateSelection(hit.collider.transform.parent.parent.GetComponent<BaseGateBehavior>());
            }
        }
    }
}
