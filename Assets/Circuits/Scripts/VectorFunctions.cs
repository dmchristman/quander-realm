using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorFunctions
{
    public static Vector3 Floor(Vector3 v)
    {
        return new Vector3(Mathf.Floor(v.x), Mathf.Floor(v.y), Mathf.Floor(v.z));
    }
}
