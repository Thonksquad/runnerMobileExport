using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

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
    private bool newHighScore() => distance > DBGrabUser.highScore;
    public static float gameLength;
    public static float enemiesKilled;
    public static float distance;
    public static int coins = 0;
    private int hounds = 1;
    private Coroutine coUpdateTimer;


    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        ActionSystem.onPlayerDeath += playerDeath;
        ActionSystem.onPlayerRevive += playerRevive;
        ActionSystem.onEnemyDeath += enemyKilled;

    }

    private void OnDisable()
    {
        ActionSystem.onPlayerDeath -= playerDeath;
        ActionSystem.onPlayerRevive -= playerRevive;
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

    public void playerRevive()
    {
        enemiesKilled = 0;
        gameLength = 0;
        coins = 0;
        Time.timeScale = 1;
        gameOverScreen.SetActive(false);
        AudioListener.pause = false;
        SoundManager.Instance.TurnMusicOn();
    }

    public void playerDeath()
    {
        if( adsManager.Instance.hasVideoChance)
        {
            adsManager.Instance.showVideo();
        }
        else
        {
            if (newHighScore())
            {
                DBGrabUser.highScore = (int)Mathf.Round(distance);
            }
            Time.timeScale = 0;
            SoundManager.Instance.PlaySound(_deathSound);
            gameOverScreen.SetActive(true);
            SoundManager.Instance.ToggleMusic();
            EnddistanceUI.text = Mathf.Round(distance).ToString();
            BestdistanceUI.text = DBGrabUser.highScore.ToString();
            EndcoinsUI.text = coins.ToString();
        }
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