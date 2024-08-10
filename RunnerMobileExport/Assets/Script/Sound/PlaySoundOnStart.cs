using UnityEngine;

public class PlaySoundOnStart : MonoBehaviour
{
    [SerializeField] private AudioClip _clip;


    public void OnEnable()
    {
        SoundManager.Instance.PlaySound(_clip);
    }
}