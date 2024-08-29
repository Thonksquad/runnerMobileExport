using UnityEngine;
using UnityEngine.UI;
using UnityServiceLocator;


public class VolumeSlider : MonoBehaviour
{

    [SerializeField] private Slider _slider;

    private SoundManager _soundManager;


    void Start()
    {
        ServiceLocator.ForSceneOf(this).Get(out _soundManager);
        _soundManager.ChangeMasterVolume(_slider.value);
        _slider.onValueChanged.AddListener(val => _soundManager.ChangeMasterVolume(val));
    }

}
