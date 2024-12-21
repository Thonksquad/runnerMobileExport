using UnityEngine;
using UnityServiceLocator;

public class PlaySoundOnStart : MonoBehaviour
{
    [SerializeField] private AudioClip _clip;

    private SoundManager _soundManager;

    

    public void OnEnable()
    {
        ServiceLocator.ForSceneOf(this).Get(out _soundManager);
        _soundManager.PlaySound(_clip);
    }
}
