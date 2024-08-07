using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    public Collider2D[] DetectedEnemies;
    public float DetectionRadius;

    private bool isSpawning;

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
    [SerializeField] private GameObject fireballPrefab;

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
    [SerializeField] private AnimationCurve respawnTimer;

    private bool IsSafeToSpawn(Vector2 pos, float radius) => Physics2D.OverlapCircleAll(pos, radius, EnemyDetectionLayer).Length == 0;
    private ObjectPool coinPool = new ObjectPool();
    private ObjectPool houndPool = new ObjectPool();
    private ObjectPool batPool = new ObjectPool();
    private ObjectPool dasherPool = new ObjectPool();
    private ObjectPool leaperPool = new ObjectPool();
    private ObjectPool archerPool = new ObjectPool();
    private ObjectPool floatingObsPool = new ObjectPool();
    private ObjectPool rotatingObsPool = new ObjectPool();
    private ObjectPool groundObsPool = new ObjectPool();
    private ObjectPool longObsPool = new ObjectPool();
    private ObjectPool fireballPool = new ObjectPool();

    private List<SpawnEntry> spawnQueue = new List<SpawnEntry>();
    private List<GameObject> currentActiveEnemies = new List<GameObject>();
    private List<Coroutine> disableEnemyList = new List<Coroutine>();
    private struct SpawnEntry
    {
        internal System.Func<GameObject> functionName;
        internal Quaternion newRotation;
        internal bool willChangeRotation;
    };

    private void Awake()
    {
        coinPool.CreateObjectPool(coinPrefab,3);
        houndPool.CreateObjectPool(houndPrefab, 2);
        dasherPool.CreateObjectPool(dasherPrefab, 2);
        batPool.CreateObjectPool(batPrefab, 2);
        leaperPool.CreateObjectPool(leaperPrefab, 2);
        archerPool.CreateObjectPool(archerPrefab, 1);
        floatingObsPool.CreateObjectPool(floatingobstaclePrefab, 2);
        rotatingObsPool.CreateObjectPool(rotatingobstaclePrefab, 2);
        groundObsPool.CreateObjectPool(groundobstaclePrefab, 2);
        longObsPool.CreateObjectPool(longobstaclePrefab, 2);
        fireballPool.CreateObjectPool(fireballPrefab, 2);

        Instance = this;
        ActionSystem.onPlayerHoundPickup += DisableAllEnemies;
    }

    private void Start()
    {
        isSpawning = false;
        player = FindObjectOfType<Player>();
        StartCoroutine(SpawnEnemyTimer(mobspawnInterval));
        StartCoroutine(SpawnObstacleTimer(mobspawnInterval));
        StartCoroutine(SpawnCoinTimer(coinspawnInterval));

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            ActionSystem.onPlayerHoundPickup();
    }

    public void SpawnHound()
    {
        xRef = player.transform.position.x + mobSpawnDistance;
        yRef = -6.5f;

        if (IsSafeToSpawn(new Vector2(xRef, yRef), 3 * DetectionRadius))
        {
            houndPool.DoSpawn(new Vector3(xRef, yRef, 0));
        } else
        {
            Invoke(nameof(SpawnHound), 0.5f);
        }
    }

    public void SpawnCoin(float xRef, float yRef)
    {
        GameObject newCoin = coinPool.DoSpawn(new Vector3(xRef, yRef, 0));
        Vector2 forceDirection = new Vector2(Random.Range(0, .001f), Random.Range(.001f, .003f)).normalized;
        newCoin.GetComponent<Rigidbody2D>().AddForce(.1f * forceDirection, ForceMode2D.Impulse);
    }

    public void SpawnRandomCoin()
    {
        coinPool.DoSpawn(new Vector3(player.transform.position.x + mobSpawnDistance, Random.Range(-4f, 4f), 0));
    }

    private void SpawnObstacle()
    {
        SpawnEntry spawnEntryObsTemp = new SpawnEntry();
        spawnEntryObsTemp.willChangeRotation = true;
        xRef = player.transform.position.x + mobSpawnDistance;
        yRef = Random.Range(-6f, 6f);

        switch (Random.Range(0, 4))
        {
            case 0:     // Floating Obstacle
                spawnEntryObsTemp.functionName = () => floatingObsPool.DoSpawn(new Vector3(xRef, yRef, 0));
                spawnEntryObsTemp.newRotation = Quaternion.Euler(0, 0, Random.Range(0, 180));
                break;
            case 1:     // ground Obstacle
                if (IsSafeToSpawn(new Vector2(xRef, yRef), DetectionRadius /* 3*/))
                {
                    xRef = player.transform.position.x + mobSpawnDistance;
                    yRef = -8f;
                    enemy = groundobstaclePrefab;
                    spawnEntryObsTemp.functionName = () => groundObsPool.DoSpawn(new Vector3(xRef, -8, 0));
                    spawnEntryObsTemp.willChangeRotation = false;
                }
                else
                {
                    goto case 4;
                }
                break;
            case 2:
                spawnEntryObsTemp.functionName = () => longObsPool.DoSpawn(new Vector3(xRef, yRef, 0));
                spawnEntryObsTemp.newRotation = Quaternion.Euler(0, 0, Random.Range(0, 180));
                break;
            case 3:
                spawnEntryObsTemp.functionName = () => rotatingObsPool.DoSpawn(new Vector3(xRef, yRef, 0));
                spawnEntryObsTemp.willChangeRotation = false;
                break;
            case 4:
                spawnEntryObsTemp.functionName = () => fireballPool.DoSpawn(new Vector3(xRef, yRef, 0));
                spawnEntryObsTemp.willChangeRotation = false;
                break;
            default: break;
        }

        spawnQueue.Add(spawnEntryObsTemp);

        if (!isSpawning)    //check if spawn coroutine is playing
        {
            isSpawning = true;
            StartCoroutine(ProcessSpawnQueue(3));    // this coroutine will handle queue spawning to avoid multi spawns at the same time
        }
    }
    private void SpawnEnemy()
    {
        SpawnEntry spawnEntryTemp = new SpawnEntry();
        spawnEntryTemp.willChangeRotation = false;
        xRef = player.transform.position.x + mobSpawnDistance;
        yRef = Random.Range(-6f, 6f);

        switch (Random.Range(0, 3))
        {
            case 0:
                spawnEntryTemp.functionName = () => batPool.DoSpawn(new Vector3(xRef, yRef, 0));
                break;
            case 1:
                spawnEntryTemp.functionName = () => dasherPool.DoSpawn(new Vector3(xRef, -7.5f, 0));
                break;
            case 2:
                spawnEntryTemp.functionName = () => leaperPool.DoSpawn(new Vector3(xRef, yRef, 0));
                break;
            case 3:
                spawnEntryTemp.functionName = () => archerPool.DoSpawn(new Vector3(xRef, yRef, 0));
                break;
            default: break;
        }

        spawnQueue.Add(spawnEntryTemp);

        if (!isSpawning)    //check if spawn coroutine is playing
        {
            isSpawning = true;
            StartCoroutine(ProcessSpawnQueue(1));    // this coroutine will handle queue spawning to avoid multi spawns at the same time
        }
    }

    private void DisableAllEnemies()
    {
        for (int x = 0; x < disableEnemyList.Count; x++)
        {
            StopCoroutine(disableEnemyList[0]);
        }

        for (int y = 0; y < currentActiveEnemies.Count; y++)
        {
            currentActiveEnemies[0].SetActive(false);
            currentActiveEnemies.RemoveAt(0);
        }
    }
    //

    private IEnumerator ProcessSpawnQueue(float detectionRangeMultiplier) // this coroutine will iterate through spawn calls one at a time and toggle isSpawning when done 
    {
        while (spawnQueue.Count > 0)
        {
            if (IsSafeToSpawn(new Vector2(xRef, yRef), DetectionRadius * detectionRangeMultiplier))
            {
                GameObject obj = spawnQueue[0].functionName();

                if (spawnQueue[0].willChangeRotation)
                    obj.transform.rotation = spawnQueue[0].newRotation;

                disableEnemyList.Add(StartCoroutine(DisableEnemy(obj, mobAutoDestroy)));
                currentActiveEnemies.Add(obj);
                spawnQueue.RemoveAt(0);
            }

            yield return null;
        }

        isSpawning = false;
    }

    private IEnumerator DisableEnemy(GameObject obj, float timer)
    {
        yield return new WaitForSeconds(timer);
        obj.SetActive(false);
        currentActiveEnemies.Remove(obj);
    }

    private IEnumerator SpawnEnemyTimer(float interval)
    {
        yield return new WaitForSeconds(interval);

        if (!BossHandler.bossAlive) // check if boss is alive
        {
            SpawnEnemy();

            if (mobspawnInterval > 1f)
            {
                mobspawnInterval -= .05f;
            }
        }

        StartCoroutine(SpawnEnemyTimer(mobspawnInterval));
    }

    private IEnumerator SpawnObstacleTimer(float interval)
    {
        yield return new WaitForSeconds(interval);

        if (!BossHandler.bossAlive) // check if boss is alive
            SpawnObstacle();

        StartCoroutine(SpawnObstacleTimer(mobspawnInterval));
    }

    private IEnumerator SpawnCoinTimer(float interval)
    {
        yield return new WaitForSeconds(interval);

        if (!BossHandler.bossAlive) // check if boss is alive
            SpawnRandomCoin();

        StartCoroutine(SpawnCoinTimer(coinspawnInterval));
    }

    private void OnApplicationQuit()
    {
        spawnQueue.Clear();
    }
}
