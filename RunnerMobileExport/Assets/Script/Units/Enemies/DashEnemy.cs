using UnityEngine;

public class DashEnemy: BaseEnemy
{
    [SerializeField] float walkSpeed = 0f;
    [SerializeField] float obstacleRayDistance;
    public GameObject obstacleRayObject;

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
        if (!isDead)
        {
            Debug.DrawRay(obstacleRayObject.transform.position, transform.TransformDirection(Vector2.left) * 5f, Color.red);
            RaycastHit2D hitObstacle = Physics2D.Raycast(obstacleRayObject.transform.position, Vector2.left, obstacleRayDistance * 2);

            if (hitObstacle)
            {
                _anim.CrossFade("chase", 0, 0);
                walkSpeed = 15f;
            }

            _rigidbody.velocity = new Vector2(-walkSpeed, 0f);
        }
        else
        {
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            //myRigidbody.velocity = new Vector3(0, -10, 0);
            walkSpeed = 0;
        }

    }
}
