using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReloadPfp : MonoBehaviour
{
    private Image ProfileImage; //Unity image to store the profile picture

    private void Awake()
    {
        TryGetComponent(out ProfileImage);
    }

    public void ChangeImage()
    {
        //ProfileImage.sprite = DBGrabUser.StoredSprite;
    }
}
