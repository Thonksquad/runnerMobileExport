using UnityEngine;
using UnityEngine.Pool;


public class PoolMember : MonoBehaviour
{

    [SerializeField] private float _enemyDestroyTimer = 10f;
    [SerializeField] private float _minSpawnX = 45f;
    [SerializeField] private float _maxSpawnX = 55f;
    [SerializeField] private float _minSpawnY = -6f;
    [SerializeField] private float _maxSpawnY = 6f;

    public IObjectPool<PoolMember> _pool;

    public virtual void SetPool(IObjectPool<PoolMember> pool)
    {
        _pool = pool;
    }

    public virtual void OnEnable()
    {
        //Invoke(nameof(ReturnToPool), _enemyDestroyTimer);
        transform.position = new Vector3(Random.Range(_minSpawnX, _maxSpawnX), Random.Range(_minSpawnY, _maxSpawnY), 0f);
    }

    public virtual void ReturnToPool()
    {
        _pool.Release(this);
    }

}

