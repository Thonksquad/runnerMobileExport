using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour, IPointerDownHandler
{

    private Player player;
    LeaderboardClass leaderboards = new LeaderboardClass();

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        leaderboards.AddScore(GameManager.distance);
        ActionSystem.onPlayerRevive();
        SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Single);
        player.gameOver = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
    }
}
