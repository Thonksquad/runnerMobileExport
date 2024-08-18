using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvMovement : MonoBehaviour
{
    [SerializeField] public static float _speed = 8.5f;
    public Camera Camera;
    public float ParallaxEffect;

    private float _startPos, _length;
    private float nextBgX, passedX;

    private void Start()
    {
        _startPos = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;

        // Calculate the next background and passed positions
        nextBgX = _startPos + _length;
        passedX = _startPos - _length;
    }

    private void Update()
    {
        float distance = Camera.transform.position.x * ParallaxEffect;
        float movement = Camera.transform.position.x * (1 - ParallaxEffect);

        transform.position = new Vector3(_startPos + distance - _speed * Time.deltaTime, transform.position.y, transform.position.z);

        if (transform.position.x < passedX)
        {
            transform.position = new Vector3(nextBgX, transform.position.y, transform.position.z);
            _startPos = nextBgX;
        }
    }
}
