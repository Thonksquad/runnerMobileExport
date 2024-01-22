using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Authentication.PlayerAccounts;
using Unity.Services.Core;
using UnityEngine;

// External dependencies

#if UNITY_IPHONE

using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using AppleAuth.Native;
        
#endif


public class LeaderboardClass : MonoBehaviour
{


#if UNITY_IPHONE

    IAppleAuthManager m_AppleAuthManager;
    public string Token { get; private set; }
    public string Error { get; private set; }
        
#endif



    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        
    }

    public async void signInAsGuest()
    {
        try
        {
            await SignInAnonymously();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public async void signInWithID()
    {

#if UNITY_IPHONE

        try
        {
        
            LoginToApple();
            await SignInWithAppleAsync(Token);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
#endif

#if UNITY_ANDROID

        try
        {

            InitializePlayGamesLogin();
            LoginGoogle();

            string id = (PlayGamesLocalUser)Social.localUser).GetIdToken();
            await SignInWithGoogleAsync( id );
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

#endif



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

    ////////// android codes
#if UNITY_ANDROID

    void InitializePlayGamesLogin()
    {
        var config = new PlayGamesClientConfiguration.Builder()
            // Requests an ID token be generated.  
            // This OAuth token can be used to
            // identify the player to other services such as Firebase.
            .RequestIdToken()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();
    }

    void LoginGoogle()
    {
        Social.localUser.Authenticate(OnGoogleLogin);
    }

    void OnGoogleLogin(bool success)
    {
        if (success)
        {
            // Call Unity Authentication SDK to sign in or link with Google.
            Debug.Log("Login with Google done. IdToken: " + ((PlayGamesLocalUser)Social.localUser).GetIdToken());
        }
        else
        {
            Debug.Log("Unsuccessful login");
        }
    }



    async Task SignInWithGoogleAsync(string idToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithGoogleAsync(idToken);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

#endif


    ////////// apple codes
    ///



#if UNITY_IPHONE
    public void Initialize()
    {
        var deserializer = new PayloadDeserializer();
        m_AppleAuthManager = new AppleAuthManager(deserializer);
    }

    public void Update()
    {
        if (m_AppleAuthManager != null)
        {
            m_AppleAuthManager.Update();
        }
    }

    public void LoginToApple()
    {
        // Initialize the Apple Auth Manager
        if (m_AppleAuthManager == null)
        {
            Initialize();
        }

        // Set the login arguments
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        // Perform the login
        m_AppleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                var appleIDCredential = credential as IAppleIDCredential;
                if (appleIDCredential != null)
                {
                    var idToken = Encoding.UTF8.GetString(
                        appleIDCredential.IdentityToken,
                        0,
                        appleIDCredential.IdentityToken.Length);
                    Debug.Log("Sign-in with Apple successfully done. IDToken: " + idToken);
                    Token = idToken;
                }
                else
                {
                    Debug.Log("Sign-in with Apple error. Message: appleIDCredential is null");
                    Error = "Retrieving Apple Id Token failed.";
                }
            },
            error =>
            {
                Debug.Log("Sign-in with Apple error. Message: " + error);
                Error = "Retrieving Apple Id Token failed.";
            }
        );
    }

    async Task SignInWithAppleAsync(string idToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithAppleAsync(idToken);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
#endif



}
