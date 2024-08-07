using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B1HomingProjectile : MonoBehaviour
{
    internal float bulletSpeed;
    internal Transform target;
    private float addedRotation;

    internal void BulletStart(Transform target , float velocity, float addedRotate) // need to be called every spawn
    {
        bulletSpeed = velocity;
        this.target = target;
        addedRotation = addedRotate;
        GetComponent<TrailRenderer>().Clear();

        StartCoroutine(MoveTowardsTarget());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator MoveTowardsTarget()
    {
        Vector3 targ = target.transform.position;
        targ.z = 0f;

        Vector3 objectPos = transform.position;
        targ.x = targ.x - objectPos.x;
        targ.y = targ.y - objectPos.y;

        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle + addedRotation));

        while (gameObject.activeInHierarchy)
        {
            transform.Translate(-transform.right * bulletSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
