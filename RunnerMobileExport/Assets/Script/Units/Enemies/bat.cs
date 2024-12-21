using UnityEngine;

public class bat : BaseEnemy
{
    public float speed;
    public bool chase = false;
    private Rigidbody2D _rigidbody;



    public override void OnEnable()
    {
        base.OnEnable(); 
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
    }

    public override void Update()
    {
        base.Update();
        if (player == null)
            return;
        if (!isDead)
        {
            Chase();
            Flip();
        } 
        else
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.AddForce(Physics.gravity * _rigidbody.mass);
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
                speed = 4f * (1 + ((player.speed - 5) / 10));
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                speed = 2f * (1 + ((player.speed - 5) / 10));
            }
        }
    }

}