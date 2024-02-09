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
        vcCoin.text = GameManager.coins.ToString();
    }

    public async void loadStatsGo()
    {
        goScore.text = GameManager.distance.ToString();
        goCoin.text = GameManager.coins.ToString();

        int highScore;
        if (int.TryParse(await UploadHandler.Instance.getPlayerScore(), out highScore))
        {
            Debug.Log(highScore.ToString());
        } else
        {
            goHighScore.text = await UploadHandler.Instance.getPlayerScore();
        }

        if (highScore == null) return;
        if (GameManager.distance > highScore)
        {
            goHighScore.text = highScore.ToString();
        } else
        {
            goHighScore.text = await UploadHandler.Instance.getPlayerScore();
        }
    }


}
