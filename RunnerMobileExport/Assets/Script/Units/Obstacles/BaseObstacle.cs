using UnityEngine;

public class BaseObstacle : MonoBehaviour
{

    [SerializeField] private float _speed = 8f;

    private void Update()
    {
        transform.position = new Vector3(transform.position.x - _speed * Time.deltaTime, transform.position.y, transform.position.z);
    }

}
