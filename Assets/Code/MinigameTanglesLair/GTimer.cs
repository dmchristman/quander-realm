using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GTimer : MonoBehaviour
{
	public TimerManager timeManager;
    public float timeout = 1.0f;
    public bool repeat = false;
    public bool autoStart = false;
    public EventTrigger.TriggerEvent targets;

    private float currWakeup = -1;
    private int currIndex = -1;
    private bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        if(autoStart)
        { 
			startTimer();
	    }
    }

    public void stopTimer()
    { 
        if(currWakeup > 0)
        {
            active = false;
            timeManager.deregisterTimer(currWakeup, currIndex);
            currWakeup = -1;
            currIndex = -1;
	    }
    }

    public void startTimer()
    {
        stopTimer();    
        active = true;
        var res = timeManager.registerTimer(timeout, callBack);
        currWakeup = res.Item1;
        currIndex = res.Item2;
    }

    public void callBack()
    {
        BaseEventData eventData = new BaseEventData(EventSystem.current);
        eventData.selectedObject = this.gameObject;
        targets.Invoke(eventData);
        if (repeat && active)
        {
            var res = timeManager.registerTimer(timeout, callBack);
            currWakeup = res.Item1;
            currIndex = res.Item2;

	    }
    }

}
