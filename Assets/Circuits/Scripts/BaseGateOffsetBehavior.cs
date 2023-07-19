using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGateOffsetBehavior : MonoBehaviour
{
    private bool isHint = false;
    public void mistakeAnimationFinished()
    {
        Debug.Log("TEST");
        if (isHint) 
	    {
            GetComponent<Animation>().Play("GateMotion");
	    }
    }

    public void setHint(bool status)
    {
        isHint = status;
        if(isHint)
        {
            GetComponent<Animation>().Play("GateMotion");
	    }
        else
	    {
            GetComponent<Animation>().Stop();
	    }
    }

    public void mistakeShake() 
    {
        GetComponent<Animation>().Play("Mistake");
    }
}
