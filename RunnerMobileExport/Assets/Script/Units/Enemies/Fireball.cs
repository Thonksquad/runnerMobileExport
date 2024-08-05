using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] float walkSpeed = 12f;
    [SerializeField] private AudioClip fireballSound;
    Rigidbody2D myRigidbody;
    public int damage = 1;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnBecameVisible()
    {
        SoundManager.Instance.PlaySound(fireballSound);
    }

    private void Update()
    {
        myRigidbody.velocity = new Vector2(-walkSpeed, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {
            player.TakeDamage(damage);
            //Destroy(gameObject);
        }
    }

}
