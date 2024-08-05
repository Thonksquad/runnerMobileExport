using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityServiceLocator;

public class RandomCoin : PoolMember
{
    [SerializeField] private float _speed = 0.04f;
    [SerializeField] private AudioClip _clip;
    private Player player;
    private float step = .2f;
    public Collider2D[] DetectPlayer;
    public float DetectionRadius;
    [SerializeField] private LayerMask PlayerLayer;
    [SerializeField] private Collider2D PhysicsCollider;
    private bool IsChaseOn(Vector2 pos, float radius) => Physics2D.OverlapCircleAll(pos, radius, PlayerLayer).Length > 0;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            SoundManager.Instance.PlaySound(_clip);
            GameManager.Instance.IncreaseCoin(1);
            //Destroy(gameObject);
            ReturnToPool();
        }
    }

    private void Update()
    {
        if (IsChaseOn(new Vector2(transform.position.x, transform.position.y), DetectionRadius))
        {
            PhysicsCollider.enabled = false;
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
            step += 5 * Time.deltaTime;
        }
        else
        {
            transform.position = new Vector3(transform.position.x - _speed, transform.position.y, transform.position.z);
        }
    }

    public override void OnEnable()
    {
        ServiceLocator.ForSceneOf(this).Get(out player);
        base.OnEnable();
    }

}