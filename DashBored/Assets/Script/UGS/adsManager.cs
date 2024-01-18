using System.Collections;
using TMPro;
using UnityEngine;

public class adsManager : MonoBehaviour
{
    public static adsManager Instance;
    [SerializeField] private GameObject videoChanceScreen;
    public bool hasVideoChance = true;
    public TextMeshProUGUI vcTxt;
    public Color vcMainColor;
    public Color vcSecondColor;
    public float timeToChoose = 3f;
    public int vcMainFontSize = 240;
    public int vcSecondFontSize = 120;
    private bool vcTxtOnMainState = true;
    private bool clickedWatchBtn = false;


    private void Awake()
    {
        Instance = this;
        clickedWatchBtn = false;
        hasVideoChance = true;
        videoChanceScreen.SetActive(false);
    }

    public void activateAds()
    {
        clickedWatchBtn = true;
    }

    public void showVideo()
    {
        rewardedAdsButton.Instance.LoadAd();
        clickedWatchBtn = false;
        hasVideoChance = false;
        Time.timeScale = 0;
        SoundManager.Instance.ToggleMusic();
        videoChanceScreen.SetActive(true);
        StartCoroutine(videoChanceTimer());
    }


    public void playerWatchedAds()
    {
        Time.timeScale = 1;
        videoChanceScreen.SetActive(false);
        AudioListener.pause = false;
        SoundManager.Instance.TurnMusicOn();
    }

    private IEnumerator videoChanceTimer()
    {
        float vcTimer = timeToChoose;
        while (vcTimer >= 0f)
        {
            yield return new WaitForSeconds(0.5f);
            vcTimer -= 0.5f;
            vcTxt.text = vcTimer.ToString();
            changeVCtext();
        }

        vcTxt.text = timeToChoose.ToString();
        vcTxt.color = vcMainColor;
        vcTxt.fontSize = vcMainFontSize;

        if (!clickedWatchBtn)
        {
            GameManager.Instance.playerDeath();
        }
        clickedWatchBtn = false;
    }

    public void cancelVideoAds()
    {
        GameManager.Instance.playerDeath();
        clickedWatchBtn = false;
        videoChanceScreen.SetActive(false);
        vcTxt.color = vcMainColor;
        vcTxt.fontSize = vcMainFontSize;
        hasVideoChance = true;
    }

    private void changeVCtext()
    {
        
        if (vcTxtOnMainState)
        {
            vcTxt.color = vcSecondColor;
            vcTxt.fontSize = vcSecondFontSize;
        }
        else
        {
            vcTxt.color = vcMainColor;
            vcTxt.fontSize = vcMainFontSize;
        }
    }


}
