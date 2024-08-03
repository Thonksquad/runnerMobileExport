using UnityEngine;
using UnityEngine.Pool;


public class PoolMember : MonoBehaviour
{

    public IObjectPool<PoolMember> _pool;

    public virtual void SetPool(IObjectPool<PoolMember> pool)
    {
        _pool = pool;
    }

    public virtual void OnEnable()
    {

    }

    public virtual void ReturnToPool()
    {
        _pool.Release(this);
    }

}

