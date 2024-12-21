using System.Collections;
using UnityServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Exceptions;
using UnityEngine.SocialPlatforms.Impl;
using Unity.Services.Core;

public class BossHandler : MonoBehaviour
{
    public static event Action OnBossComplete;
    public static bool bossAlive;
    public static int bossCurrentHP = 100;
    public static int bossMaxHP = 100;
    public static int bossCoins = 0;
    private GameObject currentBoss;
    [SerializeField] private Color _orangeClr;
    [SerializeField] private float _stopEnemySpawns;
    [SerializeField] private float _bossSpawnWarningPosition;
    [SerializeField] private float _bossSpawnPosition;
    [SerializeField] private GameObject _warningImage;
    [SerializeField] private GameObject _killScreen;
    [SerializeField] private AudioClip _bossMusic;
    [SerializeField] private AudioClip _bossCry;

    [Header("Spawner Stats")]
    [SerializeField] BossNames bossToSpawn;
    [SerializeField] float spawnTimer;
    [Header("Boss References")]
    [SerializeField] private GameObject wallOfFleshBoss;
    [Header("References")]
    [SerializeField] private Canvas bossUICanvas;
    [SerializeField] private TextMeshProUGUI bossHPtext;
    [SerializeField] private Image bossHPBar;
    [SerializeField] private TextMeshProUGUI bossDistance;
    [SerializeField] private TextMeshProUGUI bestDistance;

    private SoundManager _soundManager;


    private Player _player;
    private UnitManager _unitManager;
    private SpawnCooldown _spawnCooldown;
    private GameManager _gameManager;


    private enum BossNames
    {
        WallOfFlesh
    }

    private void Start()
    {
        ServiceLocator.ForSceneOf(this).Get(out _player);
        ServiceLocator.ForSceneOf(this).Get(out _unitManager);
        ServiceLocator.ForSceneOf(this).Get(out _spawnCooldown);
        ServiceLocator.ForSceneOf(this).Get(out _gameManager);
        ServiceLocator.ForSceneOf(this).Get(out _soundManager);

        bossUICanvas.gameObject.SetActive(false);
        bossAlive = false;
        StartCoroutine(WaitForStopEnemySpawn());
        StartCoroutine(WaitForSpawnWarning()); 
        StartCoroutine(WaitForSpawn());
    }


    #region Waitings
    private IEnumerator WaitForStopEnemySpawn()
    {
        bool wait = true;
        while (wait)
        {
            if (GameManager.distance > _stopEnemySpawns)
            {
                wait = false;
            }
            yield return new WaitForSeconds(1f);
        }

        _spawnCooldown.StartBoss(); 
        
    }


    private IEnumerator WaitForSpawnWarning()
    {
        bool wait = true;
        while (wait)
        {
            if (GameManager.distance > _bossSpawnWarningPosition)
            {
                wait = false;
            }
            yield return new WaitForSeconds(1f);
        }

        if (!_player.onHound)
        {
            _unitManager.SpawnHound();
        }

        _warningImage.SetActive(true);
        StartCoroutine(MusicTransition(4));
        StartCoroutine(WarningFadeIn());
        
        bossUICanvas.gameObject.SetActive(true);
    }

    private IEnumerator MusicTransition(float fadeDuration)
    {
        // Fade out
        float timer = 0f;
        float startVolume = 1f;
        float targetVolume = 0f;

        while (timer < fadeDuration)
        {
            _soundManager.ChangeMusicVolume(Mathf.Lerp(startVolume, targetVolume, timer / fadeDuration));
            timer += Time.deltaTime;
            yield return null;
        }

        _soundManager.ChangeMusicVolume(targetVolume);

        _soundManager.PlayMusic(_bossMusic);
        yield return new WaitForSeconds(0.2f);

        // Fade in
        timer = 0f;
        startVolume = 0f;
        targetVolume = 1f;

        while (timer < fadeDuration)
        {
            _soundManager.ChangeMusicVolume(Mathf.Lerp(startVolume, targetVolume, timer / fadeDuration));
            timer += Time.deltaTime;
            yield return null;
        }

        _soundManager.ChangeMusicVolume(targetVolume);
        _soundManager.PlaySound(_bossCry);
    }


    private IEnumerator WarningFadeIn()
    {
        Image image = _warningImage.GetComponent<Image>();
        Color color = image.color;
        float alpha = 0f;

        while (alpha < 1f)
        {
            alpha += Time.deltaTime / 2f;
            color.a = alpha;
            image.color = color;
            yield return null;
        }
    }

    private IEnumerator WaitForSpawn()
    {
        bool wait = true;
        while (wait)
        {
            if (GameManager.distance > _bossSpawnPosition)
            {
                wait = false;
            }
            yield return new WaitForSeconds(1f);
        }
        _warningImage.SetActive(false);

        // Spawn
        EnableBossUI();
        SpawnBoss();
    }

    private IEnumerator WaitForBossToDie()
    {
        while (currentBoss.activeInHierarchy)
            yield return null;

        bossDistance.text = Mathf.RoundToInt(GameManager.distance).ToString();

        BossKillScreen();
        EndBossEncounter();
    }
    #endregion // Waitings

    private void SpawnBoss()
    {
        bossAlive = true;

        switch (bossToSpawn)
        {
            case BossNames.WallOfFlesh:
                wallOfFleshBoss.SetActive(true);
                currentBoss = wallOfFleshBoss;
                break;
        }

        UpdateBossUI();
        StartCoroutine(WaitForBossToDie());
    }


    private async void BossKillScreen()
    {
        try
        {
            var _leaderboardResponse = await LeaderboardsService.Instance.GetPlayerScoreAsync("bossmode");
            if (_leaderboardResponse != null)
            {
                bestDistance.text = _leaderboardResponse.Score.ToString();
            }
            else
            {
                Debug.Log("Score doesn't exist");
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        _killScreen.SetActive(true);
        OnBossComplete?.Invoke();
    }

    internal void EndBossEncounter()
    {
        bossAlive = false;
        DisableBossUI();
    }

    private void EnableBossUI()
    {
        bossUICanvas.enabled = true;
    }

    private void DisableBossUI()
    {
        bossUICanvas.enabled = false;
    }

    private void UpdateBossUI()
    {
        Color tempC = bossHPBar.color;

        float hpCompute = (float)bossCurrentHP / (float)bossMaxHP;

        bossHPtext.text = "" + hpCompute * 100 + "%";
        //bossHPBar.transform.localScale = new Vector3(hpCompute, 1, 1);
        bossHPBar.fillAmount = hpCompute;

        if( hpCompute > 0.75f)
        {
            bossHPBar.color = (tempC * hpCompute) + (Color.green * (1.0f - hpCompute));
        }
        else if( hpCompute > 0.33f )
        {
            bossHPBar.color = (tempC * hpCompute) + (_orangeClr * (1.0f - hpCompute));
        }
        else
        {
            bossHPBar.color = (tempC * hpCompute) + (Color.red * (1.0f - hpCompute));
        }
        
    }

    public static void bossTakeDamage(int amount)
    {
        BossHandler scrRef = FindObjectOfType<BossHandler>(); //not optimal
        bossCurrentHP -= amount;
        scrRef.UpdateBossUI();
    }

}
