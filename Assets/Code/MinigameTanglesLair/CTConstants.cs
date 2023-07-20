using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTConstants
{
    public const float camera_scale = 10.75f;
    public const float gridResolution_w = .45f * camera_scale;
    public const float gridResolution_h = .37f * camera_scale;
#if LITE_VERSION
    public const int N_LEVELS = 16;
#else
    public const int N_LEVELS = 25;
#endif
}

public struct Gates
{
    //public static GateData H = new GateData("H");
    //public static GateData CNOT = new GateData("CNOT", 2);
    //public static GateData CNOTL = new GateData("CNOTL", 3);
    //public static GateData CZ = new GateData("CZ", 2);
    //public static GateData I = new GateData("I");
    //public static GateData Z = new GateData("Z");
    //public static GateData NOT = new GateData("NOT");
    //public static GateData SWAP = new GateData("SWAP", 2);

    //public static GateData[] VALID_GATES = {H, CNOT, CNOTL, CZ, Z, NOT, SWAP};

    public static GameObject loadGate(string g){
		return Resources.Load<GameObject>($"Prefabs/{g}_Gate");
	}

	
}
