using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public delegate void CallBackFun();
    SortedList<float, List<CallBackFun>> callbacks = new SortedList<float, List<CallBackFun>>();
    public (float, int) registerTimer(float duration, CallBackFun f)
    {
        float wakeupTime = duration + Time.time;
        if(!callbacks.ContainsKey(wakeupTime))
        {
            callbacks.Add(wakeupTime, new List<CallBackFun>());
	    }
        List<CallBackFun> l = callbacks[wakeupTime];
        l.Add(f);
        return (wakeupTime, l.Count - 1);
    }

    public void deregisterTimer(float wakeupTime, int index)
    { 
        if(callbacks.ContainsKey(wakeupTime))
        {
            callbacks[wakeupTime][index] = ()=>{};
	    }
    }
    private List<CallBackFun> popFirst() {
        if(callbacks.Count == 0)
        {
            return null;
	    }
	
		var output = callbacks.Values[0];
        callbacks.RemoveAt(0);
        return output;
	     
    }

    // Update is called once per frame
    void Update()
    {
        float currTime = Time.time;
        while(callbacks.Count >0 && callbacks.Keys[0] < currTime)
        {
            List<CallBackFun> l = popFirst();
            foreach (CallBackFun callback in l)
            {
                callback();
            }
	    }
    }
}
