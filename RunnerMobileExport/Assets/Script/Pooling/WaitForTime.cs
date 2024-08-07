using UnityEngine;

public class WaitForTime
{
    public float secondsToWait;

    public bool TimerIsWaiting()
    {
        float timeRef = 0;
        if (timeRef == 0)
            timeRef = Time.time;

        if (Time.time < timeRef + secondsToWait)
            return true;
        else
            return false;
    }
}
