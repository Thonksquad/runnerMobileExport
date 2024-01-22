using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameToggle : MonoBehaviour
{
    void Start()
    {
        if (ModeToggle.isPCMode)
        {
            gameObject.SetActive(false);
        } else
        {
            gameObject.SetActive(true);
        }
    }
}
