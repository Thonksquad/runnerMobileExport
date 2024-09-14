using UnityEngine;

public class BossWheelAnimation : MonoBehaviour
{
    private bool _turn = true;
    private bool _turnLeft = true;

    private void Update()
    {
        if (_turn)
        { 
            if (_turnLeft)
            {
                transform.Rotate(0f, 0f, -1f);
            }
            else
            {
                transform.Rotate(0f, 0f, 1f);
            }
        }
    }

    public void StartPhase()
    {
        _turnLeft = false;
    }

    public void FightPhase()
    {
        _turnLeft = true;
    }

    public void Stop()
    {
        _turn = false;
    }
}
