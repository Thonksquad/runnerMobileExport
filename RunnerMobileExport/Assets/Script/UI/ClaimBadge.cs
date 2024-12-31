using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClaimBadge : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Image ClaimBackground;
    [SerializeField] Sprite ClaimImage;
    private string communityID;
    private string badgeID;
    private string apiKey;

    private Coroutine ClaimBadgeQuery;

    private void OnEnable()
    {
        BossHandler.OnBossComplete += ClaimBossBadge;
    }

    private void OnDisable()
    {
        BossHandler.OnBossComplete -= ClaimBossBadge;
    }

    [System.Serializable]
    public class RequestData
    {
        public string wallet;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Application.OpenURL("https://other.page/badges/1adc1418-858c-400e-b277-69630eebba20");
    }

    public void ClaimBossBadge()
    {
        communityID = "8ff68bd4-87cf-4cfd-b25e-40467d3a22e5";
        badgeID = "1adc1418-858c-400e-b277-69630eebba20";
        apiKey = Secret.API;
        ClaimBadgeQuery = StartCoroutine(SendPostRequest(communityID, badgeID));
    }

    private IEnumerator SendPostRequest(string communityID, string badgeId)
    {
        string apiUrl = $"https://api.other.page/v1/community/{communityID}/badge/{badgeId}/attribution?autoClaim=false";
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

                if (request.responseCode == 400)
                {
                    //Badge already claimed
                }
                else
                {
                    //Connection error
                }
            }
            else
            {
                Debug.Log($"Response: {request.downloadHandler.text}");
                if (request.responseCode == 201)
                {
                    //Successful API request
                    Debug.Log($"Success: {request.downloadHandler.text}");
                    //ClaimBackground.sprite = ClaimImage;
                }
                else
                {
                    //Error occurred
                    Debug.LogError($"Error: {request.error} with response code: {request.responseCode}");
                }
            }
        }
    }
}
