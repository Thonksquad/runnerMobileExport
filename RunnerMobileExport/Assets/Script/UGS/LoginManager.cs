using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _username;
    [SerializeField] private TextMeshProUGUI _coins;
    [SerializeField] private Image _userPFP;
    [HideInInspector] public string googlePlayToken, googlePlayError;

    private async void Awake()
    {
        PlayGamesPlatform.Activate();
        await InitializeServices();
    }

    private async void Start()
    {
        await SignInWithGPGS(googlePlayToken);
    }

    private async Task InitializeServices()
    {
        if (!Application.isPlaying) return;

        if (UnityServices.State == ServicesInitializationState.Initialized) return;

        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticateGPS();
            LoginUGS();
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    private Task AuthenticateGPS()
    {
        var tcs = new TaskCompletionSource<object>();

        PlayGamesPlatform.Instance.Authenticate((status) =>
        {
            if (status == SignInStatus.Success)
            {
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code => { Debug.Log($"Auth code is: {code}"); googlePlayToken = code; tcs.SetResult(null); });
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
            //PlayerPrefs.SetString("ugsPlayerIds", AuthenticationService.Instance.PlayerId);
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

    public async void LoginUGS()
    {
        try
        {
            await AnonymousLogin();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    async Task AnonymousLogin()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
            PlayerPrefs.SetString("ugsPlayerIds", AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            Debug.Log(s);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
