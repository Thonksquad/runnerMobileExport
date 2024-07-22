using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

public class VolumeSlider : MonoBehaviour
{
    private static float _OldValue = .5f;
    [SerializeField] private Slider _slider;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        SoundManager.Instance.ChangeMasterVolume(_slider.value);
        _slider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeMasterVolume(val));
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ChangeValue(float val)
    {
        _OldValue = val;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _slider.value = _OldValue;
    }
}
