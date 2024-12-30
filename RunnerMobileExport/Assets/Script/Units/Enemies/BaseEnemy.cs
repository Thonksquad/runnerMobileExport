using UnityEngine;
using UnityServiceLocator;

public abstract class BaseEnemy : MonoBehaviour
{
    public EnemyDifficulty EnemyDifficulty;
    public Player player;
    public Animator _anim;
    public bool isDead { get; protected set; } = false;

    [SerializeField] private float _speed;

    private UnitManager _unitManager;


    protected virtual void Start()
    {
        ServiceLocator.ForSceneOf(this).Get(out player);
        ServiceLocator.ForSceneOf(this).Get(out _unitManager);
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
    }

    public virtual void OnEnable()
    {
        ServiceLocator.ForSceneOf(this).Get(out player);
        isDead = false;
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        gameObject.layer = enemyLayer;
        _speed = player.speed;
    }

    public virtual void Update()
    {
        transform.position = new Vector3(transform.position.x - _speed * Time.deltaTime, transform.position.y, transform.position.z);
    }

    public virtual void OnTriggerEnter2D(Collider2D colider)
    {
        int coinChance = Random.Range(1, 101);
        if (colider.gameObject.GetComponent<Bullet>() != null)
        {
            if (player.speed < 20)
            {
                player.speed += .5f;
            } else
            {
                player.speed += .25f;
            }

            colider.gameObject.GetComponent<Bullet>().ReturnToPool();

            if (gameObject.TryGetComponent(out BaseEnemy enemy)){
                enemy.HandleDeath();
            }

            if (coinChance <= 30)
            {
                _unitManager.SpawnCoin(transform.position.x, transform.position.y);
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
