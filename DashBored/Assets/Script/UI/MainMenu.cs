using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Unity.Services.Leaderboards;
using Newtonsoft.Json;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour, IPointerDownHandler
{

    private Player player;
    const string leaderboardId = "leaderboard";


    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        addScore();
        ActionSystem.onPlayerRevive();
        SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Single);
        player.gameOver = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
    }


    public async void addScore()
    {
        var metadata = new Dictionary<string, string>() {
            { "gameLength", GameManager.gameLength.ToString() } ,
            { "enemiesKilled", GameManager.enemiesKilled.ToString() },
            { "speed", CameraManager.Instance.CamSpeed.ToString() }
        };
        var playerEntry = await LeaderboardsService.Instance
            .AddPlayerScoreAsync(leaderboardId, GameManager.distance,
            new AddPlayerScoreOptions { Metadata = metadata }
            );
        Debug.Log(JsonConvert.SerializeObject(playerEntry));
    }


}
