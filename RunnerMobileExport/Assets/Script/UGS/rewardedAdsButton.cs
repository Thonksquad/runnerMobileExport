using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityServiceLocator;

public class rewardedAdsButton : MonoBehaviour 
{

    [SerializeField] Button _showAdButton;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms
    [SerializeField] bool _testMode = true;

    private adsManager _adsManager;



    void Awake()
    {
        ServiceLocator.ForSceneOf(this).Register<rewardedAdsButton>(this); // Scene Scope
        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#elif UNITY_EDITOR
        _adUnitId = _androidAdUnitId; //Only for testing the functionality in the Editor
#endif

        Debug.Log(" id = " + _adUnitId);
        // Disable the button until the ad is ready to show:
        _showAdButton.interactable = false; 
    }

    private void Start()
    {
        ServiceLocator.ForSceneOf(this).Get(out _adsManager);
    }

    // Call this public method when you want to get an ad ready to show.
    public void LoadAd()
    {

    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId))
        {
            // Configure the button to call the ShowAd() method when clicked:
            _showAdButton.onClick.AddListener(ShowAd);
            // Enable the button for users to click:
            _showAdButton.interactable = true;
            //adsManager.Instance.activateAds();
        }
    }

    // Implement a method to execute when the user clicks the button:
    public void ShowAd()
    {
        // Disable the button:
        _showAdButton.interactable = false;
        _adsManager.activateAds();
        // Then show the ad:

    }



    void OnDestroy()
    {
        // Clean up the button listeners:
        _showAdButton.onClick.RemoveAllListeners();
    }



}
