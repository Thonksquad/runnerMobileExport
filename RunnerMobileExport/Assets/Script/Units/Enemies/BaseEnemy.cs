using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    public EnemyDifficulty EnemyDifficulty;
    public Player player;
    public Animator _anim;
    public bool isDead { get; protected set; } = false;

    protected virtual void Start()
    {
        player = FindObjectOfType<Player>();
        _anim = GetComponent<Animator>();
        _anim.CrossFade("alive", 0, 0);
    }

    public virtual void HandleDeath()
    {
        ActionSystem.onEnemyDeath(this);
        isDead = true;
        _anim.CrossFade("dead", 0, 0);
        int deadLayer = LayerMask.NameToLayer("deadEnemy");
        gameObject.layer = deadLayer;
        //moved to individual classes
        //rb.bodyType = RigidbodyType2D.Dynamic;
        //rb.velocity = new Vector3(0, -10, 0);
    }

    public virtual void OnTriggerEnter2D(Collider2D colider)
    {
        int coinChance = Random.Range(1, 101);
        if (colider.gameObject.GetComponent<Bullet>() != null)
        {
            if (CameraManager.Instance.CamSpeed < 20)
            {
                CameraManager.Instance.CamSpeed += .5f;
            } else
            {
                CameraManager.Instance.CamSpeed += .25f;
            }
            Destroy(colider.gameObject);

            if (gameObject.TryGetComponent(out BaseEnemy enemy)){
                enemy.HandleDeath();
            }

            //Destroy(this.gameObject,1f);

            if (coinChance <= 30)
            {
                UnitManager.Instance.SpawnCoin(transform.position.x, transform.position.y);
            }
        }
    }
}

public enum EnemyDifficulty
{
    mob = 0,
    elite = 1,
    boss = 2
}
