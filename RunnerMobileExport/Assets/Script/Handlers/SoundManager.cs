using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : SingletonPersistent<SoundManager>
{
    [SerializeField] private AudioSource _musicSource, _effectSource;
    //[SerializeField] private Button _musicButton, _effectsButton;
    //private bool _toggleMusic, _toggleEffects;

    /**
 private void Awake()
 {
     if (Instance == null)
     {
         Instance = this;
         DontDestroyOnLoad(gameObject);
     } else Destroy(gameObject);

     //InitButtons();
 }


 private void InitButtons()
 {
     _toggleMusic = true;
     _toggleEffects = true;

     ColorBlock colors = _musicButton.colors;
     colors.normalColor = Color.green;
     _musicButton.colors = colors;

     colors = _effectsButton.colors;
     colors.normalColor = Color.green;
     _effectsButton.colors = colors;
 }

 public void ToggleMusic()
 {
     _toggleMusic = !_toggleMusic;
     _musicSource.mute = !_musicSource.mute;
     ColorBlock colors = _musicButton.colors;
     colors.normalColor = _toggleMusic ? Color.green : Color.red;
     _musicButton.colors = colors;
 }

 public void ToggleEffects()
 {
     _effectSource.mute = !_effectSource.mute;
     _toggleEffects = !_toggleEffects;

     ColorBlock colors = _effectsButton.colors;
     colors.normalColor = _toggleEffects ? Color.green : Color.red;
     _effectsButton.colors = colors;
 }
 **/

    public void PlaySound(AudioClip clip)
    {
        _effectSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip)
    {
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void TurnMusicOn()
    {
        _musicSource.mute = false;
    }

    public void ToggleMusic()
    {
        _musicSource.mute = !_musicSource.mute;
    }

    public void ToggleSound()
    {
        _effectSource.mute = !_effectSource.mute;
    }

    public void ChangeMasterVolume(float val)
    {
        AudioListener.volume = val;
    }
}
