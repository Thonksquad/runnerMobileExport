using UnityEngine;
using TMPro;

public class ReloadCoins : MonoBehaviour
{
    private TextMeshProUGUI coinsText; // System.Text to contain multi-bit names, obtain from API

    private void Awake()
    {
        TryGetComponent(out coinsText);
        ChangeCoins();
    }

    public async void ChangeCoins()
    {
        //coinsText.text = DBGrabUser.coinAllColected.ToString();
        coinsText.text = await UploadHandler.Instance.getCoins();
    }
}
