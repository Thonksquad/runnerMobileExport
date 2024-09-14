using System.Collections;
using UnityServiceLocator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHandler : MonoBehaviour
{
    public static bool bossAlive;
    public static int bossCurrentHP = 100;
    public static int bossMaxHP = 100;
    public static int bossCoins = 0;
    private GameObject currentBoss;
    [SerializeField] private Color _orangeClr;

    [SerializeField] private float _stopEnemySpawns = 1300f;
    [SerializeField] private float _bossSpawnWarningPosition = 1400f;
    [SerializeField] private float _bossSpawnPosition = 1500f;
    [SerializeField] private GameObject _warningImage;
    [SerializeField] private GameObject _killScreen;
    [SerializeField] private float _killScreenTime = 5f;

    [Header("Spawner Stats")]
    [SerializeField] BossNames bossToSpawn;
    [SerializeField] float spawnTimer;
    [Header("Boss References")]
    [SerializeField] private GameObject wallOfFleshBoss;
    [Header("References")]
    [SerializeField] private Canvas bossUICanvas;
    [SerializeField] private TextMeshProUGUI bossHPtext;
    [SerializeField] private Image bossHPBar;




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
        bossUICanvas.gameObject.SetActive(true);
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

        StartCoroutine(BossKillScreen());
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


    private IEnumerator BossKillScreen()
    {
        BossCoinCalculator(); 
        _killScreen.SetActive(true);
        yield return new WaitForSeconds(_killScreenTime);
        _killScreen.SetActive(false);
        _spawnCooldown.EndBoss();
    }

    private void BossCoinCalculator()
    {
        int hits = Player.bossHit;
        if (hits <= 1)
        {
            bossCoins = 5;
        }
        else if (hits <= 4)
        {
            bossCoins = 3;
        }
        else
        {
            bossCoins = 1;
        }
        _gameManager.bossCoinUI.text = bossCoins.ToString();
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
        BossHandler scrRef = FindObjectOfType<BossHandler>();
        bossCurrentHP -= amount;
        scrRef.UpdateBossUI();
    }

}
