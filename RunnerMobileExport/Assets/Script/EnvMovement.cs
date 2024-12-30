using UnityEngine;
using UnityServiceLocator;

public class EnvMovement : MonoBehaviour
{
    public ParallaxLayer _parallaxLayer;
    private float _startPos, _length;
    private Player _player;
    private ParallaxManager _parallaxManager;
    private ParallaxSetting _parallaxSetting;

    private void Start()
    {
        ServiceLocator.ForSceneOf(this).Get(out _player);
        ServiceLocator.ForSceneOf(this).Get(out _parallaxManager);
        _startPos = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
        _parallaxSetting = _parallaxManager.parallaxSettings.Find(setting => setting.ParallaxLayer == _parallaxLayer);
    }

    private void Update()
    {
        if (transform.position.x < _startPos - _length)
        {
            transform.position = new Vector3(_startPos, transform.position.y, transform.position.z);
        }

        transform.position = new Vector3(
            transform.position.x - _player.speed * _parallaxSetting.parallaxEffect * Time.deltaTime,
            transform.position.y,
            transform.position.z);
    }
}
