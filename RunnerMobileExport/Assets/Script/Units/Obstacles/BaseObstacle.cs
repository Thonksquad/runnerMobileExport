using UnityEngine;
using UnityServiceLocator;

public class BaseObstacle : MonoBehaviour
{
    public ParallaxLayer _parallaxLayer = ParallaxLayer.BlackBackground;
    private ParallaxManager _parallaxManager;
    private ParallaxSetting _parallaxSetting;
    private Player _player;

    public virtual void OnEnable()
    {
        ServiceLocator.ForSceneOf(this).Get(out _player);
        ServiceLocator.ForSceneOf(this).Get(out _parallaxManager);
        _parallaxSetting = _parallaxManager.parallaxSettings.Find(setting => setting.ParallaxLayer == _parallaxLayer);
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x - _player.speed * _parallaxSetting.parallaxEffect * Time.deltaTime, transform.position.y, transform.position.z);
    }
}
