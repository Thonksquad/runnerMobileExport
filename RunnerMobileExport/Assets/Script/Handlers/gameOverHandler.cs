using TMPro;
using UnityEngine;

public class gameOverHandler : MonoBehaviour
{

    public TextMeshProUGUI vcScore;
    public TextMeshProUGUI vcHighScore;
    public TextMeshProUGUI vcCoin;

    public TextMeshProUGUI goScore;
    public TextMeshProUGUI goHighScore;
    public TextMeshProUGUI goCoin;


    [HideInInspector]
    public static gameOverHandler Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public async void loadStatsVc()
    {
        vcScore.text = GameManager.distance.ToString();
        vcHighScore.text = await UploadHandler.Instance.getPlayerScore();
        vcCoin.text = await UploadHandler.Instance.getCoins();
    }

    public async void loadStatsGo()
    {
        goScore.text = GameManager.distance.ToString();
        goHighScore.text = await UploadHandler.Instance.getPlayerScore();
        goCoin.text = await UploadHandler.Instance.getCoins();
    }


}
