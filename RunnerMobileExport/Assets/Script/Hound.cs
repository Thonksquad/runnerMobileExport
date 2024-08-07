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
        Invoke("DoDisable", 10.0f);
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
