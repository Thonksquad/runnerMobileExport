using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using TMPro;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _username;
    [SerializeField] private Image _userPFP;
    [SerializeField] private GameObject ManualLogin;
    public string GooglePlayToken;
    public string GooglePlayError;

    private async void Start()
    {
        await Authenticate();
    }

    public void showOptions()
    {
        ManualLogin.SetActive(true);
    }

    public async Task Authenticate()
    {
        PlayGamesPlatform.Activate();
        await UnityServices.InitializeAsync();

        PlayGamesPlatform.Instance.Authenticate((success) =>
        {
            if (success == SignInStatus.Success)
            {
                Debug.Log("Login with Google Play");
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
                {
                    GooglePlayToken = code;
                    Debug.Log("Auth code is " + GooglePlayToken);
                });
            } else
            {
                GooglePlayError = "Failed to retrieve Google Play auth code";
                Debug.LogError("Login unsuccessful");
            }
        });

        await AuthenticateWithUnity();
    }

    private async Task AuthenticateWithUnity()
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGoogleAsync(GooglePlayToken);
            PlayerPrefs.SetString("ugsPlayerIds", AuthenticationService.Instance.PlayerId);
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
            throw;
        }
        catch (RequestFailedException ex)
        {
            Debug.LogException(ex);
            throw;
        }
    }

    /**
    public async void guestSignIn()
    {
        try
        {
            await SignInAnonymously();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        ManualLogin.SetActive(false);
    }

    async Task SignInAnonymously()
    {
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
            PlayerPrefs.SetString("ugsPlayerIds", AuthenticationService.Instance.PlayerId);
        };
        AuthenticationService.Instance.SignInFailed += s =>
        {
            // Take some action here...
            Debug.Log(s);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }


    // max characters = 50
    public void setPlayerName(string playerName)
    {
        try
        {
            AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

    }
    **/
}
