using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReloadUsername : MonoBehaviour
{
    private TextMeshProUGUI usernameText; // System.Text to contain multi-bit names, obtain from API

    private void Awake()
    {
        TryGetComponent(out usernameText);
    }

    private void Start()
    {
        UpdateUsername();
    }

    public void UpdateUsername()
    {
        if (LoginDataSingleton.ShortWallet != null)
        {
            usernameText.text = LoginDataSingleton.ShortWallet;
        }
    }
}
