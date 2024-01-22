using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashEnemy: BaseEnemy
{
    [SerializeField] float walkSpeed = 0f;
    [SerializeField] float obstacleRayDistance;
    public GameObject obstacleRayObject;

    Rigidbody2D myRigidbody;

    protected override void Start()
    {
        base.Start();
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isDead)
        {
            Debug.DrawRay(obstacleRayObject.transform.position, transform.TransformDirection(Vector2.left) * 5f, Color.red);
            RaycastHit2D hitObstacle = Physics2D.Raycast(obstacleRayObject.transform.position, Vector2.left, obstacleRayDistance * 2);

            if (hitObstacle)
            {
                _anim.CrossFade("chase", 0, 0);
                walkSpeed = 15f;
            }

            myRigidbody.velocity = new Vector2(-walkSpeed, 0f);
        }
        else
        {
            myRigidbody.bodyType = RigidbodyType2D.Dynamic;
            myRigidbody.velocity = new Vector3(0, -10, 0);
            walkSpeed = 0;
        }

    }
}
