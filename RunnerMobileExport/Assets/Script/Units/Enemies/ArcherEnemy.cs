using UnityEngine;

public class ArcherEnemy : BaseEnemy
{
    public ArcherAim myArm;
    [SerializeField] private SpriteRenderer arm;

    public override void OnEnable()
    {
        base.OnEnable();
        myArm.enabled = true;
        arm.enabled = true;
    }

    public override void Update()
    {
        base.Update();
        if ( isDead)
        {
            myArm.StopShooting();
            myArm.enabled = false;
            arm.enabled = false;
        } 
        else
        {
            Flip();
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
