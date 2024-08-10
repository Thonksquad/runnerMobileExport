using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvMovement : MonoBehaviour
{

    [SerializeField] public static float _speed = 8.5f; 
    [SerializeField] private float _nextBgX = 362f; 
    [SerializeField] private float _passedX = -140f;


    private void Update()
    {
        transform.position = new Vector3( transform.position.x - _speed * Time.deltaTime, transform.position.y, transform.position.z);
        if ( transform.position.x < _passedX)
        {
            transform.position = new Vector3( _nextBgX, transform.position.y, transform.position.z);
        }
    }

}
