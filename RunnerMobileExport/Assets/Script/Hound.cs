using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityServiceLocator;

public class Hound : MonoBehaviour
{
    [SerializeField] private float _speed = 0.04f;

    private Player player;

    private void Start()
    {
        Destroy(gameObject, 15);
    }

    private void OnEnable()
    {
        ServiceLocator.ForSceneOf(this).Get(out player);
        _speed = player.speed;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x - _speed * Time.deltaTime, transform.position.y, transform.position.z);
    }

    private void OnBecameVisible()
    {
        if (player.onHound)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            if (!player.onHound)
            {
                player.onHound = true;
                player.hp += 1;
                Destroy(gameObject);
            }
        }
    }
}
