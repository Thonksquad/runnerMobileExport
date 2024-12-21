using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BossProjectilePool : MonoBehaviour
{
    [SerializeField] private B1Projectile _bulletPrefab;
    private ObjectPool<B1Projectile> _pool;
    private void Start()
    {
        _pool = new ObjectPool<B1Projectile>(() =>
        {
            var newBullet = Instantiate(_bulletPrefab);
            newBullet.Init(_pool.Release); // give it the reference back home here
            return newBullet;
        }, bullet =>
        {
            bullet.gameObject.SetActive(true);
        }, bullet =>
        {
            bullet.gameObject.SetActive(false);
        }, bullet =>
        {
            Destroy(bullet.gameObject);
        }, false, 30, 40);
    }

    public B1Projectile DoSpawn(Vector3 spawnPos)
    {
        B1Projectile projectile = _pool.Get();
        projectile.transform.position = spawnPos;
        return projectile;
    }
}
