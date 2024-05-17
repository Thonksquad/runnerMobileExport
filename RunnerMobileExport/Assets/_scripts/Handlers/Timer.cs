
using System.Collections.Generic;
using System;
using UnityEngine;

public class Timer
{
    // TO DO: DESTROY ALL TIMERS WHEN YOU HIT PLAY
    public static List<Timer> ActiveTimers = new();
    private float _resetDuration;
    private float _remaining;
    private Action _onTimerReachesZero;
    private bool _repeating;

    public Timer(float duration, Action onEnd)
    {
        _resetDuration = duration;
        _remaining = duration;
        _onTimerReachesZero = onEnd;
        ActiveTimers.Add(this);
    }

    public static void UpdateTimers(float deltaTime)
    {
        for (int i = ActiveTimers.Count - 1; i >= 0; i--) // THIS FOR LOOP IS DANGEROUS AS IT CONTINUES EVEN WHEN IT GOES TO PLAY
        {
            Timer timer = ActiveTimers[i];

            timer._remaining -= deltaTime;

            if (timer._remaining <= 0f)
            {
                timer._onTimerReachesZero?.Invoke();

                if (GameManager.GameState == GameState.ArcadeMode && timer._repeating)
                {
                    timer.HandleReset();
                }
                else
                {
                    ActiveTimers.Remove(timer);
                }
            }
        }
    }

    private void HandleReset()
    {
        if (_resetDuration > 2f)
        {
            _resetDuration -= .075f;
        }
        _remaining = _resetDuration;

        Debug.Log("Reset duration is now " + _resetDuration);
    }

    public static void CreateNewTimer(float duration, bool repeat, Action onEnd)
    {
        Timer newTimer = new Timer(duration, onEnd);
    }
}