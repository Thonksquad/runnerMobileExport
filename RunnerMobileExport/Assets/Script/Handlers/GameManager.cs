using System.Collections;
using UnityEngine;
using System;
using TMPro;
using Unity.Services.Leaderboards;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float HoundModifier;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gamePlayScreen;
    [SerializeField] private AudioClip _deathSound;
    public static GameManager Instance;
    public static GameState GameState;
    public TextMeshProUGUI BestdistanceUI;
    public TextMeshProUGUI EnddistanceUI;
    public TextMeshProUGUI EndcoinsUI;
    public TextMeshProUGUI distanceUI;
    public TextMeshProUGUI coinUI;

    public static float gameLength;
    public static float enemiesKilled;
    public static float distance;
    public static int coins = 0;
    private int hounds = 1;
    private Coroutine coUpdateTimer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(this);
            }
        }
    }

    private void OnEnable()
    {
        ActionSystem.onPlayerHit += playerHit;
        ActionSystem.onPlayerRestart += playerRestart;
        ActionSystem.onEnemyDeath += enemyKilled;
        ActionSystem.onUpdateCoins += coinToDatabase;
    }

    private void OnDisable()
    {
        ActionSystem.onPlayerHit -= playerHit;
        ActionSystem.onPlayerRestart -= playerRestart;
        ActionSystem.onEnemyDeath -= enemyKilled;
        ActionSystem.onUpdateCoins -= coinToDatabase;
    }

    void Start()
    {
        ChangeState(GameState.ArcadeMode);
    }

    public void IncreaseCoin(int amt)
    {
        coins += amt;
        coinUI.text = coins.ToString();
    }

    public void ChangeState(GameState newState)
    {
        GameState = newState;
        switch (newState)
        {
            case GameState.StartScreen:
                break;
            case GameState.ArcadeMode:
                gameLength = 0;
                enemiesKilled = 0;
                distance = 0;
                coins = 0;
                hounds = 1;
                coUpdateTimer = StartCoroutine(UpdateTimer());
                break;
            case GameState.GamePause:
                StopCoroutine(coUpdateTimer);
                break;
            case GameState.GameOver:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    public void enemyKilled(BaseEnemy enemy)
    {
        enemiesKilled++;
    }

    public void playerRestart()
    {
        enemiesKilled = 0;
        gameLength = 0;
        coins = 0;
        Time.timeScale = 1;
        gameOverScreen.SetActive(false);
        AudioListener.pause = false;
        SoundManager.Instance.TurnMusicOn();
    }

    public void playerHit()
    {
        Time.timeScale = 0;
        SoundManager.Instance.PlaySound(_deathSound);
        SoundManager.Instance.ToggleMusic();
        EnddistanceUI.text = Mathf.Round(distance).ToString();
        EndcoinsUI.text = coins.ToString();
        UploadHandler.Instance.addScore();

        if (adsManager.Instance.hasVideoChance)
        {
            gameOverHandler.Instance.loadStatsVc();
            adsManager.Instance.showVideo();
        }
        else
        {
            coinToDatabase();
            gameOverHandler.Instance.loadStatsGo();
            gameOverScreen.SetActive(true);
        }
    }

    public void coinToDatabase()
    {
        UploadHandler.Instance.addCoins();
    }

    private IEnumerator UpdateTimer()
    {
        while (GameState == GameState.ArcadeMode)
        {
            gameLength += Time.deltaTime;
            distance = Mathf.Round(gameLength * CameraManager.Instance.CamSpeed);
            distanceUI.text = (distance.ToString() + "m");

            if (distance / (500 + ((hounds - 1) * HoundModifier)) > hounds)
            {
                UnitManager.Instance.SpawnHound();
                hounds++;
            }
            coinUI.text = (coins.ToString());

            yield return null;
        }
    }
}

public enum GameState
{
    StartScreen = 0,
    ArcadeMode = 1,
    GamePause = 2,
    GameOver = 3
}