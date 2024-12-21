using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageLoad : MonoBehaviour
{
    [SerializeField] private Image Discordpfp;
    private string ProfilePicture = "https://wallpapers.com/images/featured/40lkhq7b7tl3p1qw.jpg";

    void Start()
    {
        StartCoroutine(LoadImage(ProfilePicture));
    }

    IEnumerator LoadImage(string uri)
    {
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(uri);
        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Texture2D myTexture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
            Sprite newSprite = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f));
            Discordpfp.sprite = newSprite;
        }
        else
        {
            Debug.LogError(webRequest.error);
        }
    }
}
