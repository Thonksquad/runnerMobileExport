using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObstacle : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 0, 360 * 1 * Time.deltaTime);
    }
}
