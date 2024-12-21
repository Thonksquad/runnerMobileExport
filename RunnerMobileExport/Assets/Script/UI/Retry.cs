using UnityEngine;
using UnityEngine.EventSystems; 
using UnityEngine.SceneManagement; 
using UnityServiceLocator;

public class Retry : MonoBehaviour, IPointerDownHandler
{

    private Player _player;
    private GameManager _gameManager;


    private void Start()
    {
        ServiceLocator.ForSceneOf(this).Get(out _player);
        ServiceLocator.ForSceneOf(this).Get(out _gameManager);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        _player.gameOver = false; 
        _gameManager.ChangeState(GameState.ArcadeMode);
        _gameManager.playerRecover();
        GameManager.distance = 0;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync(currentSceneName, LoadSceneMode.Single);
    }
}
