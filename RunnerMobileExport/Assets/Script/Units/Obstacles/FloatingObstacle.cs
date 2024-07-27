using UnityEngine;

public class FloatingObstacle : BaseObstacle
{

    private void OnEnable()
    {
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 180));
    }

}
