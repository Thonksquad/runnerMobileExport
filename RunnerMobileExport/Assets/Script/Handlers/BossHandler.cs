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


    private Player player;

    private enum BossNames
    {
        WallOfFlesh
    }

    private void Start()
    {
        ServiceLocator.ForSceneOf(this).Get(out player);
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
         
        SpawnCooldown.Instance.StartBoss(); 
        
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


        if (!player.onHound)
        {
            UnitManager.Instance.SpawnHound();
        }

        _warningImage.SetActive(true);
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
        SpawnCooldown.Instance.EndBoss();
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
        GameManager.Instance.bossCoinUI.text = bossCoins.ToString();
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
        bossHPBar.transform.localScale = new Vector3(hpCompute, 1, 1);
        bossHPBar.color = (tempC * hpCompute) + (Color.red * (1.0f - hpCompute));
    }

    public static void bossTakeDamage(int amount)
    {
        BossHandler scrRef = FindObjectOfType<BossHandler>();
        bossCurrentHP -= amount;
        scrRef.UpdateBossUI();
    }

}
