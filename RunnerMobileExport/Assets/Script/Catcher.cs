using UnityEngine;

public class Catcher : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PoolMember>() != null)
        {
            collision.gameObject.GetComponent<PoolMember>().ReturnToPool();
        }
    }

}
