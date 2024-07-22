using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;

#if UNITY_ANDROID && !UNITY_EDITOR
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif

using TMPro;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _username;
    [SerializeField] private TextMeshProUGUI _coins;
    [SerializeField] private Image _userPFP;
    [HideInInspector] public string googlePlayToken, googlePlayError;

    private Texture2D pfp;
    private Sprite sprite;
    private string pfpURL;

    const string coinsLB = "coins";
    private string userGooglePlayName = "";

    private async void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        PlayGamesPlatform.Activate();
        await InitializeGPS();
        await SignInWithGPGS(googlePlayToken);
#endif
        await UnityServices.InitializeAsync();
        await UGSLogin();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "MainMenu") return;
#if UNITY_ANDROID && !UNITY_EDITOR
        setPlayerName(userGooglePlayName);
#endif

        _coins.text = await UploadHandler.Instance.getCoins();

        if (sprite != null)
        {
            _userPFP.sprite = sprite;
        }
        else
        {
            if (googlePlayError == null)
            {
                StartCoroutine(LoadImage(pfpURL));
            }
        }
    }

    private async Task InitializeGPS()
    {
        Debug.Log("Initalizing services");
        if (!Application.isPlaying) return;
        if (UnityServices.State == ServicesInitializationState.Initialized) return;

        try
        {
            await UnityServices.InitializeAsync();

#if UNITY_ANDROID && !UNITY_EDITOR
        await AuthenticateGPS();
#endif

        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private Task AuthenticateGPS()
    {
        var tcs = new TaskCompletionSource<object>();
        PlayGamesPlatform.Instance.Authenticate(async (status) =>
        {
            if (status == SignInStatus.Success)
            {
                PlayGamesPlatform.Instance.RequestServerSideAccess(true, code => { Debug.Log($"Auth code is: {code}"); googlePlayToken = code; tcs.SetResult(null); });
                userGooglePlayName = PlayGamesPlatform.Instance.GetUserDisplayName();
                _username.text = userGooglePlayName;
                pfpURL = PlayGamesPlatform.Instance.GetUserImageUrl();
                StartCoroutine(LoadImage(pfpURL));
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
#endif

    async Task UGSLogin()
    {
        try
        {
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
                Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
                PlayerPrefs.SetString("ugsPlayerIds", AuthenticationService.Instance.PlayerId);

            };
            AuthenticationService.Instance.SignInFailed += s =>
            {
                Debug.Log(s);
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    private IEnumerator LoadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        } else
        {
            pfp = ((DownloadHandlerTexture)request.downloadHandler).texture;
            sprite = Sprite.Create(pfp, new Rect(0, 0, pfp.width, pfp.height), Vector2.one * 0.5f);
            _userPFP.sprite = sprite;
        }
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