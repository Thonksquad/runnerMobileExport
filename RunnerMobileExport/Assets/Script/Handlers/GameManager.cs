using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Newtonsoft.Json;
using Unity.Services.Leaderboards;


public class GameManager : MonoBehaviour
{
    [SerializeField] private float HoundModifier;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gamePlayScreen;
    [SerializeField] private AudioClip _deathSound;
    public static GameManager Instance;
    public GameState GameState;
    public TextMeshProUGUI BestdistanceUI;
    public TextMeshProUGUI EnddistanceUI;
    public TextMeshProUGUI EndcoinsUI;
    public TextMeshProUGUI distanceUI;
    public TextMeshProUGUI coinUI;

    // Refactor this so that it grabs from UGS's DB
    //private bool newHighScore() => distance > DBGrabUser.highScore;
    public static float gameLength;
    public static float enemiesKilled;
    public static float distance;
    public static int coins = 0;
    private int hounds = 1;
    private Coroutine coUpdateTimer;


    const string leaderboardId = "leaderboard";


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            if(Instance != this)
            {
                Destroy(this);
            }
        }
    }

    private void OnEnable()
    {
        ActionSystem.onPlayerHit += playerHit;
        ActionSystem.onPlayerRecover += playerRecover;
        ActionSystem.onEnemyDeath += enemyKilled;

    }

    private void OnDisable()
    {
        ActionSystem.onPlayerHit -= playerHit;
        ActionSystem.onPlayerRecover -= playerRecover;
        ActionSystem.onEnemyDeath -= enemyKilled;
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

    public void playerRecover()
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
        //BestdistanceUI.text = DBGrabUser.highScore.ToString();
        EndcoinsUI.text = coins.ToString();
        addScore();

        if ( adsManager.Instance.hasVideoChance)
        {
            adsManager.Instance.showVideo();
        }
        else
        {
            /*
            if (newHighScore())
            {
                DBGrabUser.highScore = (int)Mathf.Round(distance);
            }
            */
            gameOverScreen.SetActive(true);
        }
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


    private IEnumerator UpdateTimer()
    {
        while (GameState == GameState.ArcadeMode)
        {
            gameLength += Time.deltaTime;
            distance = Mathf.Round(gameLength * CameraManager.Instance.CamSpeed);
            distanceUI.text = (distance.ToString() + "m");

            if (distance/(500 + ((hounds-1)*HoundModifier)) > hounds)
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