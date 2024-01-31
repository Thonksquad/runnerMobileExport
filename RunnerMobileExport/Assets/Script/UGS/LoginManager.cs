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

    public GameObject loginPnl;

    private async void Awake()
    {
        loginPnl.SetActive(false);
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }


        
    }

    public void showOptions()
    {
        loginPnl.SetActive(true);
    }




    public void googlePlaySignIn()
    {

        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        loginPnl.SetActive(false);

    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {

            // Continue with Play Games Services
            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string image = PlayGamesPlatform.Instance.GetUserImageUrl();

            _username.text = name;

        }
        else
        {
            // Disable your integration with Play Games Services or show a login button
            // to ask users to sign-in. Clicking it should call
            // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).

            _username.text = "Guest " + UnityEngine.Random.Range(100, 5000);
        }
    }


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
        loginPnl.SetActive(false);
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
}
