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
    [SerializeField] private BossProjectilePool _projectiles;
    private Vector3 initialLocalPosition;
    private IEnumerator phaseBehaviorCoroutine;
    internal BossShake shakeScript;
    [SerializeField] internal List<GameObject> _closedBossEyes = new List<GameObject>();

    [SerializeField] private int eyeHitCount;
    [SerializeField] private Vector2 eyeOpenDelayMINMAX;
    [Header("References")]
    [SerializeField] private Transform playerTransformReference;
    [SerializeField] private List<BossEye> existingBossEyes;
    [SerializeField] private GameObject finalPhaseMeleeEye;
    [Header("Types Of Eyes")]
    [SerializeField] internal List<EyeEntries> _Eyes;
    [Serializable] public class EyeEntries { public EyeTypesEnum eyeType; public Sprite pupilSprite; public float reloadTime; public Color eyeColor;}


    [Header("BossWheelReferences")]
    [SerializeField] private BossWheelAnimation bossWheelAnimation1;
    [SerializeField] private BossWheelAnimation bossWheelAnimation2;

    private Cooldown _phase1ChooseEyeToOpenCd = new(0.5f);
    private Cooldown _phase1OpenEyeCd;
    private Cooldown _phase2ChooseEyeToOpenCd = new(1.75f);
    private Cooldown _phase2OpenEyeCd;
    private Cooldown _phase3ChooseEyeToOpenCd = new(1.5f);
    private Cooldown _phase3OpenEyeCd;
    private Cooldown _deathExitCd = new(3f);

    public enum EyeTypesEnum
    {
        NORMAL,
        TRIPPLE,
        LASER
    }


    #region Unity Callbacks
    private void Awake()
    {
        shakeScript = GetComponent<BossShake>();
    }

    private void OnEnable()
    {
        _phase1OpenEyeCd = new(Random.Range(eyeOpenDelayMINMAX.x, eyeOpenDelayMINMAX.y));
        _phase2OpenEyeCd = new(Random.Range(eyeOpenDelayMINMAX.x, eyeOpenDelayMINMAX.y));
        _phase3OpenEyeCd = new(Random.Range(eyeOpenDelayMINMAX.x, eyeOpenDelayMINMAX.y));
        _phase1ChooseEyeToOpenCd.Completed += Phase1_ChooseEyeToOpenHandler;
        _phase1OpenEyeCd.Completed += Phase1OpenEyeHandler;
        _phase2ChooseEyeToOpenCd.Completed += Phase2_ChooseEyeToOpenHandler;
        _phase2OpenEyeCd.Completed += Phase2OpenEyeHandler;
        _phase3ChooseEyeToOpenCd.Completed += Phase3_ChooseEyeToOpenHandler;
        _phase3OpenEyeCd.Completed += Phase3OpenEyeHandler;
        _deathExitCd.Completed += Exited;
        BossHandler.bossCurrentHP = health;
        initialLocalPosition = transform.localPosition;
        transform.localPosition = new Vector3(initialLocalPosition.x + 30, initialLocalPosition.y , initialLocalPosition.z );
        StartCoroutine(Do_Intro());
    } 

    private void OnDisable()
    {
        _phase1ChooseEyeToOpenCd.Completed -= Phase1_ChooseEyeToOpenHandler;
        _phase1OpenEyeCd.Completed -= Phase1OpenEyeHandler;
        _phase2ChooseEyeToOpenCd.Completed -= Phase2_ChooseEyeToOpenHandler;
        _phase2OpenEyeCd.Completed -= Phase2OpenEyeHandler;
        _phase3ChooseEyeToOpenCd.Completed -= Phase3_ChooseEyeToOpenHandler;
        _phase3OpenEyeCd.Completed -= Phase3OpenEyeHandler;
        _deathExitCd.Completed -= Exited;
    }
    #endregion // Unity Callbacks


    private IEnumerator Do_Intro()
    {
        bossWheelAnimation1.StartPhase();
        bossWheelAnimation2.StartPhase();

        float lerpDuration = 0.75f;
        float lerpSpeed = 1f / lerpDuration;

        while (Mathf.Abs(initialLocalPosition.x - transform.localPosition.x) > 0.1f)
        {
            transform.localPosition = new Vector3(Mathf.Lerp(transform.localPosition.x, initialLocalPosition.x, lerpSpeed * Time.deltaTime), transform.localPosition.y, transform.localPosition.z);
            yield return null;
        }

        transform.localPosition = initialLocalPosition;

        bossWheelAnimation1.FightPhase();
        bossWheelAnimation2.FightPhase();

        shakeScript.Do_shake(0.7f, 0.4f);
        yield return new WaitForSeconds(0.4f);

        switch (phase)
        {
            case 1:
                _phase1ChooseEyeToOpenCd.Start();
                break;
            case 2:
                _phase2ChooseEyeToOpenCd.Start();
                break;
        }
    }

    #region Phase1
    private void Phase1_ChooseEyeToOpenHandler()
    {
        StartCoroutine(Phase1_ChooseEyeToOpen()); 
    }

    private IEnumerator Phase1_ChooseEyeToOpen()
    { 
        OpenRandomEye(0); 
        while (BossHandler.bossCurrentHP > (float)BossHandler.bossMaxHP * 0.75)
        {
            while (_closedBossEyes.Count <= 0)
            {
                yield return null;
            }

            _phase1OpenEyeCd.Start();
            yield return null;
        }

        // when HP reaches below 50% go phase 2 since phase 3 isnt available
        Do_Changephase(2);
    }

    private void Phase1OpenEyeHandler()
    { 
        int phase1EyeType;

        if (Random.Range(0, 2) == 1)    //Spawn eye with either basic or 50% chance of tripple
            phase1EyeType = 1;
        else
            phase1EyeType = 0;

        if (BossHandler.bossCurrentHP > (float)BossHandler.bossMaxHP * 0.75)
        {
            OpenRandomEye(phase1EyeType);
        }   
    }
    #endregion //Phase1

    #region Phase2
    private void Phase2_ChooseEyeToOpenHandler()
    {
        StartCoroutine(Phase2_ChooseEyeToOpen());
    }

    private IEnumerator Phase2_ChooseEyeToOpen()
    {

        OpenRandomEye(Random.Range(0, 3)); 
        OpenRandomEye(2);


        while (BossHandler.bossCurrentHP > (float)BossHandler.bossMaxHP * 0.33)
        {
            while (_closedBossEyes.Count <= 0)
            {
                yield return null;
            }
            _phase2OpenEyeCd.Start(); 

            yield return null;
        }

        Do_Changephase(3);
    }

    private void Phase2OpenEyeHandler()
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
    #endregion //Phase2

    #region Phase3
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
        while (BossHandler.bossCurrentHP > 0)
        {
            while (_closedBossEyes.Count <= 0)
            {
                yield return null;
            }
            _phase3ChooseEyeToOpenCd.Start();
            

            yield return null;
        }

        Do_Death();
    } 

    private void Phase3OpenEyeHandler()
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
    #endregion // Phase3

    private void Do_Death()
    {
        Vector3 origPos = transform.localPosition;
        finalPhaseMeleeEye.SetActive(false);
        foreach (BossEye eye in existingBossEyes)
        {
            eye.gameObject.SetActive(false);
        }

        bossWheelAnimation1.Stop();
        bossWheelAnimation2.Stop();

        shakeScript.Do_shake(0.8f, 2.0f);

        _deathExitCd.Start();
        StartCoroutine(DoExit()); 
        transform.localPosition = origPos;
    }
    private IEnumerator DoExit()
    {
        bool x = _deathExitCd.IsActive;
        while (x)
        {
            transform.localPosition = transform.localPosition + Vector3.right * Time.deltaTime * 4;
            x = _deathExitCd.IsActive;
            yield return null;
        } 
    }

    private void Exited()
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
            _phase2ChooseEyeToOpenCd.Start();
        }
        else if (phaseToChangeTo == 3)
        {
            shakeScript.Do_shake(0.75f, 0.85f);
            _phase3ChooseEyeToOpenCd.Start();
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
        B1Projectile bullet = _projectiles.DoSpawn(spawnPosition);
        bullet.GetComponent<B1Projectile>().BulletStart(playerTransformReference, addedRotation);
        bullet.transform.localScale = new Vector2(scale, scale);
    }
}