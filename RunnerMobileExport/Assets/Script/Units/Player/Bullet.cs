using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 mousepos;
    private Camera mainCam;
    private Rigidbody2D rb;
    public float force;

    void OnEnable()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        mousepos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 bulletDirection = mousepos - transform.position;
        Vector3 bulletRotation = transform.position - mousepos;
        rb.velocity = new Vector3(90, 0).normalized * (force + CameraManager.Instance.CamSpeed);

        //Omnidirection
        //rb.velocity = new Vector3(bulletDirection.x, bulletDirection.y).normalized * force;
        //float rot = Mathf.Atan2(bulletRotation.y, bulletRotation.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, 0, rot+90);
    }

    void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }

    public void ReturnToPool()
    {
        Debug.Log("hit eye");
        gameObject.SetActive(false);
    }

}
