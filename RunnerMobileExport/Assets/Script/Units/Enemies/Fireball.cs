using UnityEngine;
using UnityServiceLocator;

public class Fireball : MonoBehaviour
{
    [SerializeField] float walkSpeed = 200f; // Standardize this to others
    [SerializeField] private AudioClip fireballSound;
    Rigidbody2D myRigidbody;
    public int damage = 1;

    private SoundManager _soundManager;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        ServiceLocator.ForSceneOf(this).Get(out _soundManager);
    }

    private void OnBecameVisible()
    {
        _soundManager.PlaySound(fireballSound);
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
        }
    }

}
