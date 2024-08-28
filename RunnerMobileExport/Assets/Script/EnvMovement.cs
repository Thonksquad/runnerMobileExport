using UnityEngine;
using UnityServiceLocator;

public class EnvMovement : MonoBehaviour
{
    [Range(0f, 1f)] public float ParallaxEffect; 

    private float _startPos, _length;


    private Player _player;

    //public List<EnvMovement> s;

    private void Start()
    {
        ServiceLocator.ForSceneOf(this).Get(out _player);

        _startPos = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x; 
    }

    private void Update()
    {
        if (transform.position.x < _startPos - _length)
        {
            transform.position = new Vector3( _startPos , transform.position.y, transform.position.z );
        }

        transform.position = new Vector3(
            transform.position.x - _player.speed * ParallaxEffect * Time.deltaTime,
            transform.position.y,
            transform.position.z); 
    }
}
