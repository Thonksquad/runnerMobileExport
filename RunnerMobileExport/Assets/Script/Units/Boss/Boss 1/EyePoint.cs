using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyePoint : MonoBehaviour
{
    [SerializeField] private Transform playerTarget;
    private Vector3 targ;
    internal bool doFollow;

    private void FixedUpdate()
    {
        if (doFollow)
        {
            Vector3 targ = playerTarget.transform.position;
            targ.z = 0f;
            targ.x = targ.x - transform.position.x;
            targ.y = targ.y - transform.position.y;
            float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
             transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 180));
        }
    }
}
