using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityServiceLocator;

public class adsManager : MonoBehaviour
{

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


    private GameManager _gameManager;
    private SoundManager _soundManager;
    private rewardedAdsButton _rewardedAdsButton;


    private void Awake()
    {
        ServiceLocator.ForSceneOf(this).Register<adsManager>(this); // Scene Scope

        clickedWatchBtn = false;
        hasVideoChance = true;
        videoChanceScreen.SetActive(false);
    }

    private void Start()
    {
        ServiceLocator.ForSceneOf(this).Get(out _gameManager);
        ServiceLocator.ForSceneOf(this).Get(out _soundManager);
        ServiceLocator.ForSceneOf(this).Get(out _rewardedAdsButton);
    }

    public void activateAds()
    {
        clickedWatchBtn = true;
    }

    public void showVideo()
    {

        _rewardedAdsButton.LoadAd();
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
        ActionSystem.onPlayerRevive();
        Time.timeScale = 1;
        videoChanceScreen.SetActive(false);
        AudioListener.pause = false;
        _soundManager.TurnMusicOn();
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
        _gameManager.playerDeath();
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
