using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WaitForTime;
using Random = UnityEngine.Random;

public class Boss1 : MonoBehaviour
{
    public int health;
    [SerializeField] private int phase;
    private float homingBulletVelocity = 8;
    private ObjectPool homingPool = new ObjectPool();
    private Vector3 initialLocalPosition;
    private IEnumerator phaseBehaviorCoroutine;
    internal BossShake shakeScript;
    [SerializeField] internal List<GameObject> _closedBossEyes = new List<GameObject>();

    [SerializeField] private int eyeHitCount;
    [SerializeField] private Vector2 eyeOpenDelayMINMAX;
    [Header("References")]
    [SerializeField] private Transform playerTransformReference;
    [SerializeField] private GameObject bulletHoming;
    [SerializeField] private GameObject bulletBouncing;
    [SerializeField] private List<BossEye> existingBossEyes;
    [SerializeField] private GameObject finalPhaseMeleeEye;
    [Header("Types Of Eyes")]
    [SerializeField] internal List<EyeEntries> _Eyes;
    [Serializable] public class EyeEntries { public EyeTypesEnum eyeType; public Sprite pupilSprite; public float reloadTime; public Color eyeColor;}

    public enum EyeTypesEnum
    {
        NORMAL,
        TRIPPLE,
        LASER
    }

    private void Awake()
    {
        homingPool.CreateObjectPool(bulletHoming, 3);
        shakeScript = GetComponent<BossShake>();
    }

    private void OnEnable()
    {
        BossHandler.bossCurrentHP = health;
        initialLocalPosition = transform.localPosition;
        transform.localPosition = new Vector3(initialLocalPosition.x + 30, initialLocalPosition.y , initialLocalPosition.z );
        StartCoroutine(Do_Intro());
    }


    private IEnumerator Do_Intro()
    {
        while (Mathf.Abs(initialLocalPosition.x - transform.localPosition.x) > 0.1f)
        {
            transform.localPosition = new Vector3( Mathf.Lerp(transform.localPosition.x, initialLocalPosition.x , 0.0035f) , transform.localPosition.y, transform.localPosition.z);
            yield return null;
        }

        transform.localPosition = initialLocalPosition;

        yield return new WaitForSeconds(0.25f);
        shakeScript.Do_shake(0.7f, 0.7f);
        yield return new WaitForSeconds(1.0f);
            
        switch (phase)
        {
            case 1:
                phaseBehaviorCoroutine = Phase1_ChooseEyeToOpen();
                StartCoroutine(phaseBehaviorCoroutine); // startBoss
                break;
            case 2:
                phaseBehaviorCoroutine = Phase2_ChooseEyeToOpen();
                StartCoroutine(phaseBehaviorCoroutine); // startBoss
                break;
        }
    }

    private IEnumerator Phase1_ChooseEyeToOpen()
    {
        float newT = Time.time;
        while (Time.time < newT + 0.5f)    // REPLACE WITH NEW TIMER SYSTEM
            yield return null;

        OpenRandomEye(0);

        while (BossHandler.bossCurrentHP > (float)BossHandler.bossMaxHP / 2)
        {
            while (_closedBossEyes.Count <= 0)
            {
                yield return null;
            }

            // yield return new WaitForSeconds(Random.Range(eyeOpenDelayMINMAX.x, eyeOpenDelayMINMAX.y));
            float newT2 = Time.time;    // REPLACE WITH NEW TIMER SYSTEM
            float newTimer = Random.Range(eyeOpenDelayMINMAX.x, eyeOpenDelayMINMAX.y);
            while (Time.time < newT2 + newTimer)
                yield return null;

            int phase1EyeType;

            if (Random.Range(0, 2) == 1)    //Spawn eye with either basic or 50% chance of tripple
                phase1EyeType = 1;
            else
                phase1EyeType = 0;

            if (BossHandler.bossCurrentHP > (float)BossHandler.bossMaxHP / 2)
                OpenRandomEye(phase1EyeType);
            yield return null;
        }

        // when HP reaches below 50% go phase 2 since phase 3 isnt available
        Do_Changephase(2);
    }

