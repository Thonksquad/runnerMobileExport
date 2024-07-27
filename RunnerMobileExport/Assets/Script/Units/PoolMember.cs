using UnityEngine;
using UnityEngine.Pool;


public class PoolMember : MonoBehaviour
{

    [SerializeField] private float _enemyDestroyTimer = 10f;
    [SerializeField] private float _minSpawnX = 45f;
    [SerializeField] private float _maxSpawnX = 55f;
    [SerializeField] private float _minSpawnY = -6f;
    [SerializeField] private float _maxSpawnY = 6f;

    private IObjectPool<PoolMember> _pool;

    public void SetPool(IObjectPool<PoolMember> pool)
    {
        _pool = pool;
    }

    private void OnEnable()
    {
        //Invoke(nameof(ReturnToPool), _enemyDestroyTimer);
        transform.position = new Vector3(Random.Range(_minSpawnX, _maxSpawnX), Random.Range(_minSpawnY, _maxSpawnY), 0f);
    }

    public void ReturnToPool()
    {
        _pool.Release(this);
    }

    /*
    private void TypeManager()
    {
        switch (_poolMemberType)
        {
            case PoolMemberType.bat:
                // reset animation
                break;

            case PoolMemberType.floatingObstacle:
                transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 180));
                break;

            case PoolMemberType.longObstacle:
                transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 180));
                break;

            default:
                break;
        }
    }
    */
}

