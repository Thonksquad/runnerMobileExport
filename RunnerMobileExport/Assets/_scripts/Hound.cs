using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hound : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Start()
    {
        Destroy(gameObject, 10);
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
