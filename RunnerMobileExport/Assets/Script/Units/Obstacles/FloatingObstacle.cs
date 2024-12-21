using UnityEngine;

public class FloatingObstacle : BaseObstacle
{

    public override void OnEnable()
    {
        base.OnEnable();
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 180));
    }

}
