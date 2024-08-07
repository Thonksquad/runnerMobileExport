using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBoss : MonoBehaviour
{
    public bool isDead { get; protected set; } = false;

    public virtual void HandleDeath()
    {
        ActionSystem.onBossDeath(this);
        isDead = true;
      /**  _anim.CrossFade("dead", 0, 0);
        int deadLayer = LayerMask.NameToLayer("deadEnemy");
        gameObject.layer = deadLayer;**/
        //moved to individual classes
        //rb.bodyType = RigidbodyType2D.Dynamic;
        //rb.velocity = new Vector3(0, -10, 0);
    }
}
