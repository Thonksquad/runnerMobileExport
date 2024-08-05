using UnityEngine;
using UnityServiceLocator;


public class Bullet : PoolMember
{

    private Vector3 mousepos;
    private Camera mainCam;
    private Rigidbody2D rb;
    public float force; 

    public Transform spawnLocation; 
    private Player player;


    private void Start()
    {
        ServiceLocator.ForSceneOf(this).Get(out player);
        spawnLocation = player.bulletTransform;
    }

    public override void OnEnable()
    {
        ServiceLocator.ForSceneOf(this).Get(out player);
        spawnLocation = player.bulletTransform;
        transform.position = spawnLocation.position;
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        mousepos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 bulletDirection = mousepos - transform.position;
        Vector3 bulletRotation = transform.position - mousepos;
        rb.velocity = new Vector3(90, 0).normalized * (force + CameraManager.Instance.CamSpeed);
    }


    /*
    void OnBecameInvisible()
    {
        //Destroy(gameObject);
        ReturnToPool();
    }
    */


}
