using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 mousepos;
    private Camera mainCam;
    private Rigidbody2D rb;
    public float force;

    void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        mousepos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 bulletDirection = mousepos - transform.position;
        Vector3 bulletRotation = transform.position - mousepos;
        rb.velocity = new Vector3(90, 0).normalized * (force + CameraManager.Instance.CamSpeed);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

}
