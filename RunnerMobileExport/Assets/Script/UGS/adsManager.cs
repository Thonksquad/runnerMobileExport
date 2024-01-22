using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class adsManager : MonoBehaviour
{
    public static adsManager Instance;
    [SerializeField] private GameObject videoChanceScreen;
    [SerializeField] private GameObject vcCloseBtn;
    [SerializeField] private GameObject vcCloseAndroidPos;
    public bool hasVideoChance = true;
    public TextMeshProUGUI vcTxt;
    public Color vcMainColor;
    public Color vcSecondColor;
    public float timeToChoose = 5f;
    public int vcMainFontSize = 240;
    public int vcSecondFontSize = 120;
    private bool vcTxtOnMainState = true;
    private bool clickedWatchBtn = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
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


        videoChanceScreen.SetActive(true);

#if UNITY_ANDROID
        vcCloseBtn.transform.position = vcCloseAndroidPos.transform.position;
#endif


        vcTxt.text = timeToChoose.ToString();
        vcTxt.color = vcMainColor;
        vcTxt.fontSize = vcMainFontSize;
        vcTxtOnMainState = true;
        StartCoroutine(videoChanceTimer());
        

    }


    public void playerWatchedAds()
    {
        ActionSystem.onAdRevive();
        Time.timeScale = 1;
        videoChanceScreen.SetActive(false);
        AudioListener.pause = false;
        SoundManager.Instance.TurnMusicOn();
    }

    private IEnumerator videoChanceTimer()
    {
        float vcTimer = timeToChoose;
        while (vcTimer > 0f)
        {
            vcTimer -= 0.5f;
            changeVCtext(vcTimer);
            yield return new WaitForSecondsRealtime(0.5f);
        }


        if (!clickedWatchBtn)
        {
            cancelVideoAds();
        }
        else
        {
            Debug.Log("show video");
        }
        clickedWatchBtn = false;
    }



    public void cancelVideoAds()
    {
        GameManager.Instance.playerHit();
        clickedWatchBtn = false;
        videoChanceScreen.SetActive(false);
        vcTxt.color = vcMainColor;
        vcTxt.fontSize = vcMainFontSize;
    }

    private void changeVCtext(float t)
    {
        if (vcTxtOnMainState)
        {
            vcTxt.color = vcSecondColor;
            vcTxt.fontSize = vcSecondFontSize;
            vcTxtOnMainState = false;
        }
        else
        {
            vcTxtOnMainState = true;
            vcTxt.color = vcMainColor;
            vcTxt.fontSize = vcMainFontSize;
        }
        vcTxt.text = t.ToString();
    }


}
