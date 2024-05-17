using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;

    public GameObject ceiling;
    public GameObject floor;
    public GameObject ceiling2;
    public GameObject floor2;
    public GameObject bg;
    public GameObject bg2;
    public GameObject volcano;
    public GameObject volcano2;
    public GameObject backdrop;
    public GameObject backdrop2;
    public GameObject player;

    private void Awake()
    {
        Instance = this;
    }


    private void Update()
    {
        if (player.transform.position.x > floor.transform.position.x + 251)
        {
            floor.transform.position += new Vector3(502, 0, 0);
            ceiling.transform.position += new Vector3(502, 0, 0);
            bg.transform.position += new Vector3(502, 0, 0);
            volcano.transform.position = (bg2.transform.position + new Vector3(251, -7f, 0));
            backdrop.transform.position += new Vector3(502, 0, 0);
        }

        if (player.transform.position.x > (floor2.transform.position.x + 251))
        {
            floor2.transform.position += new Vector3(502, 0, 0);
            ceiling2.transform.position += new Vector3(502, 0, 0);
            bg2.transform.position += new Vector3(502, 0, 0);
            volcano2.transform.position = (bg.transform.position + new Vector3(251, -7f, 0));
            backdrop2.transform.position += new Vector3(502, 0, 0);
        }
    }
}
