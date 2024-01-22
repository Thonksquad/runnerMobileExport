using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    private float savedSpd;
    public float CamSpeed;
    public Player player;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        ActionSystem.onAdRevive += SpeedBoost;
    }

    private void OnDisable()
    {
        ActionSystem.onAdRevive -= SpeedBoost;
    }

    private void SpeedBoost()
    {
        savedSpd = CamSpeed;

        if (savedSpd < 15)
        {
            CamSpeed = 20;
        }

        Invoke(nameof(StopBoost), .5f);
    }

    private void StopBoost()
    {
        CamSpeed = savedSpd;
    }

    void Update()
    {
        if (!player.gameOver)
        {
            transform.position += new Vector3(CamSpeed * Time.deltaTime, 0, 0);
        }
    }
}
