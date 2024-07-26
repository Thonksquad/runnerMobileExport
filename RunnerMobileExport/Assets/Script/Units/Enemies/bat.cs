using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bat : BaseEnemy
{
    public float speed;
    public bool chase = false;
    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        if (player == null)
            return;
        if (!isDead)
         {
            Chase();
            Flip();
         } else
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = new Vector3(0, -12, 0);
        }

    }

    private void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    private void Flip()
    {
        if (!isDead)
        {
            if (transform.position.x > player.transform.position.x)
            {
                chase = true;
                speed = 4f * (1 + ((CameraManager.Instance.CamSpeed - 5) / 10));
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                speed = 2f * (1 + ((CameraManager.Instance.CamSpeed - 5) / 10));
            }
        }
    }

}