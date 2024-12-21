using UnityEngine;
using UnityServiceLocator;

public class EnemyProjectile : MonoBehaviour
{

    [SerializeField] private float _speed = 3f;
    public int damage = 1;

    private Player _player;
    private float _playerSpeed;



    public void Init(Vector2 dir)
    {
        ServiceLocator.ForSceneOf(this).Get(out _player);
        _playerSpeed = _player.speed;
        float rot = Mathf.Atan2(-dir.y, -dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0,rot);
        Invoke(nameof(Deactivation), 3);
    }

    public void Update()
    {
        transform.Translate( -transform.right * _playerSpeed * _speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            player.TakeDamage(damage);
            Deactivation();
        }
    }

    private void Deactivation()
    {
        if(gameObject.activeInHierarchy)
        { 
            gameObject.GetComponent<PoolMember>().ReturnToPool();
        }
    }


}
