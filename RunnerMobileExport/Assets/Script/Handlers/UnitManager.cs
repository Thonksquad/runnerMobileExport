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
    [SerializeField] private GameObject RandomCoinPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject houndPrefab;

    [SerializeField] private GameObject batPrefab;
    [SerializeField] private GameObject dasherPrefab;
    [SerializeField] private GameObject leaperPrefab;
    [SerializeField] private GameObject archerPrefab;

    [SerializeField] private GameObject floatingobstaclePrefab;
    [SerializeField] private GameObject rotatingobstaclePrefab;
    [SerializeField] private GameObject groundobstaclePrefab;
    [SerializeField] private GameObject longobstaclePrefab;
    [SerializeField] private GameObject _fireballPrefab;

    private ObjectPool<GameObject> _fireballPool;

    private Player player;
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
        player = FindObjectOfType<Player>();

        _fireballPool = new ObjectPool<GameObject>(() =>
        {
            return Instantiate(_fireballPrefab, new Vector3(xRef, yRef, 0), Quaternion.identity);
        }, fireball =>
        {
            fireball.gameObject.SetActive(false);
        });

        StartCoroutine(SpawnEnemyTimer(mobspawnInterval));
        StartCoroutine(SpawnObstacleTimer(mobspawnInterval));
        StartCoroutine(SpawnCoinTimer(coinspawnInterval));
    }

    public void SpawnHound()
    {
        xRef = player.transform.position.x + mobSpawnDistance;
        yRef = -6.5f;

        if (IsSafeToSpawn(new Vector2(xRef, yRef), 3 * DetectionRadius))
        {
            GameObject newHound = Instantiate(houndPrefab, new Vector3(xRef, yRef, 0), Quaternion.identity);
        } else
        {
            Invoke(nameof(SpawnHound), 0.5f);
        }
    }

    public void SpawnCoin(float xRef, float yRef)
    {
        GameObject newCoin = Instantiate(coinPrefab, new Vector3(xRef, yRef, 0), Quaternion.identity);
        Vector2 forceDirection = new Vector2(Random.Range(0, .001f), Random.Range(.001f, .003f)).normalized;
        newCoin.GetComponent<Rigidbody2D>().AddForce(.1f * forceDirection, ForceMode2D.Impulse);
    }

    public void SpawnRandomCoin()
    {
        GameObject newCoin = Instantiate(RandomCoinPrefab, new Vector3(player.transform.position.x + mobSpawnDistance, Random.Range(-4f, 4f), 0), Quaternion.identity);
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
                    } else
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
                    enemy = _fireballPrefab;
                    GameObject newFireball = Instantiate(enemy, new Vector3(xRef, yRef, 0), Quaternion.identity);
                    Destroy(newFireball, mobAutoDestroy);
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

    private IEnumerator SpawnCoinTimer(float interval)
    {
        yield return new WaitForSeconds(interval);
        SpawnRandomCoin();

        StartCoroutine(SpawnCoinTimer(coinspawnInterval));
    }
}
