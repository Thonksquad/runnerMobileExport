using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHandler : MonoBehaviour
{
    public static bool bossAlive;
    public static int bossCurrentHP = 100;
    public static int bossMaxHP = 100;
    private GameObject currentBoss;

    [SerializeField] private float _bossSpawnWarningPosition = 400f;
    [SerializeField] private float _bossSpawnPosition = 500f;
    [SerializeField] private GameObject _warningImage;

    [Header("Spawner Stats")]
    [SerializeField] BossNames bossToSpawn;
    [SerializeField] float spawnTimer;
    [Header("Boss References")]
    [SerializeField] private GameObject wallOfFleshBoss;
    [Header("References")]
    [SerializeField] private Canvas bossUICanvas;
    [SerializeField] private TextMeshProUGUI bossHPtext;
    [SerializeField] private Image bossHPBar;

    private enum BossNames
    {
        WallOfFlesh
    }

    private void Start()
    {
        bossAlive = false;
        StartCoroutine(WaitForSpawnWarning());
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
            yield return new WaitForSeconds(3f);
        }


        if (!Player.Instance.onHound)
        {
            UnitManager.Instance.SpawnHound();
        }
        SpawnCooldown.Instance.StartBoss();
        _warningImage.SetActive(true);
        StartCoroutine(WaitForSpawn());
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
            yield return new WaitForSeconds(3f);
        }
        _warningImage.SetActive(false);

        // Spawn
        EnableBossUI();
        SpawnBoss();
    }


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

    private IEnumerator WaitForBossToDie()
    {
        while (currentBoss.activeInHierarchy)
            yield return null;

        EndBossEncounter();
        SpawnCooldown.Instance.EndBoss();
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
