using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityServiceLocator;

public class Hound : MonoBehaviour
{
    [SerializeField] private float _speed = 0.04f;
    public ParallaxLayer _parallaxLayer = ParallaxLayer.BlackBackground;
    private ParallaxManager _parallaxManager;
    private ParallaxSetting _parallaxSetting;
    private Player _player;

    private void Start()
    {
        Destroy(gameObject, 15);
    }

    private void OnEnable()
    {
        ServiceLocator.ForSceneOf(this).Get(out _player);
        ServiceLocator.ForSceneOf(this).Get(out _parallaxManager);
        _parallaxSetting = _parallaxManager.parallaxSettings.Find(setting => setting.ParallaxLayer == _parallaxLayer);
        _speed = _player.speed;
    }

    private void Update()
    {
        transform.position = new Vector3(
        transform.position.x - _player.speed * _parallaxSetting.parallaxEffect * Time.deltaTime,
        transform.position.y,
        transform.position.z);
    }

    private void OnBecameVisible()
    {
        if (_player.onHound)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            if (!_player.onHound)
            {
                _player.onHound = true;
                _player.hp += 1;
                Destroy(gameObject);
            }
        }
    }
}
