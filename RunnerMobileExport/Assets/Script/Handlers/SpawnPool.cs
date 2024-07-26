using UnityEngine;
using UnityEngine.Pool;


public class SpawnPool : MonoBehaviour
{

    [SerializeField] private PoolMember _poolMemberPrefab;

    [SerializeField] private bool _collectionCheck = false;
    [SerializeField] private int _defaultCapacity = 5;
    [SerializeField] private int _maxSize = 5;

    private IObjectPool<PoolMember> _pool;


    private void Awake()
    {
        _pool = new ObjectPool<PoolMember>(
            CreateEnemy,
            OnGet, 
            OnRelease, 
            poolMember => { Destroy(poolMember.gameObject); },
            _collectionCheck,
            _defaultCapacity,
            _maxSize
            );
    }

    public void Spawner()
    {
        _pool.Get();
    }

    private void OnGet(PoolMember poolMember)
    {
        poolMember.gameObject.SetActive(true);
    }

    private void OnRelease(PoolMember poolMember)
    {
        poolMember.gameObject.SetActive(false);
    }

    private PoolMember CreateEnemy()
    {
        PoolMember poolMember = Instantiate(_poolMemberPrefab);
        poolMember.SetPool(_pool);
        return poolMember;
    } 

}
