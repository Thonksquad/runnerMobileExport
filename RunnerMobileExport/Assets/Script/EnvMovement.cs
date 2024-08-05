using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvMovement : MonoBehaviour
{

    public static float _speed = 0.04f; 
    [SerializeField] private float _nextBgX = 362f; 
    [SerializeField] private float _passedX = -140f;


    private void Update()
    {
        transform.position = new Vector3( transform.position.x - _speed, transform.position.y, transform.position.z);
        if ( transform.position.x < _passedX)
        {
            transform.position = new Vector3( _nextBgX, transform.position.y, transform.position.z);
        }
    }

}