    private IEnumerator Phase2_ChooseEyeToOpen()
    {
        float newT = Time.time;    // REPLACE WITH NEW TIMER SYSTEM
        float newTimer = 1.75f;
        while (Time.time < newT + newTimer)
            yield return null;

        OpenRandomEye(Random.Range(0, 3));
     //   OpenRandomEye(Random.Range(0, 3));
        OpenRandomEye(2);

        //  while (BossHandler.bossCurrentHP > (float)BossHandler.bossMaxHP * 0.15f)
        while (BossHandler.bossCurrentHP > (float)BossHandler.bossMaxHP * 0.25)
        {
            while (_closedBossEyes.Count <= 0)
            {
                yield return null;
            }

            //   yield return new WaitForSeconds(Random.Range(eyeOpenDelayMINMAX.x, eyeOpenDelayMINMAX.y));
            float newT2 = Time.time;    // REPLACE WITH NEW TIMER SYSTEM
            float newTimer2 = Random.Range(eyeOpenDelayMINMAX.x, eyeOpenDelayMINMAX.y);
            while (Time.time < newT2 + newTimer2)
                yield return null;
            int phase2EyeType;

            phase2EyeType = Random.Range(0, 3);
            OpenRandomEye(phase2EyeType);

            if (_closedBossEyes.Count == 3) // IF ALL EYES ARE CLOSED OPEN ANOTHER EYE
            {
                phase2EyeType = Random.Range(0, 3);
                OpenRandomEye(phase2EyeType);
            }

            yield return null;
        }

        Do_Changephase(3);
    }
    private IEnumerator Phase3_ChooseEyeToOpen()
    {
        float newT = Time.time;    // REPLACE WITH NEW TIMER SYSTEM
        float newTimer = 1.5f;
        while (Time.time < newT + newTimer)
            yield return null;

        existingBossEyes[1].gameObject.SetActive(false);
        finalPhaseMeleeEye.SetActive(true);
        OpenRandomEye(Random.Range(0, 3));
        //   OpenRandomEye(Random.Range(0, 3));
        OpenRandomEye(2);

        //  while (BossHandler.bossCurrentHP > (float)BossHandler.bossMaxHP * 0.15f)
        while (BossHandler.bossCurrentHP > 0)
        {
            while (_closedBossEyes.Count <= 0)
            {
                yield return null;
            }

            //   yield return new WaitForSeconds(Random.Range(eyeOpenDelayMINMAX.x, eyeOpenDelayMINMAX.y));
            float newT2 = Time.time;    // REPLACE WITH NEW TIMER SYSTEM
            float newTimer2 = Random.Range(eyeOpenDelayMINMAX.x, eyeOpenDelayMINMAX.y);
            while (Time.time < newT2 + newTimer2)
                yield return null;
            int phase2EyeType;

            phase2EyeType = Random.Range(0, 3);
            OpenRandomEye(phase2EyeType);

            if (_closedBossEyes.Count == 3) // IF ALL EYES ARE CLOSED OPEN ANOTHER EYE
            {
                phase2EyeType = Random.Range(0, 3);
                OpenRandomEye(phase2EyeType);
            }

            yield return null;
        }

        StartCoroutine(Do_Death());
    }

    private IEnumerator Do_Death()
    {
        Vector3 origPos = transform.localPosition;
        finalPhaseMeleeEye.SetActive(false);
        foreach (BossEye eye in existingBossEyes)
        {
            eye.gameObject.SetActive(false);
        }

        shakeScript.Do_shake(1.2f, 3.0f);

        float newT2 = Time.time;    // REPLACE WITH NEW TIMER SYSTEM
        float newTimer2 = 3;
        while (Time.time < newT2 + newTimer2)
        {
            transform.localPosition = transform.localPosition + Vector3.right * Time.deltaTime * 4;
            yield return null;
        }

        Do_reset();
        gameObject.SetActive(false);
        transform.localPosition = origPos;
    }

    private void Do_reset()
    {
        phase = 1;
        foreach (BossEye eye in existingBossEyes)
        {
            eye.gameObject.SetActive(true);
        }
    }


    private void Do_Changephase(int phaseToChangeTo)
    {
        //StopCoroutine(phaseBehaviorCoroutine);
        StopAllCoroutines();
        CloseAllEyes(false);

        if (phaseToChangeTo == 2)
        {
            shakeScript.Do_shake(0.55f, 1.0f);
            phaseBehaviorCoroutine = Phase2_ChooseEyeToOpen();
            StartCoroutine(phaseBehaviorCoroutine); // startBoss}
        }
        else if (phaseToChangeTo == 3)
        {
            shakeScript.Do_shake(0.75f, 0.85f);
            phaseBehaviorCoroutine = Phase3_ChooseEyeToOpen();
            StartCoroutine(phaseBehaviorCoroutine); // startBoss}
        }
    }

    private void CloseAllEyes(bool doDamage)
    {
        foreach (BossEye b in existingBossEyes)
        {
            if (b.canBeHit)
                b.DoEyeClose(doDamage);
        }
    }

    private void OpenRandomEye(int whatEyeType)
    {
        int whatEyeToOpen = Random.Range(0, _closedBossEyes.Count - 1);

        if (_closedBossEyes.Count > 0)
            _closedBossEyes[whatEyeToOpen].GetComponent<BossEye>().DoEyeOpen(eyeHitCount, _Eyes[whatEyeType]);
    }

    internal void SpawnHomingBullet(Vector2 spawnPosition, float scale, float addedRotation)
    {
        GameObject bullet = homingPool.DoSpawn(spawnPosition);
        bullet.GetComponent<B1HomingProjectile>().BulletStart(playerTransformReference, homingBulletVelocity, addedRotation);
        bullet.transform.localScale = new Vector2(scale, scale);
    }
}