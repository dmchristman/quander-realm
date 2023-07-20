using System.Collections;
using System.Collections.Generic;

public class GateData
{
    public string type;
    public GateData owner;
    public GateData(string t, GateData o)
    {
        type = t;
        owner = o;
    }

    public GateData(string t) : this(t,null) { }


    public override string ToString()
    {
        return type != null ? $"[{type}]" : "-";
    }
}
