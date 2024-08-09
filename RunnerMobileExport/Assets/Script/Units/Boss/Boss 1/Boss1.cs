using System;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using static WaitForTime;
using Random = UnityEngine.Random;
using Utilities.Cooldown;

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

    private Cooldown _cd1 = new(0.5f);
    private Cooldown _cd2;
    private Cooldown _cd3 = new(1.75f);
    private Cooldown _cd4;
    private Cooldown _cd5 = new(1.5f);
    private Cooldown _cd6;
    private Cooldown _cd7 = new(3f);

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
        _cd2 = new(Random.Range(eyeOpenDelayMINMAX.x, eyeOpenDelayMINMAX.y)); 
        _cd4 = new(Random.Range(eyeOpenDelayMINMAX.x, eyeOpenDelayMINMAX.y));
        _cd6 = new(Random.Range(eyeOpenDelayMINMAX.x, eyeOpenDelayMINMAX.y));
        _cd1.Completed += Phase1_ChooseEyeToOpenHandler;
        _cd2.Completed += Cd2;
        _cd3.Completed += Phase2_ChooseEyeToOpenHandler;
        _cd4.Completed += Cd4;
        _cd5.Completed += Phase3_ChooseEyeToOpenHandler;
        _cd6.Completed += Cd6;
        _cd7.Completed += Cd7;
        BossHandler.bossCurrentHP = health;
        initialLocalPosition = transform.localPosition;
        transform.localPosition = new Vector3(initialLocalPosition.x + 30, initialLocalPosition.y , initialLocalPosition.z );
        StartCoroutine(Do_Intro());
    } 

    private void OnDisable()
    {
        _cd1.Completed -= Phase1_ChooseEyeToOpenHandler;
        _cd2.Completed -= Cd2;
        _cd3.Completed -= Phase2_ChooseEyeToOpenHandler;
        _cd4.Completed -= Cd4;
        _cd5.Completed -= Phase3_ChooseEyeToOpenHandler;
        _cd6.Completed -= Cd6;
        _cd7.Completed -= Cd7;
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
                _cd1.Start();
                break;
            case 2:
                _cd3.Start();
                break;
        }
    } 

    private void Phase1_ChooseEyeToOpenHandler()
    {
        StartCoroutine(Phase1_ChooseEyeToOpen()); 
    }

    private IEnumerator Phase1_ChooseEyeToOpen()
    { 
        OpenRandomEye(0); 
        while (BossHandler.bossCurrentHP > (float)BossHandler.bossMaxHP / 2)
        {
            while (_closedBossEyes.Count <= 0)
            {
                yield return null;
            }

            _cd2.Start();
            yield return null;
        }

        // when HP reaches below 50% go phase 2 since phase 3 isnt available
        Do_Changephase(2);
    }

    private void Cd2()
    { 
        int phase1EyeType;

        if (Random.Range(0, 2) == 1)    //Spawn eye with either basic or 50% chance of tripple
            phase1EyeType = 1;
        else
            phase1EyeType = 0;

        if (BossHandler.bossCurrentHP > (float)BossHandler.bossMaxHP / 2)
        {
            OpenRandomEye(phase1EyeType);
        }   
    }


    private void Phase2_ChooseEyeToOpenHandler()
    {
        StartCoroutine(Phase2_ChooseEyeToOpen());
    }

    private IEnumerator Phase2_ChooseEyeToOpen()
    {

        OpenRandomEye(Random.Range(0, 3)); 
        OpenRandomEye(2);


        while (BossHandler.bossCurrentHP > (float)BossHandler.bossMaxHP * 0.25)
        {
            while (_closedBossEyes.Count <= 0)
            {
                yield return null;
            }
            _cd4.Start(); 

            yield return null;
        }

        Do_Changephase(3);
    }

    private void Cd4()
    { 
        int phase2EyeType;

        phase2EyeType = Random.Range(0, 3);
        OpenRandomEye(phase2EyeType);

        if (_closedBossEyes.Count == 3) // IF ALL EYES ARE CLOSED OPEN ANOTHER EYE
        {
            phase2EyeType = Random.Range(0, 3);
            OpenRandomEye(phase2EyeType);
        }
    }

    private void Phase3_ChooseEyeToOpenHandler()
    {
        StartCoroutine(Phase3_ChooseEyeToOpen());
    }

    private IEnumerator Phase3_ChooseEyeToOpen()
    { 
        existingBossEyes[1].gameObject.SetActive(false);
        finalPhaseMeleeEye.SetActive(true);
        OpenRandomEye(Random.Range(0, 3)); 
        OpenRandomEye(2);
        //  while (BossHandler.bossCurrentHP > (float)BossHandler.bossMaxHP * 0.15f)
        while (BossHandler.bossCurrentHP > 0)
        {
            while (_closedBossEyes.Count <= 0)
            {
                yield return null;
            }
            _cd6.Start();
            

            yield return null;
        }

        Do_Death();
    }


    private void Cd6()
    {
        int phase2EyeType;

        phase2EyeType = Random.Range(0, 3);
        OpenRandomEye(phase2EyeType);

        if (_closedBossEyes.Count == 3) // IF ALL EYES ARE CLOSED OPEN ANOTHER EYE
        {
            phase2EyeType = Random.Range(0, 3);
            OpenRandomEye(phase2EyeType);
        }
    }

    private void Do_Death()
    {
        Vector3 origPos = transform.localPosition;
        finalPhaseMeleeEye.SetActive(false);
        foreach (BossEye eye in existingBossEyes)
        {
            eye.gameObject.SetActive(false);
        }

        shakeScript.Do_shake(1.2f, 3.0f);

        _cd7.Start();
        StartCoroutine(DoExit()); 
        transform.localPosition = origPos;
    }
    private IEnumerator DoExit()
    {
        bool x = _cd7.IsActive;
        while (x)
        {
            transform.localPosition = transform.localPosition + Vector3.right * Time.deltaTime * 4;
            x = _cd7.IsActive;
            yield return null;
        } 
    }

    private void Cd7()
    { 
        Do_reset();
        gameObject.SetActive(false);
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
            _cd3.Start();
        }
        else if (phaseToChangeTo == 3)
        {
            shakeScript.Do_shake(0.75f, 0.85f);
            _cd5.Start();
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