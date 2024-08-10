using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherEnemy : BaseEnemy
{
    [SerializeField] private ArcherAim myArm;
    [SerializeField] private SpriteRenderer arm;

    public override void Update()
    {
        base.Update();
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
