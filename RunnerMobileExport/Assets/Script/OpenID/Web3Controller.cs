using System.Collections;
using UnityEngine;

using System;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication.PlayerAccounts;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

public class Web3Controller : MonoBehaviour
{
    public TextMeshProUGUI walletAddressText;
    public static Sprite StoredSprite;
    [SerializeField] private Image ProfileImage;
    [SerializeField] private TextMeshProUGUI coinsText;
    private string imageLink;
    public static string PlayerID = "ugsPlayerIds";

    private TaskCompletionSource<string> _tokenTcs = new TaskCompletionSource<string>();

    public async void ReceiveIdToken(string oidcID)
    {
        await UnityServices.InitializeAsync();
        LoginDataSingleton.oidcID = oidcID;
        _tokenTcs.SetResult(oidcID);
        await OidcSignIn();
    }

    public void ReceiveWallet(string wallet)
    {
        string manipulatedWallet = Regex.Replace(wallet, @"^(.{6}).*(.{4})$", "$1...$2");
        if (walletAddressText != null)
        {
            walletAddressText.text = manipulatedWallet;
            LoginDataSingleton.WalletAddress = wallet;
            LoginDataSingleton.ShortWallet = manipulatedWallet;
        }
    }

    private async Task OidcSignIn()
    {
        var token = await _tokenTcs.Task;
        await SignInWithOpenIdConnectAsync("oidc-otherpage", token);
    }

    async Task SignInWithOpenIdConnectAsync(string idProviderName, string idToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithOpenIdConnectAsync(idProviderName, idToken);
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
        catch (Exception ex)
        {
            // Catch any other unhandled exceptions
            Debug.LogException(ex);
        }
        UpdateCoins();
        PlayerPrefs.SetString(PlayerID, AuthenticationService.Instance.PlayerId);
    }

    public async void UpdateCoins()
    {
        if (coinsText != null)
        {
            var scoreResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync("coins");

            if (scoreResponse != null)
            {
                int coins = Convert.ToInt32(scoreResponse.Score);

                coinsText.text = coins.ToString();
                LoginDataSingleton.PlayerCoins = coins;
            }
            else
            {
                Debug.Log("Failed to retrieve player score for coins.");
            }
        }
    }

    public void ProfileChanged(string profileURL)
    {
        if (ProfileImage != null && profileURL != null)
        {
            imageLink = profileURL;
            StartCoroutine(GrabWebImage());
        }
    }

    public void SiweChange(string address)
    {
        Debug.Log("Auth state: " + address);
    }

    public void SiwopChange(string userId)
    {
        Debug.Log("Auth state: " + userId);
    }

    public void NetworkChange(string chainId)
    {
        Debug.Log("Network: " + chainId);
    }

    public void PaymentStatusChange(string paymentId, string status)
    {
        Debug.Log("Payment: " + paymentId + " status: " + status);
    }

    public IEnumerator GrabWebImage()
    {
        using (UnityWebRequest addImage = UnityWebRequestTexture.GetTexture(imageLink))
        {
            yield return addImage.SendWebRequest();

            if (addImage.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(addImage.error);
            }
            else
            {
                // Get downloaded asset bundle
                Texture2D myTexture = ((DownloadHandlerTexture)addImage.downloadHandler).texture;
                StoredSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
                //DontDestroyOnLoad(ProfileImage);
            }
        }
        ProfileImage.sprite = StoredSprite;
        LoginDataSingleton.PlayerSprite = StoredSprite;
    }
}