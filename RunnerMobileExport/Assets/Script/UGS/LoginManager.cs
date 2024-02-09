using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using Newtonsoft.Json;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _username;
    [SerializeField] private TextMeshProUGUI _coins;
    [SerializeField] private Image _userPFP;
    [HideInInspector] public string googlePlayToken, googlePlayError;
    const string coinsLB = "coins";

    private async void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        PlayGamesPlatform.Activate();
        await InitializeGPS();
        await SignInWithGPGS(googlePlayToken);
#endif
        await UnityServices.InitializeAsync();
        await UGSLogin();
    }

    private async Task InitializeGPS()
    {
        Debug.Log("Initalizing services");
        if (!Application.isPlaying) return;
        if (UnityServices.State == ServicesInitializationState.Initialized) return;

        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticateGPS();
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    private Task AuthenticateGPS()
    {
        var tcs = new TaskCompletionSource<object>();

        PlayGamesPlatform.Instance.Authenticate(async (status) =>
        {
            if (status == SignInStatus.Success)
            {
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code => { Debug.Log($"Auth code is: {code}"); googlePlayToken = code; tcs.SetResult(null); });
                string displayName = PlayGamesPlatform.Instance.GetUserDisplayName();
                _username.text = displayName;
            }
            else
            {
                googlePlayError = "Failed to retrieve Google play games authorization code.";
                Debug.Log($"{googlePlayError}");
                tcs.SetException(new Exception("Failed."));
            }
        });

        return tcs.Task;
    }

    private async Task SignInWithGPGS(string authCode)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    async Task UGSLogin()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
            Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
            PlayerPrefs.SetString("ugsPlayerIds", AuthenticationService.Instance.PlayerId);
            grabCoins();
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            Debug.Log(s);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private async void grabCoins()
    {
        Debug.Log("Grabbing player coins");
        try
        {
            var coins = await LeaderboardsService.Instance.GetPlayerScoreAsync(coinsLB);
            string playerCoins = JsonConvert.SerializeObject(coins);
            _coins.text = playerCoins;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            _coins.text = "0";
        }
    }

}
