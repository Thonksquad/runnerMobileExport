using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClaimBadge : MonoBehaviour
{
    [SerializeField] Button ClaimButton;
    [SerializeField] GameObject ClaimNotification;
    [SerializeField] TextMeshProUGUI claimText;
    private string communityID;
    private string badgeID;
    private string apiKey;

    private Coroutine ClaimBadgeQuery;

    [System.Serializable]
    public class RequestData
    {
        public string wallet;
    }

    public void ClaimBossBadge()
    {
        //Application.OpenURL("https://other.page/?=" + LoginDataSingleton.ugsPlayerID);
        ClaimButton.interactable = false;
        ColorBlock cb = ClaimButton.colors;
        cb.disabledColor = Color.gray;
        ClaimButton.colors = cb;
        ClaimNotification.SetActive(true);
        claimText.text = "Querying database";
        communityID = "8ff68bd4-87cf-4cfd-b25e-40467d3a22e5";
        badgeID = "1adc1418-858c-400e-b277-69630eebba20";
        apiKey = Secret.API;
        ClaimBadgeQuery = StartCoroutine(SendPostRequest(communityID, badgeID));
    }

    private IEnumerator SendPostRequest(string communityID, string badgeId)
    {
        string apiUrl = $"https://api.other.page/v1/community/{communityID}/badge/{badgeId}/attribution";
        RequestData requestData = new RequestData { wallet = LoginDataSingleton.WalletAddress };
        string jsonData = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-Api-Key", apiKey);

            // Send the request and wait for a response
            yield return request.SendWebRequest();

            // Check for errors
            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}");
                claimText.text = "Error connecting to the database.";
            }
            else
            {
                Debug.Log($"Response: {request.downloadHandler.text}");
                long responseCode = request.responseCode;

                if (responseCode == 201)
                {
                    Debug.Log($"Success: {request.downloadHandler.text}");
                    claimText.text = "Badge successfully claimed!";
                }
                else if (responseCode == 400)
                {
                    Debug.Log($"Bad Request: {request.downloadHandler.text}");
                    claimText.text = "You have already claimed this badge.";
                }
                else
                {
                    Debug.LogError($"Error: {request.error} with response code: {responseCode}");
                    claimText.text = "An error occurred.";
                }
            }
        }
    }
}
