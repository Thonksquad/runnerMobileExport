using UnityEngine;

public class BossWheelAnimation : MonoBehaviour
{

    private bool _turnLeft = true;

    private void Update()
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

    public void BossDead()
    {
        _turnLeft = false;
    }

    public void BossAlive()
    {
        _turnLeft = true;
    }

}
