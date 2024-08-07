using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1MeleeEye : MonoBehaviour
{
    [SerializeField] float timeBetweenAttacks;
    [SerializeField] List<Transform> targetPosList;

    private Vector3 origPosition;
    private int targetPos;
    private LineRenderer trailLine;
    private int previousTarget = 99;

    private void Awake()
    {
        trailLine = GetComponent<LineRenderer>();
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(LookingMode());
    }

    private void FixedUpdate()
    {
        trailLine.SetPosition(0, new Vector3(transform.parent.position.x, transform.parent.position.y, -1));
        trailLine.SetPosition(1, new Vector3(transform.position.x, transform.position.y, -1));
    }

    /*private IEnumerator Intro()
    {
        float newT = Time.time;    // REPLACE WITH NEW TIMER SYSTEM
        float newTimer = 0.5f;
        while (Time.time < newT + newTimer)
        {
            transform.localPosition = transform.localPosition += Vector3.left * Time.deltaTime;
            yield return null;
        }

        yield return null;
        StartCoroutine(LookingMode());
    }*/

    private IEnumerator LookingMode()
    {
        float tempT = Time.time + timeBetweenAttacks;

        targetPos = Random.Range(0, targetPosList.Count - 1);

        while (previousTarget == targetPos) // Force this eye to not target the same place it did before
        {
            targetPos = Random.Range(0, targetPosList.Count - 1);
            yield return null;
        }
        previousTarget = targetPos;

        Vector3 targ = targetPosList[targetPos].position;
        targ.z = 0f;
        targ.x = targ.x - transform.position.x;
        targ.y = targ.y - transform.position.y;
        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
        Quaternion targetRot = Quaternion.Euler(new Vector3(0, 0, angle - 180));

        while (Time.time < tempT)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, Time.deltaTime * 21);
            yield return null;
        }

        StartCoroutine(DoLunge());
    }
    private IEnumerator DoLunge()
    {
        origPosition = transform.localPosition;
       // Vector3 targetPosition = targetPosList[Random.Range(0,targetPosList.Count-1)].localPosition;

        float newT = Time.time;    // REPLACE WITH NEW TIMER SYSTEM
        float newTimer = 0.5f;
        while (Time.time < newT + newTimer)
        {
            yield return null;
        }
        //lunge
        while (Vector3.Distance (transform.localPosition, targetPosList[targetPos].localPosition) > 0.065f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosList[targetPos].localPosition, Time.deltaTime * 2.2f);
        //    Debug.Log(Vector3.Distance(transform.position, targetPos));
            yield return null;
        }
        transform.localPosition = targetPosList[targetPos].localPosition;   
        //goBack
        while (Vector3.Distance(transform.localPosition, origPosition) > 0.035f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, origPosition, Time.deltaTime * 6.25f);
            yield return null;
        }
        transform.localPosition = origPosition;

        StartCoroutine(LookingMode());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
            collision.gameObject.SetActive(false);
    }
}
