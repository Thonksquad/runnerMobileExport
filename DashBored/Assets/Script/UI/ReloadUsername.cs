using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReloadUsername : MonoBehaviour
{
    private TextMeshProUGUI discordName; // System.Text to contain multi-bit names, obtain from API

    private void Awake()
    {
        TryGetComponent(out discordName);
    }

    public void ChangeUsername()
    {
        discordName.text = DBGrabUser.discordNameLeader;
    }
}
