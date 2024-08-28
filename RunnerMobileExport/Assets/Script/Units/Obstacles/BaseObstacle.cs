using UnityEngine;
using UnityServiceLocator;

public class BaseObstacle : MonoBehaviour
{

    [SerializeField] private float _speed;

    private Player _player;

    public virtual void OnEnable()
    {
        ServiceLocator.ForSceneOf(this).Get(out _player);
        _speed = _player.speed;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x - _speed * Time.deltaTime, transform.position.y, transform.position.z);
    }

}
