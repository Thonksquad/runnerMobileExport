using UnityEngine;

public class BaseObstacle : MonoBehaviour
{

    [SerializeField] private float _speed = 0.04f;

    private void Update()
    {
        transform.position = new Vector3(transform.position.x - _speed, transform.position.y, transform.position.z);
    }

}
