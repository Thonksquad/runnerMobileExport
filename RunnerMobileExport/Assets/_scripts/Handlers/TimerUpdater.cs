using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerUpdater : MonoBehaviour
{
    private void LateUpdate()
    {
        if (GameManager.GameState != GameState.ArcadeMode) return; // Make sure this is working
        Timer.UpdateTimers(Time.deltaTime);
    }

    private static TimerUpdater _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
