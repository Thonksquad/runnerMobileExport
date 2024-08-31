using System.Collections;
using UnityEngine;
using UnityServiceLocator;

public class ArcherAim : MonoBehaviour
{
    [SerializeField, Range(1, 100)] private float _rotationSpeed = 70;
    public SpawnPool enemyProjectilePool;
    [SerializeField] private Transform _spawnSpot;
    [SerializeField] private float firingCD;
    private float PlayerDistance;
    private bool canFire = false;
    public Player player;
    public Transform pivot;
    public Coroutine archerRoutine;

    private void Start()
    {
        ServiceLocator.ForSceneOf(this).Get(out player);
        pivot = transform.parent;
    }

    private void OnEnable()
    {
        canFire = true;
    }

    private void Update()
    {
        PlayerDistance = Vector2.Distance(transform.position, player.transform.position);
        var playerPos = player.transform.position;
        var dir = playerPos - transform.position;
        playerPos.z = 0;

        pivot.up = Vector3.MoveTowards(pivot.up, dir, _rotationSpeed * Time.deltaTime);
        //transform.up = Vector3.MoveTowards(transform.up, dir, _rotationSpeed * Time.deltaTime);

        if (PlayerDistance < 20 && transform.position.x + 7 > player.transform.position.x)
        {
            canFire = true;
        }

        if (player.gameOver)
        {
            canFire = false;
        }
    }

    private void OnBecameVisible()
    {
        archerRoutine = StartCoroutine(ArcherActivated());
    }

    public void StopShooting()
    {
        canFire = false;

        if (archerRoutine != null)
        {
            StopCoroutine(archerRoutine);
        }
        archerRoutine = null;
    }


    private IEnumerator ArcherActivated()
    {
        firingCD = 14f * (2 / (player.speed));
        yield return new WaitForSecondsRealtime(firingCD);
        if (canFire == true)
        {
            ArcherFire();
            canFire = false;
        }

       archerRoutine = StartCoroutine(ArcherActivated());
    }

    private void ArcherFire()
    {
        Vector2 dir = player.transform.position - transform.position; 
        GameObject arrow = enemyProjectilePool.SpawnGameObject(_spawnSpot.position);
        arrow.GetComponent<EnemyProjectile>().Init(dir);
    }
}
