using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class LeaperEnemy : BaseEnemy
{
    public float leapSpeed;
    public bool chase = false;
    [SerializeField] ChaseControl chaseControl;
    [SerializeField] private SpriteRenderer myControl;
    private Rigidbody2D rb;

    private int chaseCounter = 0;
    private Transform _target;
    private Vector3 _overshoot;

    private void Awake()
    {
        _target = FindObjectOfType<Player>().transform;
    }


    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            return;
        } else
        {
            if (!isDead)
            {
                rb.velocity = new Vector3(0, Random.Range(-3, 4), 0);
                if (chase == true)
                {
                    if (chaseCounter <2)
                    {
                        overShoot(player.transform.position.x, player.transform.position.y);
                        chaseCounter++;
                    }

                    Chase();
                }

            }
            else
            {
                chaseControl.enabled = false;
                myControl.enabled = false;
                rb.bodyType = RigidbodyType2D.Dynamic;
                Destroy(gameObject, .25f);
            }
        }
    }

    private void OnBecameInvisible()
    {
        chase = false;
    }

    private void overShoot(float playerX, float playerY)
    {
        int ranX;
        int ranY;
        if (transform.position.x > playerX)
        {
            ranX = Random.Range(-5, 0);
        }
        else
        {
            ranX = Random.Range(0, 5);
        }

        if (transform.position.y > playerY)
        {
            ranY = Random.Range(-10, -5);

        }
        else
        {
            ranY = Random.Range(5, 10);
        }

        _overshoot = _target.position + new Vector3(ranX, ranY, 0);

        chaseCounter++;
    }

    private void Chase()
    {
        _anim.CrossFade("attack", 0, 0);
        transform.position = Vector3.MoveTowards(transform.position, _overshoot, leapSpeed * Time.deltaTime);
    }

}


