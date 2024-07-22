using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _speed;
    public int damage = 1;

    public void Init(Vector2 dir)
    {
        float rot = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0,rot);
        _rb.velocity = dir * _speed;
        Destroy(gameObject, 3);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }


}
