using UnityEngine;


public class EnemyDamage : MonoBehaviour
{
    public int damage = 1;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            player.TakeDamage(damage);
        }
    }
}
