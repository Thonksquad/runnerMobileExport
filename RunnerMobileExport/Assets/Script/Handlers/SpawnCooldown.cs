using UnityEngine;
using Utilities.Cooldown;

public class SpawnCooldown : MonoBehaviour
{
    [SerializeField] private float _startValue = 1f;
    [SerializeField] private float _endValue = 3f;
    [SerializeField] private float _modifier = 0.1f;

    private float _currentValue; 
    private Cooldown _cd = new(7f);

    private void OnEnable()
    {
        _cd.Completed += CoolDownFinished;
        _currentValue = _startValue;
        Main();
    }


    private void OnDisable()
    {
        _cd.Completed -= CoolDownFinished;
    }


    private void CoolDownFinished()
    {
        Main();
    }

    private void Main()
    {
        _cd.Start();

        int roundedValue = Mathf.FloorToInt(_currentValue);
        SpawnObject(roundedValue);

        if (_currentValue < _endValue)
        {
            _currentValue += _modifier;
        }

    }

    private void SpawnObject(int x)
    {
        if (UnitManager.Instance != null)
        {
            for (int i = 0; i < x; i++)
            {
                UnitManager.Instance.SpawnRandomCoin();
                UnitManager.Instance.SpawnObstacle();
                UnitManager.Instance.SpawnEnemy();
            }
        }
    }

}
