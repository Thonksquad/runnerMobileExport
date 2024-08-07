using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherEnemy : BaseEnemy
{
    [SerializeField] private ArcherAim myArm;
    [SerializeField] private SpriteRenderer arm;

    private void Update()
    {
        if (!isDead)
        {
            Flip();
        } else
        {
            myArm.StopShooting();
            myArm.enabled = false;
            arm.enabled = false;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        myArm.enabled = true;
        arm.enabled = true;
    }

    private void Flip()
    {
        if (transform.position.x > player.transform.position.x)
        {
            transform.rotation = Quaternion.identity;
            arm.flipY = false;
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            arm.flipY = true;
        }
    }

}
