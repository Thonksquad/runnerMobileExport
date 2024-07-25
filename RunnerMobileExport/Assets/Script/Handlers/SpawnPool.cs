using UnityEngine;
using UnityEngine.Pool;


public class SpawnPool : MonoBehaviour
{

    [SerializeField] private SpawnEnemy _enemyPrefab;

    private IObjectPool<SpawnEnemy> _enemyPool;


    private void Awake()
    {
        _enemyPool = new ObjectPool<SpawnEnemy>(CreateEnemy, OnGet, OnRelease);
    }

    public void Spawner()
    {
        _enemyPool.Get();
    }

    private void OnGet(SpawnEnemy spawnEnemy)
    {
        spawnEnemy.gameObject.SetActive(true);
    }

    private void OnRelease(SpawnEnemy spawnEnemy)
    {
        spawnEnemy.gameObject.SetActive(false);
    }

    private SpawnEnemy CreateEnemy()
    {
        SpawnEnemy spawnEnemy = Instantiate(_enemyPrefab);
        spawnEnemy.SetPool(_enemyPool);
        return spawnEnemy;
    } 

}
