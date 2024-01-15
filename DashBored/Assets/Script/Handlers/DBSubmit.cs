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

public class DBSubmit : SingletonPersistent<DBSubmit>
{
    public static string baseURL = "https://ecmgqf1op7.execute-api.us-east-1.amazonaws.com/";
    public static string token2 = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJnYW1lIiwiZXhwIjoxNjkzOTg2MjI1fQ.crB0vdpfT8LJNXBk432ilXDZi9fTP0MnPhM4eW4KLtA";
    public static int counterSendScoreOneTime = 0; //Counter increases when score is sent

    private string plainText = "Working";
    private static readonly byte[] key = Encoding.UTF8.GetBytes("NewPassword12311NewPassword12311");
    private static readonly byte[] iv = Encoding.UTF8.GetBytes("Tsalt1wtQt9kHuHg");



    private void OnEnable()
    {
        ActionSystem.onPlayerDeath += sendToDatabase;
    }

    private void OnDisable()
    {
        ActionSystem.onPlayerDeath -= sendToDatabase;
    }

    public void sendToDatabase()
    {
        //StartCoroutine(SendCoin());
        //StartCoroutine(SendScore());
        //StartCoroutine(SendAdditionalData());
    }

    private void Start()
    {
        string encrypted = Encrypt(plainText);
        //Debug.Log(encrypted);
        Decrypt(encrypted);
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

        //Debug.Log(plaintext);
        return plaintext;
    }

[System.Serializable]
    public class UserData
    {
        public string discord_username;
        public string discord_avatar_url;
        public int total_coins;
    }

    public IEnumerator SendCoin()
    {
        int coinRoll = GameManager.coins * 174;
        string coinEnc = Encrypt(coinRoll.ToString());
        string coinDec = Decrypt(coinEnc.ToString());
        // Debug.Log(coinEnc);
        // Debug.Log(coinDec + " Decript");
        counterSendScoreOneTime++;
        //yield return new WaitForSecondsRealtime(1f);


        UnityWebRequest uwr = UnityWebRequest.Put(baseURL + "coins/add?player_id=" + DBGrabUser.discordId + "&coins=" + Uri.EscapeDataString(coinEnc) + "&game=6", "placeholder");
        uwr.SetRequestHeader("Authorization", "Bearer " + token2);

        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            //Debug.Log("Coin uploaded successfully");
        }
        uwr.Dispose();
    }

    [System.Serializable]
    public class DataScore
    {
        public string discord_id;
        public int game_id;
        public string score;
    }

    [System.Serializable]
    public class AdditionalDataScore
    {
        public string discord_id;
        public int game_id;
        public string _distanceTravelled;
        public string _gameLength;
        public string _enemiesKilled;
        public string _endPlayerSpeed;
    }


    public IEnumerator SendScore()
    {
        int scoreRoll = Mathf.RoundToInt(GameManager.distance) * 174;
        string scoreEnc = Encrypt(scoreRoll.ToString());
        counterSendScoreOneTime++;
        //yield return new WaitForSecondsRealtime(1f);

        DataScore data = new DataScore();
        data.discord_id = DBGrabUser.discordId.ToString();
        data.game_id = 6;
        data.score = scoreEnc;

        //leaderboards.AddScore(scoreRoll);

        // Serialize the data to JSON format
        string jsonData = JsonUtility.ToJson(data);
        //Debug.Log(jsonData);
        // Create a new UnityWebRequest object with a PUT method
        UnityWebRequest request = UnityWebRequest.Put(baseURL + "scores", jsonData);
        // Set the content type header to "application/json"
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token2);

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            //Debug.Log("Score sent successfully!");
        }

        request.Dispose();
    }

    private IEnumerator SendAdditionalData()
    {
        //Encrypting the scores
        string scoreEnc = Encrypt((Mathf.RoundToInt(GameManager.distance) * 174).ToString());
        string timeEnc = Encrypt((Mathf.RoundToInt(GameManager.gameLength) * 174).ToString());
        string enemiesEnc = Encrypt((Mathf.RoundToInt(GameManager.enemiesKilled * 174)).ToString());
        string speedEnc = Encrypt((Mathf.RoundToInt(CameraManager.Instance.CamSpeed * 174)).ToString());

        // Create the JSON payload
        AdditionalDataScore data = new AdditionalDataScore();
        data._distanceTravelled = scoreEnc;
        data._gameLength = timeEnc;
        data._enemiesKilled = enemiesEnc;
        data._endPlayerSpeed = speedEnc;

        string jsonData = JsonUtility.ToJson(data);

        // Create the request object
        UnityWebRequest request = UnityWebRequest.Post(baseURL + "games/6/players/" + DBGrabUser.playerID + "/data", "");

        // Set the request headers
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token2);

        /**
        string jsonPayload = "{\n" +
            "\"distance_travelled\": " + scoreEnc + "," +
            "\"game_length\": " + timeEnc + "," +
            "\"enemies_killed\": " + enemiesEnc + "," +
            "\"player_speed_at_death\": " + speedEnc +
            "}";
        **/

        // double {{ is how you "escape" the string interpolation to print out a single "{"
        string jsonPayload = $"{{\n\"distance_travelled\": \"{scoreEnc}\",\n" +
        $"\"game_length\": \"{timeEnc}\",\n" +
        $"\"enemies_killed\": \"{enemiesEnc}\",\n" +
        $"\"player_speed_at_death\": \"{speedEnc}\"\n}}";

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        // Set the request body
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);

        // Send the request and wait for the response
        yield return request.SendWebRequest();

        // Check for any errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error sending request: " + request.error);
        }
        else
        {
            //Debug.Log("Additional data sent successfully!");
        }

        request.Dispose();
     }
    
}
