using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Parallax : MonoBehaviour
{
    private float _startPos, _length;
    public Camera Camera;
    public float ParallaxEffect;

    private void Start()
    {
        _startPos = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void FixedUpdate()
    {
        float distance = Camera.transform.position.x * ParallaxEffect;
        float movement = Camera.transform.position.x * (1-ParallaxEffect);

        transform.position = new Vector3(_startPos + distance, transform.position.y, transform.position.z);

        if (movement > _startPos + _length)
        {
            _startPos += _length;
        } else if (movement < _startPos - _length)
        {
            _startPos -= _length;
        }
    }
}
