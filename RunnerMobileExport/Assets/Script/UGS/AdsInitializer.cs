using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour
{
    [SerializeField] string _androidGameId = "5530169";
    [SerializeField] string _iOSGameId = "5530168";
    [SerializeField] bool _testMode = true;
    private string _gameId;

    void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {

    }


    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }


}