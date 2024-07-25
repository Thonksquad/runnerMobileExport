using UnityEngine;
using UnityEngine.Pool;


public class SpawnEnemy : MonoBehaviour
{

    [SerializeField] private float _enemyDestroyTimer = 10f;
    [SerializeField] private float _minSpawnX = 45f;
    [SerializeField] private float _maxSpawnX = 55f;
    [SerializeField] private float _minSpawnY = -6f;
    [SerializeField] private float _maxSpawnY = 6f;

    private IObjectPool<SpawnEnemy> _enemyPool;

    public void SetPool(IObjectPool<SpawnEnemy> pool)
    {
        _enemyPool = pool;
    }

    private void OnEnable()
    {
        Invoke(nameof(ReturnToPool), _enemyDestroyTimer);
        transform.position = new Vector3( Random.Range(_minSpawnX, _maxSpawnX), Random.Range(_minSpawnY, _maxSpawnY), 0f);
    }

    public void ReturnToPool()
    {
        _enemyPool.Release(this);
    }
}
