using UnityEngine;
using UnityEngine.UI;
using UnityServiceLocator;

public class VolumeSlider : MonoBehaviour
{
    public Slider _slider;
    private SoundManager _soundManager;
    private static float _sliderValue = 0.5f;

    void OnEnable()
    {
        _slider.value = _sliderValue;
    }

    void Start()
    {
        ServiceLocator.ForSceneOf(this).Get(out _soundManager);

        _slider.onValueChanged.AddListener(val =>
        {
            _sliderValue = val;
            _soundManager.ChangeMasterVolume(val);
        });
    }
}
