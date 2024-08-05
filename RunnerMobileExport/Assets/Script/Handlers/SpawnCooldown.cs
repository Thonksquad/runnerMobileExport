using UnityEngine;
using Utilities.Cooldown;

public class SpawnCooldown : MonoBehaviour
{
    [SerializeField] private float _SpawnDelay = 0.75f;
    [SerializeField] private float _startValue = 1f;
    [SerializeField] private float _endValue = 3f;
    [SerializeField] private float _modifier = 0.1f;

    private float _currentValue; 
    private Cooldown _cd = new(7f);

    private void OnEnable()
    {
        _cd.Completed += CoolDownFinished;
        _currentValue = _startValue;
        Invoke(nameof(Main), 1f); 
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
                SpawnObstacle();
                Invoke(nameof(SpawnEnemy), _SpawnDelay);
                Invoke(nameof(SpawnRandomCoin), _SpawnDelay*2); 
            }
        }
    }


    private void SpawnObstacle()
    {
        UnitManager.Instance.SpawnObstacle();
    }

    private void SpawnEnemy()
    {
        UnitManager.Instance.SpawnEnemy();
    } 

    private void SpawnRandomCoin()
    {
        UnitManager.Instance.SpawnRandomCoin();
    }

}
