using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReloadCoins : MonoBehaviour
{
    private TextMeshProUGUI coinsText; // System.Text to contain multi-bit names, obtain from API

    private void Awake()
    {
        TryGetComponent(out coinsText);
    }
     
}
