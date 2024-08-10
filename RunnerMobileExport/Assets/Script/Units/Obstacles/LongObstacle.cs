using UnityEngine;

public class LongObstacle : BaseObstacle
{

    private void OnEnable()
    {
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 180));
    }

}
