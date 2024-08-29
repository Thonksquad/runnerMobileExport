using UnityEngine;
using UnityServiceLocator;

public class Coin : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;
    [SerializeField] private AudioClip _clip;
    private float step = .2f;
    public Collider2D[] DetectPlayer;
    public float DetectionRadius;
    [SerializeField] private LayerMask PlayerLayer;
    [SerializeField] private Collider2D PhysicsCollider;

    
    private Player _player;
    private GameManager _gameManager;
    private SoundManager _soundManager;


    private bool IsChaseOn(Vector2 pos, float radius) => Physics2D.OverlapCircleAll(pos, radius, PlayerLayer).Length > 0;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            _soundManager.PlaySound(_clip);
            _gameManager.IncreaseCoin(1); 
            gameObject.GetComponent<PoolMember>().ReturnToPool();
        }
    }

    private void Update()
    {
        if (IsChaseOn(new Vector2(transform.position.x, transform.position.y), DetectionRadius))
        {
            PhysicsCollider.enabled = false;
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, step);
            step += 5 * Time.deltaTime;
        }
        else
        {
            transform.position = new Vector3(transform.position.x - _speed * Time.deltaTime, transform.position.y, transform.position.z);
        }
    }

    public void OnEnable()
    {
        ServiceLocator.ForSceneOf(this).Get(out _player);
        ServiceLocator.ForSceneOf(this).Get(out _gameManager);
        ServiceLocator.ForSceneOf(this).Get(out _soundManager);
        Vector2 forceDirection = new Vector2(Random.Range(0, .001f), Random.Range(.001f, .003f)).normalized;
        gameObject.GetComponent<Rigidbody2D>().AddForce(.1f * forceDirection, ForceMode2D.Impulse);
        _speed = _player.speed;
    }

}
