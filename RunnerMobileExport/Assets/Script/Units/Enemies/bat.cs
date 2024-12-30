using UnityEngine;
using UnityServiceLocator;

public class bat : BaseEnemy
{
    public ParallaxLayer _parallaxLayer = ParallaxLayer.BlackBackground;
    private ParallaxManager _parallaxManager;
    private ParallaxSetting _parallaxSetting;
    private Player _player;
    public float speed;
    public bool chase = false;
    private Rigidbody2D _rigidbody;

    public override void OnEnable()
    {
        base.OnEnable(); 
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        ServiceLocator.ForSceneOf(this).Get(out _player);
        ServiceLocator.ForSceneOf(this).Get(out _parallaxManager);
        _parallaxSetting = _parallaxManager.parallaxSettings.Find(setting => setting.ParallaxLayer == _parallaxLayer);
    }

    public override void Update()
    {
        transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, transform.position.z);
        if (player == null)
            return;
        if (!isDead)
        {
            Chase();
            Flip();
        } 
        else
        {
            speed = 0;
            _rigidbody.bodyType = RigidbodyType2D.Dynamic;
            _rigidbody.AddForce(Physics.gravity * _rigidbody.mass);
            transform.position = new Vector3(
            transform.position.x - _player.speed * _parallaxSetting.parallaxEffect * Time.deltaTime,
            transform.position.y,
            transform.position.z);
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