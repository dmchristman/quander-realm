using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class LevelGenerator
{
    static String[] doubleGates = { "CX", "CZ", "SWAP"};
    static String[] tripleGates = { "CXL" };
    static int initialCols = 2;

    private static List<List<String>> reduceCircuit(List<List<String>> oldCircuit, Dictionary<Tuple<int, int>, String> substiution) {
        var circuit = cloneCircuit(oldCircuit);

        foreach (var sub in substiution)
        {
            Tuple<int, int> cords = sub.Key;
            int x = cords.Item1;
            int y = cords.Item2;
            String gate = sub.Value;
            if (gate == null)
            {
                circuit[y][x] = null;
            }
            else {
				int gateHeight = doubleGates.Contains<string>(gate) ? 2 : tripleGates.Contains<string>(gate) ? 3 : 1;
                for (int i = 0; i < gateHeight; i++)
                {
                    circuit[y + i][x] = gate + "-" + i;
                }
	        }

        }

        return circuit;
    }

    private static List<List<String>> cloneCircuit(List<List<String>> oldCircuit) {
        List<List<String>> circuit = new List<List<string>>(oldCircuit.Count);


        for (int y = 0; y < oldCircuit.Count; y++)
        {
			List<String> currCol = new List<string>(oldCircuit[y].Count);
			circuit.Add(currCol);
            for (int x = 0; x < oldCircuit[y].Count; x++)
            {
                circuit[y].Add(oldCircuit[y][x]);
            }
        }
        return circuit;
    }

    public static List<List<String>>  checkSubstitution(HashSet<Tuple<int, int>> selection, List<List<String>> circuit) {
        //TODO if a reduction includes a double or triple gate we need to also sub
        // the CXL-1 and CXL-2 strings when generating the new circuit
        foreach (var cord in selection)
        {

            int x = cord.Item1;
            int y = cord.Item2;
            var currReductions = checkGateReduction(x, y, circuit);
            foreach (var currReduction in currReductions)
            {

                if (currReduction.Count == selection.Count)
                {
                    bool sameReduction = true;
                    foreach (var currCord in selection)
                    {
                        if (!currReduction.ContainsKey(currCord))
                        {
                            sameReduction = false;
                            break;
                        }
                    }
                    if (sameReduction)
                    {
                        return reduceCircuit(circuit, currReduction);
                    }
                }
            }
        }
        return null;
    }

    private static String getGateName(Tuple<int, int> cords, List<List<String>> circuit) {
        if(cords == null) {
            return null;
        }
        int x = cords.Item1;
        int y = cords.Item2;
        string gate =  circuit[y][x];
        gate = gate == null ? null : gate.ToUpper();
        return gate;
    }

    public static List<Dictionary<Tuple<int, int>, String>> checkGateReduction(int x, int y, List<List<String>> circuit) {
        List<Dictionary<Tuple<int, int>, String>> reductions = new List<Dictionary<Tuple<int, int>, string>>(3) ;
        Dictionary<Tuple<int, int>, String> reduction = new Dictionary<Tuple<int, int>, string>() ;

        Tuple<int, int> currCords = new Tuple<int, int>(x, y);
        String currGate = getGateName(currCords, circuit);
        Tuple<int,int> nextGateCords = getNextGate(x, y, circuit);
        string nextGate = getGateName(nextGateCords, circuit);

        Tuple<int, int> prevGateCords = getPrevGate(x, y, circuit);
        string prevGate = getGateName(prevGateCords, circuit);

        Tuple<int, int> belowCords = new Tuple<int, int>(x, y + 1);

        Tuple<int, int> nextBelowGateCords = getNextGate(belowCords, circuit);
        string nextBelowGate = getGateName(nextBelowGateCords, circuit);

        Tuple<int, int> prevBelowGateCords = getPrevGate(belowCords, circuit);
        string prevBelowGate = getGateName(prevBelowGateCords, circuit);


        switch (currGate)
        {
            case "H-0":
                if (nextGate == "H-0")
                {
                    reduction.Add(nextGateCords, null);
                    reduction.Add(currCords, null);

                    reductions.Add(reduction);
                }
                break;
            case "X-0":

                if (nextGate == "H-0" && prevGate == "H-0")
                {
                    reduction.Add(prevGateCords, null);
                    reduction.Add(currCords, "Z");
                    reduction.Add(nextGateCords, null);

                    reductions.Add(reduction);
                }
                if (nextGate == "X-0") {
                    reduction = new Dictionary<Tuple<int, int>, string>();
                    reduction.Add(currCords, null);
                    reduction.Add(nextGateCords, null);

                    reductions.Add(reduction);
		        }
                break;
            case "Z-0":

                if (nextGate == "H-0" && prevGate == "H-0")
                {
                    reduction.Add(prevGateCords, null);
                    reduction.Add(currCords, "X");
                    reduction.Add(nextGateCords, null);

                    reductions.Add(reduction);
                }
                break;
            case "CZ-0":

                if (nextBelowGate == "H-0" && prevBelowGate == "H-0")
                {
                    reduction.Add(prevBelowGateCords, null);
                    reduction.Add(currCords, "CX");
                    reduction.Add(nextBelowGateCords, null);

                    reductions.Add(reduction);
                }
                break;
            case "CX-0":
                if(nextGate == "H-0" && prevGate == "H-0" && 
		            nextBelowGate == "H-0" && prevBelowGate == "H-0") {
                    reduction.Add(nextBelowGateCords, null);
                    reduction.Add(prevBelowGateCords, null);
                    reduction.Add(nextGateCords, null);
                    reduction.Add(prevGateCords, null);
                    reduction.Add(currCords, "CX");


                    reductions.Add(reduction);
		        }
                if (nextBelowGate == "H-0" && prevBelowGate == "H-0")
                {
                    reduction = new Dictionary<Tuple<int, int>, string>();
                    reduction.Add(prevBelowGateCords, null);
                    reduction.Add(currCords, "CZ");
                    reduction.Add(nextBelowGateCords, null);

                    reductions.Add(reduction);
                }
                if (nextBelowGate == "H-0" && prevBelowGate == "H-0")
                {
                    reduction = new Dictionary<Tuple<int, int>, string>();
                    reduction.Add(prevBelowGateCords, null);
                    reduction.Add(currCords, "CZ");
                    reduction.Add(nextBelowGateCords, null);

                    reductions.Add(reduction);
                }
          //      if(prevGate == "CX-0" && nextGate == "CX-0") { 
          //          reduction = new Dictionary<Tuple<int, int>, string>();
          //          reduction.Add(prevGateCords, null);
          //          reduction.Add(currCords, "SWAP");
          //          reduction.Add(nextGateCords, null);

          //          reductions.Add(reduction);
		        //}
                break;

            default:
                break;
        }
        return reductions;
    }

    private static Tuple<int, int> getNextGate(Tuple<int, int> cords, List<List<String>> circuit)
    {
        return getNextGate(cords.Item1, cords.Item2, circuit);
    }

    private static Tuple<int, int> getNextGate(int x, int y, List<List<String>> circuit) {
        if(y < 0 || y >= circuit.Count) { return null; }
	    for (int i = x+1; i < circuit[y].Count; i++)
        {
            string currGate = circuit[y][i];
            if(currGate != null) {
                return new Tuple<int, int>(i, y);
	        }
        }
        return null;
    }

    private static Tuple<int, int> getPrevGate(Tuple<int, int> cords, List<List<String>> circuit)
    {
        return getPrevGate(cords.Item1, cords.Item2, circuit);
    }

    private static Tuple<int, int> getPrevGate(int x, int y, List<List<String>> circuit)
    {
        if(y < 0 || y >= circuit.Count) { return null; }
        if(x < 0 || x >= circuit[0].Count) { return null; }
        for (int i = x - 1; i >= 0; i--)
        {
            string currGate = circuit[y][i];
            if (currGate != null)
            {
                return new Tuple<int, int>(i, y);
            }
        }
        return null;
    }
    private static bool isFree(int x, int y, List<List<String>> circuit) { 
        if(x < 0 || y < 0 || y >= circuit.Count || x >= circuit[0].Count) {
            return false;
	    }
        return circuit[y][x] == null;
    }

    private static string getGate(int x, int y, List<List<String>> circuit)
    {
        if (x < 0 || y < 0 || y >= circuit.Count || x >= circuit[0].Count)
        {
            return null;
        }
        string selection = circuit[y][x];
        selection = selection == null ? "" : selection.ToUpper();
        return selection;

    }

    private static void insertNewCol(int x, List<List<String>> circuit) {
        x = Math.Max(0, x);
        if (x >= circuit[0].Count)
        {
            for (int y = 0; y < circuit.Count; y++)
            {
                circuit[y].Add(null);
            }
        }
        else
        {
            for (int y = 0; y < circuit.Count; y++)
            {
                circuit[y].Insert(x, null);
            }
        }
    }

    private static int insertGateLeft(int x, int y, List<List<String>> circuit, String gate) 
    {
        bool allFree = true;
        int gateHeight = doubleGates.Contains<string>(gate) ? 2 : tripleGates.Contains<string>(gate) ? 3 : 1;
        for (int i = 0; i < gateHeight; i++)
        {
            if(!isFree(x -1, y + i, circuit)) {
                allFree = false;
                break;
	        }
        }
        if (!allFree)
        {
            insertNewCol(x, circuit);
            x++;
        }
        for (int i = 0; i < gateHeight; i++)
        {
			circuit[y+i][x - 1] = gate + "-"+i;
		}
        return x;
    }
    private static void insertGateRight(int x, int y, List<List<String>> circuit, String gate)
    {
        //if (!isFree(x + 1, y, circuit))
        //{
        //    insertNewCol(x + 1, circuit);
        //}
        //circuit[y][x + 1] = gate;

        int gateHeight = doubleGates.Contains<string>(gate) ? 2 : tripleGates.Contains<string>(gate) ? 3 : 1;
        bool allFree = true;
        for (int i = 0; i < gateHeight; i++)
        {
            if (!isFree(x + 1, y + i, circuit))
            {
                allFree = false;
                break;
            }
        }
        if (!allFree)
        {
            insertNewCol(x + 1, circuit);
        }
        for (int i = 0; i < gateHeight; i++)
        {
            circuit[y + i][x + 1] = gate + "-" + i;
        }
    }
    private static int surroundWithGate(int x, int y, List<List<String>> circuit, String gate)
    {
        //if (!isFree(x - 1, y, circuit))
        //{
        //    insertNewCol(x, circuit);
        //    x++;
        //}
        //circuit[y][x - 1] = gate;
        x = insertGateLeft(x, y, circuit, gate);
        insertGateRight(x, y, circuit, gate);
        return x;
    }
    private static int surroundWithH(int x, int y, List<List<String>> circuit) {
        return surroundWithGate(x, y, circuit, "H");
    }

    public static List<List<String>> GenerateLevel(int nLines, int nGates, string[] gatesToSample, int nExpansions, String[] allowedSubstitutions) {
        System.Random rng = new System.Random();
        List<List<String>> circuit = new List<List<string>>(nLines);

        List<Tuple<int, int>> availableSlots = new List<Tuple<int, int>>();
        for (int i = 0; i < nLines; i++)
        {
            circuit.Add(new List<String>(initialCols*2));
            for (int j = 0; j < initialCols; j++)
            {
                circuit[i].Add(null);
                availableSlots.Add(new Tuple<int, int>(i,j));
            }
        }
        availableSlots.Sort((a,b)=>rng.Next(0,10).CompareTo(5));
        int w = initialCols;
        int h = nLines;
        for (int i = 0; i < nGates; i++)
        {
            String gateSelected = gatesToSample[rng.Next(0, gatesToSample.Length)];
	        int heightOffset = 0;
            if (doubleGates.Contains<String>(gateSelected)) {
                heightOffset = 1;
            }
            else if (tripleGates.Contains<String>(gateSelected)) {
                heightOffset = 2;
            }
            bool valid = false;
			for (int j = availableSlots.Count() - 1; j >= 0; j--)
            {
                var xy = availableSlots[j];
                int y = xy.Item1;
                int x = xy.Item2;
                valid = true;
                for(int k = 0; k <= heightOffset; k++)
                {
                    if (y + k >= nLines || circuit[y + k][x] != null)
                    {
                        valid = false;
                        break;
                    }
                } 
                if (valid)
                {
                    availableSlots.RemoveAt(j);
                    for(int k = 0; k <= heightOffset; k++)
                    {
                        circuit[y + k][x] = gateSelected + "-" + k;
                    }
                    break;
                }
            }
            if (!valid) {
                int y = rng.Next(0, h - heightOffset);
                int x = w;
                for (int j = 0; j < nLines; j++)
                {
                    circuit[j].Add(null);
                }
                for (int k = 0; k <= heightOffset; k++)
                {
                    circuit[y + k][x] = gateSelected + "-" + k;
                }
                w++;

                for(int k = 0; k < nLines; k++) { 
		            if(circuit[k][x] == null) {
                        availableSlots.Add(new Tuple<int, int>(k, x));
		            }
		        }
				availableSlots.Sort((a,b)=>rng.Next(0,10).CompareTo(5));
            }
        }

        //start expansions
        h = circuit.Count;
        w = circuit[0].Count;
        for (int i = 0; i < nExpansions; i++)
        {
            List<Tuple<int, int>> availableGates = new List<Tuple<int, int>>(w * h * 2);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    String currGate = circuit[y][x];
                    if(currGate != null) {
                        int gateOffset = Int32.Parse(circuit[y][x].Split('-')[1]);
                        if(gateOffset == 0) {
                            availableGates.Add(new Tuple<int, int>(y, x));
			            }
		            }
                }
            }
// Debug Level 33 has cause this error once: ArgumentException: Unable to sort because the
// IComparer.Compare() method returns inconsistent results. Either a value does not compare
// equal to itself, or one value repeatedly compared to another value yields different results.
// IComparer: 'System.Comparison`1[System.Tuple`2[System.Int32,System.Int32]]'.
            availableGates.Sort((a, b) => rng.Next(0, 10).CompareTo(5));
            bool success = false;
            for (int j = availableGates.Count - 1; j >= 0; j--)
            {
                var xy = availableGates[j];
                int y = xy.Item1;
                int x = xy.Item2;
                String gate = circuit[y][x].Split('-')[0];
			    if (allowedSubstitutions.Contains<String>(gate))
                {
                    success = true;
                    circuit[y][x] = circuit[y][x].ToLower();
                    switch (gate)
                    {
                        case "X":
                            surroundWithH(x, y, circuit);
                            break;
                        case "Z":
			                surroundWithH(x, y, circuit);

                            break;
                        case "CX":
                            if (allowedSubstitutions.Contains<String>("CX2"))
			                {
                                if (rng.NextDouble() > 0.5)
                                {
                                    x = surroundWithH(x, y, circuit);
                                }
                            }
                            surroundWithH(x, y+1, circuit);
                            break;
                        case "CZ":
                            surroundWithH(x, y + 1, circuit);
                            break;
                        case "SWAP":
                            circuit[y][x] = "CX-0";
                            circuit[y+1][x] = "CX-1";

                            insertGateLeft(x, y, circuit, "CX");
                            insertGateRight(x, y, circuit, "CX");
                            //surroundWithH(x, y + 1, circuit);
                            break;
                        default:
                            success = false;
                            circuit[y][x] = circuit[y][x].ToUpper();
                            break;
                    }
                    if (success) {
                        break;
		            }
                }
            }
            if (!success)
            {
                Debug.Log("No substitutions possible");
                break;
            }
        }
        return circuit;
    }
}

