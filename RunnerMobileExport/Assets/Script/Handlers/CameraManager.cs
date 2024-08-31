using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public float CamSpeed;


    private void Awake()
    {
        Instance = this;
    }

}
