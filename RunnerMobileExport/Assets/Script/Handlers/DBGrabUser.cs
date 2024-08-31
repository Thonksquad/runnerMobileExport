using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Text;
using TMPro;
using System.Security.Cryptography;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DBGrabUser : SingletonPersistent<DBGrabUser>
{
    public static int coinAllColected; // Obtained from API for total coins player has
    public static Sprite StoredSprite; //Unity image to store the profile picture
    public static string discordNameLeader; //used for discord name
    public static string imageLink; //URL for image, link
    public static int highScore; //Highscore stored in DB
    public static string baseURL = "https://ecmgqf1op7.execute-api.us-east-1.amazonaws.com/";
    public static string token2 = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJnYW1lIiwiZXhwIjoxNjk0NjM3MDg4fQ.6JTypL12FsooHoAUt4b-BuMPnyFuZqsGCLN9Cf_CZYw";
    public static string discordId; //discord developer id
    public static string playerID; //player ID
    public static int counterSendScoreOneTime = 0; //Counter increases when score is sent
    public static int mainSceneAccess = 0; //Counter increases when score is sent

    private string plainText = "Working";
    private static readonly byte[] key = Encoding.UTF8.GetBytes("NewPassword12311NewPassword12311");
    private static readonly byte[] iv = Encoding.UTF8.GetBytes("Tsalt1wtQt9kHuHg");

    private ReloadPfp pfpImage;
    private ReloadUsername usernameText;
    private ReloadCoins userCoins;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        pfpImage = GameObject.FindGameObjectWithTag("profilepic").GetComponent<ReloadPfp>();
        usernameText = GameObject.FindGameObjectWithTag("username").GetComponent<ReloadUsername>();
        userCoins = GameObject.FindGameObjectWithTag("coins").GetComponent<ReloadCoins>();
        string encrypted = Encrypt(plainText);
        //Debug.Log(encrypted);
        Decrypt(encrypted);

        if (DBGrabUser.discordId != null)
        {
            StartCoroutine(DBGrabUser.Instance.GrabDiscordData());
        }
        StartCoroutine(DBGrabUser.Instance.GrabGameData());
    }


    public static string Encrypt(string plainText)
    {
        byte[] encrypted;
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.Key = key;
            aes.IV = iv;

            // Create an encryptor to perform the stream transform.
            ICryptoTransform encryptor = aes.CreateEncryptor();

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }
        return System.Convert.ToBase64String(encrypted);
    }

    public static string Decrypt(string cipherText)
    {
        byte[] cipherBytes = System.Convert.FromBase64String(cipherText);
        string plaintext = null;

        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.Key = key;
            aes.IV = iv;

            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = aes.CreateDecryptor();

            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        return plaintext;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mainSceneAccess++;
        if (mainSceneAccess > 1 && scene.name == "Main Menu" && discordId != null)
        {
            pfpImage = GameObject.FindGameObjectWithTag("profilepic").GetComponent<ReloadPfp>();
            pfpImage.ChangeImage();
            usernameText = GameObject.FindGameObjectWithTag("username").GetComponent<ReloadUsername>();
            usernameText.ChangeUsername();
            userCoins = GameObject.FindGameObjectWithTag("coins").GetComponent<ReloadCoins>();
            StartCoroutine(GrabGameData());
            userCoins.ChangeCoins();
        }
    }

    public void ReceiveDiscordID(string id)
    {
        discordId = id;
        StartCoroutine(GrabDiscordData());
        StartCoroutine(GrabGameData());
    }

    [System.Serializable]
    public class UserGameData
    {
        public int total_coins;
        public int high_score;
    }

    [System.Serializable]
    public class UserDiscordData
    {
        public int player_id;
        public string player_name; //Unused but caching it regardless
        public string discord_avatar_url;
        public string discord_username;
    }

    public IEnumerator GrabDiscordData()
    {
        UnityWebRequest www = UnityWebRequest.Get(baseURL + "players/" + discordId + "/all");
        www.SetRequestHeader("Authorization", "Bearer " + token2);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        } else
        {
            UserDiscordData playerData = JsonUtility.FromJson<UserDiscordData>(www.downloadHandler.text);
            playerID = playerData.player_id.ToString();

            //discord username lenght
            if (playerData.discord_username.Length >= 16)
            {
                discordNameLeader = playerData.discord_username.Substring(0, 16);
            }
            else
            {
                discordNameLeader = playerData.discord_username;
            }

            /**
            //discord username lenght
            if (playerData.discord_username.Length >= 10)
            {
                discordName.text = playerData.discord_username.Substring(0, 10);
            }
            else
            {
                discordName.text = playerData.discord_username;
            }
            **/
            usernameText.ChangeUsername();
            imageLink = playerData.discord_avatar_url;
            StartCoroutine(GrabWebImage());
        }
        www.Dispose();
    }


    public IEnumerator GrabGameData()
    {
        UnityWebRequest uwr = UnityWebRequest.Get(baseURL + "players/" + discordId + "/flappy?game=" +6);
        uwr.SetRequestHeader("Authorization", "Bearer " + token2);

        yield return uwr.SendWebRequest();
        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            UserGameData userData = JsonUtility.FromJson<UserGameData>(uwr.downloadHandler.text);
            highScore = userData.high_score;
            coinAllColected = userData.total_coins;
            userCoins = GameObject.FindGameObjectWithTag("coins").GetComponent<ReloadCoins>();
            userCoins.ChangeCoins();
            //CoinAllCollectedText.text = userData.total_coins.ToString(); //Commit to text
        }
        uwr.Dispose();
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
        pfpImage = GameObject.FindGameObjectWithTag("profilepic").GetComponent<ReloadPfp>();
        pfpImage.ChangeImage();
    }
}
