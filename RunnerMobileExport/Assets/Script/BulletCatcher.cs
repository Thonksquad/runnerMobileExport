using UnityEngine;

public class BulletCatcher : MonoBehaviour
{



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Bullet>() != null)
        {
            collision.gameObject.GetComponent<Bullet>().ReturnToPool();
        }
    }
}
