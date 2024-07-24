using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public float CamSpeed;
    public Player player;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!player.gameOver)
        {
            //transform.position += new Vector3(CamSpeed * Time.deltaTime, 0, 0);
        }
    }
}
