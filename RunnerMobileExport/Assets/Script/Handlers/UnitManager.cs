using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    public Collider2D[] DetectedEnemies;
    public float DetectionRadius;

    [SerializeField] private SpawnPool _batPool;
    [SerializeField] private SpawnPool _dasherPool;
    [SerializeField] private SpawnPool _leaperPool;
    [SerializeField] private SpawnPool _archerPool;

    [SerializeField] private SpawnPool _floatingObstaclePool;
    [SerializeField] private SpawnPool _rotatingObstaclePool;
    [SerializeField] private SpawnPool _groundObstaclePool;
    [SerializeField] private SpawnPool _longObstaclePool;
    [SerializeField] private SpawnPool _fireballPool;

    [SerializeField] private SpawnPool _randomCoinPool;
    [SerializeField] private SpawnPool _coinPool;

    [SerializeField] private LayerMask EnemyDetectionLayer; 
    [SerializeField] private GameObject houndPrefab;



    private Player player;  
    [HideInInspector] public Vector2 randomCoinSpawnLocation;
    [HideInInspector] public Vector2 coinSpawnLocation;
    [HideInInspector] public Vector2 obstacleSpawnPoint;
    [HideInInspector] public Vector2 enemySpawnPoint;
    [SerializeField] private float _minSpawnX = 45f;
    [SerializeField] private float _maxSpawnX = 60f;
    [SerializeField] private float _minSpawnY = -6f;
    [SerializeField] private float _maxSpawnY = 6f;


    [SerializeField] private List<BaseEnemy> _units;
    private GameObject enemy;

    private float Respawntimer => 1+(0.01f*CameraManager.Instance.CamSpeed);
    private float mobSpawnDistance => 50f + (0.1f*CameraManager.Instance.CamSpeed);
    [SerializeField] private float mobspawnInterval = 7f;
    [SerializeField] private float coinspawnInterval = 30f;
    [SerializeField] private float mobAutoDestroy = 10f;

    [SerializeField] private float xRef;
    [SerializeField] private float yRef;
    [SerializeField] private float groundHeight = -6.5f;


    private bool IsSafeToSpawn(Vector2 pos, float radius) => Physics2D.OverlapCircleAll(pos, radius, EnemyDetectionLayer).Length == 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        player = Player.Instance;
        //ServiceLocator.ForSceneOf(this).Get(out player); 
    }

    public void SpawnHound()
    {
        xRef = player.transform.position.x + 10;
        yRef = -6.5f;

        if (IsSafeToSpawn(new Vector2(xRef, yRef), 3 * DetectionRadius))
        {
            GameObject newHound = Instantiate(houndPrefab, new Vector3(xRef, yRef, 0), Quaternion.identity);
        } else
        {
            Invoke(nameof(SpawnHound), 0.25f);
        }
    }

    public void SpawnCoin(float xRef, float yRef)
    {
        coinSpawnLocation = new Vector2(xRef, yRef);
        _coinPool.Spawner(coinSpawnLocation);
    }

    public void SpawnRandomCoin()
    {
        randomCoinSpawnLocation = new Vector2(Random.Range(_minSpawnX, _maxSpawnX), Random.Range(-6f, 6f));
        _randomCoinPool.Spawner(randomCoinSpawnLocation);
    }

    public void SpawnObstacle()
    {
        xRef = Random.Range(_minSpawnX, _maxSpawnX);
        yRef = Random.Range(-6f, 6f);

        if (IsSafeToSpawn(new Vector2(xRef, yRef), DetectionRadius * 3))
        {
            obstacleSpawnPoint = new Vector2(xRef, yRef);
            switch (Random.Range(0, 5))
            {
                case 0:
                    _floatingObstaclePool.Spawner(obstacleSpawnPoint);
                    break;
                case 1:

                    xRef = Random.Range(_minSpawnX, _maxSpawnX);
                    yRef = -8f;
                    obstacleSpawnPoint = new Vector2(xRef, yRef);

                    if (IsSafeToSpawn(new Vector2(xRef, yRef), DetectionRadius * 3))
                    {
                        _groundObstaclePool.Spawner(obstacleSpawnPoint);
                    }
                    else
                    {
                        goto case 4;
                    }
                    break;
                case 2:
                    _longObstaclePool.Spawner(obstacleSpawnPoint);
                    break;
                case 3:
                    _rotatingObstaclePool.Spawner(obstacleSpawnPoint);
                    break;
                case 4:
                    _fireballPool.Spawner(obstacleSpawnPoint);
                    break;
                default: break;
            }
        }
        else
        {
            Invoke(nameof(SpawnObstacle), Respawntimer);
        }
    }

    public void SpawnEnemy()
    {
        xRef = Random.Range(_minSpawnX, _maxSpawnX);
        yRef = Random.Range(-6f, 6f);

        if (IsSafeToSpawn(new Vector2(xRef, yRef), DetectionRadius))
        {
            enemySpawnPoint = new Vector2(xRef, yRef);
            switch (Random.Range(0, 5))
            {
                case 0:
                    _batPool.Spawner(enemySpawnPoint);
                    break;
                case 1:
                    enemySpawnPoint = new Vector2(xRef, -7.5f);
                    _dasherPool.Spawner(enemySpawnPoint);
                    break;
                case 2:
                    _leaperPool.Spawner(enemySpawnPoint);
                    break;
                case 3:
                    _archerPool.Spawner(enemySpawnPoint);
                    break;
                default: break;
            }
        }
        else
        {
            Invoke(nameof(SpawnEnemy), Respawntimer);
        }

    }


}
