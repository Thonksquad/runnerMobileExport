using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityServiceLocator;

public class Hound : MonoBehaviour
{

    [SerializeField] private float _speed = 0.04f;

    private Player player;

    private void Awake()
    {
        //player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        //player = Player.Instance;
        ServiceLocator.Global.Get(out player); // Global Service

    }

    private void Start()
    {
        Destroy(gameObject, 10);
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x - _speed, transform.position.y, transform.position.z);
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
                player.hp = 2;
                Destroy(gameObject);
            }
        }
    }
}
