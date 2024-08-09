using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hound : MonoBehaviour
{

    [SerializeField] private float _speed = 0.04f;

    private Player player;

    private void Awake()
    {
        player = Player.Instance;
    }

    private void Start()
    {
        Invoke("DoDisable", 20.0f);
    } 

    private void Update()
    {
        transform.position = new Vector3(transform.position.x - _speed, transform.position.y, transform.position.z);
    }

    private void OnBecameVisible()
    {
        if (player.onHound)
        {
            gameObject.SetActive(false);
        }
    }

    private void DoDisable()
    {
        gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            if (!player.onHound)
            {
                player.onHound = true;
                player.hp = 2;
                gameObject.SetActive(false);
                ActionSystem.onPlayerHoundPickup();
            }
        }
    }
}
