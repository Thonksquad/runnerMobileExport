using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityServiceLocator;


public class GameManager : MonoBehaviour
{
    [SerializeField] private float HoundModifier;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject gamePlayScreen;
    [SerializeField] private AudioClip _deathSound;

    public GameState GameState;
    public TextMeshProUGUI BestdistanceUI;
    public TextMeshProUGUI EnddistanceUI;
    public TextMeshProUGUI EndcoinsUI;
    public TextMeshProUGUI distanceUI;
    public TextMeshProUGUI coinUI; 
    public TextMeshProUGUI bossCoinUI;

    // Refactor this so that it grabs from UGS's DB
    //private bool newHighScore() => distance > DBGrabUser.highScore;
    public static float gameLength;
    public static float enemiesKilled;
    public static float distance;
    public static int coins = 0;
    private int hounds = 1;
    private Coroutine coUpdateTimer;

    private Player _player;
    private UnitManager _unitManager;
    private SoundManager _soundManager;
    private adsManager _adsManager;

    const string leaderboardId = "leaderboard";


    private void Awake()
    {
        ServiceLocator.ForSceneOf(this).Register<GameManager>(this); // Scene Scope
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
        ServiceLocator.ForSceneOf(this).Get(out _player);
        ServiceLocator.ForSceneOf(this).Get(out _unitManager);
        ServiceLocator.ForSceneOf(this).Get(out _soundManager);
        ServiceLocator.ForSceneOf(this).Get(out _adsManager);
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
        _soundManager.TurnMusicOn();
    }

    public void playerHit()
    {
        Time.timeScale = 0;
        _soundManager.PlaySound(_deathSound);
        _soundManager.ToggleMusic();
        EnddistanceUI.text = Mathf.Round(distance).ToString();
        EndcoinsUI.text = coins.ToString();
        addScore();

        /*
        if (_adsManager.hasVideoChance)
        {
            _adsManager.showVideo();
        }
        else
        {
            if (newHighScore())
            {
                DBGrabUser.highScore = (int)Mathf.Round(distance);
            }
            gameOverScreen.SetActive(true);
        } */
        gameOverScreen.SetActive(true);

    }

    public async void addScore()
    {
        var metadata = new Dictionary<string, string>() {
            { "gameLength", GameManager.gameLength.ToString() } ,
            { "enemiesKilled", GameManager.enemiesKilled.ToString() },
            { "speed", _player.speed.ToString() }
        };

        
        
    }


    private IEnumerator UpdateTimer()
    {
        while (GameState == GameState.ArcadeMode)
        {
            gameLength += Time.deltaTime;
            distance = Mathf.Round(gameLength * _player.speed);
            distanceUI.text = (distance.ToString() + "m");

            if (distance/(500 + ((hounds-1)*HoundModifier)) > hounds)
            {
                _unitManager.SpawnHound();
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