using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ModeToggle : MonoBehaviour, IPointerDownHandler
{
    public static bool isPCMode = true;
    [SerializeField] private Image _img;
    [SerializeField] private Sprite _pcMode, _mobileMode;

    private void Awake()
    {
        if (isPCMode)
        {
            _img.sprite = _pcMode;
        } else
        {
            _img.sprite = _mobileMode;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPCMode = !isPCMode;
        if (isPCMode)
        {
            _img.sprite = _pcMode;
        }
        else
        {
            _img.sprite = _mobileMode;
        }
    }
}
