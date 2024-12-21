using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReloadCoins : MonoBehaviour
{
    private TextMeshProUGUI coinsText;

    private void Awake()
    {
        TryGetComponent(out coinsText);
    }

    private void Start()
    {
        UpdateCoins();
    }

    public void UpdateCoins()
    {
        if (coinsText != null)
        {
            coinsText.text = LoginDataSingleton.PlayerCoins.ToString();
        }
    }
}
