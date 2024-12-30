using UnityEngine;
using UnityServiceLocator;

public class ArcherEnemy : BaseEnemy
{
    public ParallaxLayer _parallaxLayer = ParallaxLayer.BlackBackground;
    private ParallaxManager _parallaxManager;
    private ParallaxSetting _parallaxSetting;
    private Player _player;

    public ArcherAim myArm;
    [SerializeField] private SpriteRenderer arm;

    public override void OnEnable()
    {
        base.OnEnable();
        myArm.enabled = true;
        arm.enabled = true;
        ServiceLocator.ForSceneOf(this).Get(out _player);
        ServiceLocator.ForSceneOf(this).Get(out _parallaxManager);
        _parallaxSetting = _parallaxManager.parallaxSettings.Find(setting => setting.ParallaxLayer == _parallaxLayer);
    }

    public override void Update()
    {
        transform.position = new Vector3(
        transform.position.x - _player.speed * _parallaxSetting.parallaxEffect * Time.deltaTime,
        transform.position.y,
        transform.position.z);

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
