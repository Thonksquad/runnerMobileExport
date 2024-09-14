using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1Bouncing : MonoBehaviour
{
    internal float bounceStrenght = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
         if (collision.tag == "Ground")
        {

        }
    }
}
