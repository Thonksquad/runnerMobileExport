using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Unity.Services.Leaderboards;
using Newtonsoft.Json;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour, IPointerDownHandler
{

    private Player player;


    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
        ActionSystem.onPlayerRecover();
        SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Single);
        player.gameOver = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
    }


    


}
