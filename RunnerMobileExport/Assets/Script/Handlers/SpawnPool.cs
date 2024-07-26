using UnityEngine;
using UnityEngine.Pool;


public class SpawnPool : MonoBehaviour
{

    [SerializeField] private PoolMember _poolMemberPrefab;

    private IObjectPool<PoolMember> _pool;


    private void Awake()
    {
        _pool = new ObjectPool<PoolMember>(CreateEnemy, OnGet, OnRelease);
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
