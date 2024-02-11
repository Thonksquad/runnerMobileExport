using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour, IPointerDownHandler
{

    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
        player.gameOver = false;
        Time.timeScale = 1;
        AudioListener.pause = false;
    }
}
