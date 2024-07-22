using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    public Collider2D[] DetectedEnemies;
    public float DetectionRadius;

    [SerializeField] private LayerMask EnemyDetectionLayer;
    [SerializeField] private Coin StaticCoinPrefab;
    [SerializeField] private Coin coinPrefab;
    [SerializeField] private GameObject houndPrefab;

    [SerializeField] private GameObject batPrefab;
    [SerializeField] private GameObject dasherPrefab;
    [SerializeField] private GameObject leaperPrefab;
    [SerializeField] private GameObject archerPrefab;

    [SerializeField] private GameObject floatingobstaclePrefab;
    [SerializeField] private GameObject rotatingobstaclePrefab;
    [SerializeField] private GameObject groundobstaclePrefab;
    [SerializeField] private GameObject longobstaclePrefab;
    [SerializeField] private Fireball _fireballPrefab;

    [SerializeField] private bool _usePool;
    private ObjectPool<Fireball> _fireballPool;
    private ObjectPool<Coin> _coinPool;
    private ObjectPool<Coin> _staticCoinPool;

    private Player player;
    [SerializeField] private List<BaseEnemy> _units;
    private GameObject enemy;

    private float Respawntimer => 1 + (0.01f * CameraManager.Instance.CamSpeed);
    private float mobSpawnDistance => 50f + (0.1f * CameraManager.Instance.CamSpeed);
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

    private void OnEnable()
    {
        ActionSystem.onAdRevive += PauseSpawns;
    }

    private void OnDisable()
    {
        ActionSystem.onAdRevive -= PauseSpawns;
    }


    private void PauseSpawns()
    {
        StopAllCoroutines();
        Invoke(nameof(ResumeSpawns), 2f);
    }

    private void ResumeSpawns()
    {
        StartCoroutine(SpawnEnemyTimer(mobspawnInterval));
        StartCoroutine(SpawnObstacleTimer(mobspawnInterval));
        StartCoroutine(SpawnCoinTimer(coinspawnInterval));
    }

    private void Start()
    {
        //Replace the old coroutines with the timer system with each general type (enemies/obstacle)
        //Create 1 coroutine that creates the timers on loop
        //Each timer will be updated by TimerUpdater

        //Convert this into a coroutine
        /**
        Timer timer = new Timer(3f, () =>
        {
            Debug.Log("Timer has ticked to 0! " + Time.time);
        });
        **/

        player = FindObjectOfType<Player>();

        CreateFireBallPool();
        CreateStaticCoinPool();
        CreateCoinPool();


        StartCoroutine(SpawnEnemyTimer(mobspawnInterval));
        StartCoroutine(SpawnObstacleTimer(mobspawnInterval));
        StartCoroutine(SpawnCoinTimer(coinspawnInterval));
    }

    private void CreateFireBallPool()
    {
        _fireballPool = new ObjectPool<Fireball>(() =>
        {
            //Return from pool
            return Instantiate(_fireballPrefab, new Vector3(xRef, yRef, 0), Quaternion.identity);
        }, fireball =>
        {
            //Grab from pool
            fireball.gameObject.SetActive(true);
        }, fireball =>
        {
            //On release
            fireball.gameObject.SetActive(false);
        }, fireball =>
        {
            Destroy(fireball.gameObject);
        },
        // Check this to false to save CPU cycles (this checks if it's in the pool or not)
        false,
        //Current array size
        3,
        //Max array size
        10);
    }

    //Fix this
    private void DespawnFireball(Fireball fireball)
    {
        Debug.Log("Attempting to despawn fireball");
        if (_usePool) _fireballPool.Release(fireball);
        else Destroy(fireball, mobAutoDestroy);
    }

    private void CreateStaticCoinPool()
    {
        _staticCoinPool = new ObjectPool<Coin>(() =>
        {
            //Return from pool
            return Instantiate(StaticCoinPrefab);
        }, staticCoin =>
        {
            //Grab from pool
            staticCoin.gameObject.SetActive(true);
        }, staticCoin => {
            //On release
            staticCoin.gameObject.SetActive(false);
        }, staticCoin =>
        {
            Destroy(staticCoin.gameObject);
        },
        // Check this to false to save CPU cycles (this checks if it's in the pool or not)
        false,
        //Current array size
        3,
        //Max array size
        10);
    }

    private void CreateCoinPool()
    {
        _coinPool = new ObjectPool<Coin>(() =>
        {
            //Return from pool
            return Instantiate(coinPrefab);
        }, dynamicCoin =>
        {
            //Grab from pool
            dynamicCoin.gameObject.SetActive(true);
        }, dynamicCoin => {
            //On release
            dynamicCoin.gameObject.SetActive(false);
        }, dynamicCoin =>
        {
            Destroy(dynamicCoin.gameObject);
        },
        // Check this to false to save CPU cycles (this checks if it's in the pool or not)
        false,
        //Current array size
        3,
        //Max array size
        10);
    }

    public void SpawnHound()
    {
        xRef = player.transform.position.x + mobSpawnDistance;
        yRef = -6.5f;

        if (IsSafeToSpawn(new Vector2(xRef, yRef), 3 * DetectionRadius))
        {
            GameObject newHound = Instantiate(houndPrefab, new Vector3(xRef, yRef, 0), Quaternion.identity);
        }
        else
        {
            Invoke(nameof(SpawnHound), 0.5f);
        }
    }

    public void SpawnCoin(float xRef, float yRef)
    {
        Coin newCoin = Instantiate(coinPrefab, new Vector3(xRef, yRef, 0), Quaternion.identity);
        Vector2 forceDirection = new Vector2(Random.Range(0, .001f), Random.Range(.001f, .003f)).normalized;
        newCoin.GetComponent<Rigidbody2D>().AddForce(.1f * forceDirection, ForceMode2D.Impulse);
    }

    public void SpawnRandomCoin()
    {
        Coin newCoin = Instantiate(StaticCoinPrefab, new Vector3(player.transform.position.x + mobSpawnDistance, Random.Range(-4f, 4f), 0), Quaternion.identity);
    }

    private void SpawnObstacle()
    {
        xRef = player.transform.position.x + mobSpawnDistance;
        yRef = Random.Range(-6f, 6f);

        if (IsSafeToSpawn(new Vector2(xRef, yRef), DetectionRadius * 3))
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                    enemy = floatingobstaclePrefab;
                    GameObject newFloating = Instantiate(enemy, new Vector3(xRef, yRef, 0), Quaternion.identity);
                    newFloating.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 180));
                    Destroy(newFloating, mobAutoDestroy);
                    break;
                case 1:
                    xRef = player.transform.position.x + mobSpawnDistance;
                    yRef = -8f;
                    if (IsSafeToSpawn(new Vector2(xRef, yRef), DetectionRadius * 3))
                    {
                        enemy = groundobstaclePrefab;
                        GameObject newGround = Instantiate(enemy, new Vector3(xRef, yRef, 0), Quaternion.identity);
                        Destroy(newGround, mobAutoDestroy);
                    }
                    else
                    {
                        goto case 4;
                    }
                    break;
                case 2:
                    enemy = longobstaclePrefab;
                    GameObject newLong = Instantiate(enemy, new Vector3(xRef, yRef, 0), Quaternion.identity);
                    newLong.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 180));
                    Destroy(newLong, mobAutoDestroy);
                    break;
                case 3:
                    enemy = rotatingobstaclePrefab;
                    GameObject newRotate = Instantiate(enemy, new Vector3(xRef, yRef, 0), Quaternion.identity);
                    Destroy(newRotate, mobAutoDestroy);
                    break;
                case 4:
                    var newFireball = _usePool ? _fireballPool.Get() : Instantiate(_fireballPrefab, new Vector3(xRef, yRef, 0), Quaternion.identity);
                    newFireball.Init(DespawnFireball);
                    break;
                default: break;
            }
        }
        else
        {
            Invoke(nameof(SpawnObstacle), Respawntimer);
        }
    }

    private void SpawnEnemy()
    {
        xRef = player.transform.position.x + mobSpawnDistance;
        yRef = Random.Range(-6f, 6f);

        if (IsSafeToSpawn(new Vector2(xRef, yRef), DetectionRadius))
        {
            switch (Random.Range(0, 5))
            {
                case 0:
                    enemy = batPrefab;
                    GameObject newBat = Instantiate(enemy, new Vector3(xRef, yRef, 0), Quaternion.identity);
                    Destroy(newBat, mobAutoDestroy);
                    break;
                case 1:
                    enemy = dasherPrefab;
                    GameObject newDasher = Instantiate(enemy, new Vector3(xRef, -7.5f, 0), Quaternion.identity);
                    Destroy(newDasher, mobAutoDestroy);
                    break;
                case 2:
                    enemy = leaperPrefab;
                    GameObject newLeaper = Instantiate(enemy, new Vector3(xRef, yRef, 0), Quaternion.identity);
                    Destroy(newLeaper, mobAutoDestroy);
                    break;
                case 3:
                    enemy = archerPrefab;
                    GameObject newArcher = Instantiate(enemy, new Vector3(xRef, yRef, 0), Quaternion.identity);
                    Destroy(newArcher, mobAutoDestroy);
                    break;
                default: break;
            }
        }
        else
        {
            Invoke(nameof(SpawnEnemy), Respawntimer);
        }
    }

    private IEnumerator SpawnEnemyTimer(float interval)
    {
        yield return new WaitForSeconds(interval);
        SpawnEnemy();

        if (mobspawnInterval > 1f)
        {
            mobspawnInterval -= .05f;
        }

        StartCoroutine(SpawnEnemyTimer(mobspawnInterval));
    }

    private IEnumerator SpawnObstacleTimer(float interval)
    {
        yield return new WaitForSeconds(interval);
        SpawnObstacle();

        StartCoroutine(SpawnObstacleTimer(mobspawnInterval));
    }

    // Convert this to using timer
    private IEnumerator SpawnCoinTimer(float interval)
    {
        yield return new WaitForSeconds(interval);
        SpawnRandomCoin();

        StartCoroutine(SpawnCoinTimer(coinspawnInterval));
    }
}
