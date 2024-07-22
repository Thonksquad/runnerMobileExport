using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Retry : MonoBehaviour, IPointerDownHandler
{
    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        player.gameOver = false;
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single);
        ActionSystem.onPlayerRestart();
    }
}
