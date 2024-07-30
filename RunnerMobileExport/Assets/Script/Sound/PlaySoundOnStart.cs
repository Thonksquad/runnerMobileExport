using UnityEngine;

public class PlaySoundOnStart : MonoBehaviour
{
    [SerializeField] private AudioClip _clip;

    /*
    private void Start()
    {
        SoundManager.Instance.PlaySound(_clip);
    }
    */

    public void OnEnable()
    {
        SoundManager.Instance.PlaySound(_clip);
    }
}
