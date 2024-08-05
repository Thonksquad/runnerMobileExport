using UnityEngine;
using Utilities.Cooldown;

public class SpawnCooldown : MonoBehaviour
{
    [SerializeField] private float _SpawnDelay = 0.75f;
    [SerializeField] private float _startValue = 1f;
    [SerializeField] private float _endValue = 3f;
    [SerializeField] private float _modifier = 0.1f;

    private float _currentValue;
    private int _roundedValue;
    private Cooldown _cdMain = new(7f);
    private Cooldown _cdObstacle = new(7f);
    private Cooldown _cdEnemy = new(7f);
    private Cooldown _cdCoin = new(7f);

    private void OnEnable()
    {
        _cdMain.Completed += MainCooldown;
        _cdObstacle.Completed += ObstacleCooldown;
        _cdEnemy.Completed += EnemyCooldown;
        _cdCoin.Completed += CoinCooldown;
        _currentValue = _startValue;
        Invoke(nameof(MainCooldown), 0.75f);
        Invoke(nameof(ObstacleCooldown), 1f);
        Invoke(nameof(EnemyCooldown), 1f + _SpawnDelay);
        Invoke(nameof(CoinCooldown), 1f + _SpawnDelay * 2);
    }


    private void OnDisable()
    {
        _cdMain.Completed -= MainCooldown;
        _cdObstacle.Completed -= ObstacleCooldown;
        _cdEnemy.Completed -= EnemyCooldown;
        _cdCoin.Completed -= CoinCooldown;
    }

    private void MainCooldown()
    {
        AdjustTimer();
        _cdMain.Start();
    }

    private void AdjustTimer()
    {
        if (_currentValue < _endValue)
        {
            _currentValue += _modifier;
        }
        _roundedValue = Mathf.FloorToInt(_currentValue);
    }

    private void ObstacleCooldown()
    {
        if (UnitManager.Instance != null)
        {
            for (int i = 0; i < _roundedValue; i++)
            {
                UnitManager.Instance.SpawnObstacle();
            }
        }
        _cdObstacle.Start();
    }

    private void EnemyCooldown()
    {
        if (UnitManager.Instance != null)
        {
            for (int i = 0; i < _roundedValue; i++)
            {
                UnitManager.Instance.SpawnEnemy();
            }
        }
        _cdEnemy.Start();
    }

    private void CoinCooldown()
    {
        if (UnitManager.Instance != null)
        {
            for (int i = 0; i < _roundedValue; i++)
            {
                UnitManager.Instance.SpawnRandomCoin();
            }
        }
        _cdCoin.Start();
    } 

}
