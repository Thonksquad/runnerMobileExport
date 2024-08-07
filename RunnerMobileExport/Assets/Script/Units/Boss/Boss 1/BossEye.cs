using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Boss1;

public class BossEye : MonoBehaviour
{
    [SerializeField] internal bool canBeHit;
    [SerializeField] private Animator eyeLidAnim;
    [SerializeField] private EyePoint pupilPivotScript;
    [SerializeField] SpriteRenderer eyePupil;

    internal int hitHP;
    private EyeEntries thisEyeEntry;
    private SpriteRenderer eyeRenderer;
    private LineRenderer laserLineRenderer;
    private IEnumerator shootCoroutineReference;
    private Boss1 bossScrReference => transform.parent.GetComponent<Boss1>();

    private void Awake()
    {
        eyeRenderer = GetComponent<SpriteRenderer>();
        laserLineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        DoEyeClose(false);
    }

    internal void DoEyeClose(bool doDamage)
    {
        StopAllCoroutines();
        bossScrReference._closedBossEyes.Add(gameObject);
        bossScrReference.shakeScript.Do_shake(0.3f, 0.5f);

        canBeHit = false;
        eyeLidAnim.CrossFade("B1EyelidClose", 0, 0);
        pupilPivotScript.enabled = false;
        laserLineRenderer.enabled = false;

        if (doDamage)
            BossHandler.bossTakeDamage(10);
    }
    internal void DoEyeOpen(int hpValue, EyeEntries eyeEntry)
    {
        StopAllCoroutines();
        pupilPivotScript.doFollow = true;
        bossScrReference._closedBossEyes.Remove(gameObject);
        thisEyeEntry = eyeEntry;

        eyePupil.sprite = eyeEntry.pupilSprite;
        eyeRenderer.color = eyeEntry.eyeColor;
        hitHP = hpValue;
        pupilPivotScript.enabled = true;
        eyeLidAnim.CrossFade("B1EyelidOpen", 0, 0);
    }

    internal void OpeningEyeDoneEnableThisEye() // TO BE CALLED BY OPEN EYE ANIMATION
    {
        canBeHit = true;

        if (thisEyeEntry.eyeType == EyeTypesEnum.LASER) // IF EYE TYPE IS LASER turn line renderer ON
            StartCoroutine(Handle_LaserLineRenderer());

        shootCoroutineReference = Handle_Shooting();
        StartCoroutine(shootCoroutineReference);
    }

    private IEnumerator Handle_Shooting()
    {
        while (canBeHit)
        {
            float newT2 = Time.time;    // REPLACE WITH NEW TIMER SYSTEM
            float newTimer = thisEyeEntry.reloadTime;
            while (Time.time < newT2 + newTimer)
                yield return null;
            //  yield return new WaitForSeconds(thisEyeEntry.reloadTime);   // RELOAD RATE / basic will reload 1x slower 
            StartCoroutine("DoShoot_" + thisEyeEntry.eyeType.ToString());      // Determine what type of eye this is and do its shooting pattern
        }
    }

    private IEnumerator DoShoot_NORMAL()
    {
        bossScrReference.SpawnHomingBullet(transform.position, 1.5f, 0);
        yield return null;
    }
    private IEnumerator DoShoot_TRIPPLE()
    {
        bossScrReference.SpawnHomingBullet(transform.position, 1, 0);
        bossScrReference.SpawnHomingBullet(transform.position, 1, 30);
        bossScrReference.SpawnHomingBullet(transform.position, 1, -30);
        yield return null;
    }
    private IEnumerator DoShoot_LASER()
    {
        StopCoroutine(shootCoroutineReference);
        float origSize = laserLineRenderer.startWidth;

        pupilPivotScript.doFollow = false;
        canBeHit = false;

        float newT = Time.time;    // REPLACE WITH NEW TIMER SYSTEM
        float newTimer2 = 0.4f;
        while (Time.time < newT + newTimer2)
            yield return null;


        while (laserLineRenderer.startWidth < 1.5f)
        {
            laserLineRenderer.startWidth += 0.01f;
            laserLineRenderer.endWidth += 0.01f;
            yield return null;
        }

        float newT2 = Time.time;    // REPLACE WITH NEW TIMER SYSTEM
        float newTimer = 0.35f;
        while (Time.time < newT2 + newTimer)
            yield return null;

        while (laserLineRenderer.startWidth > origSize)
        {
            laserLineRenderer.startWidth -= 0.005f;
            laserLineRenderer.endWidth -= 0.005f;
            yield return null;
        }
        laserLineRenderer.startWidth = origSize;
        laserLineRenderer.endWidth = origSize;
        pupilPivotScript.doFollow = true;
        canBeHit = true;

        shootCoroutineReference = Handle_Shooting();
        StartCoroutine(shootCoroutineReference);
    }

    private IEnumerator Handle_LaserLineRenderer()
    {
        laserLineRenderer.enabled = true;

        while (true)
        {
            laserLineRenderer.SetPosition(0, new Vector3(pupilPivotScript.transform.position.x, pupilPivotScript.transform.position.y, -0.1f));
            laserLineRenderer.SetPosition(1, pupilPivotScript.transform.position -pupilPivotScript.transform.right * 50);
            yield return null;
        }
    }

    private IEnumerator EyeHit()
    {
        Color origColor = eyeRenderer.color;
        eyeRenderer.color = Color.red;
            
        while (eyeRenderer.color != origColor)
        {
            eyeRenderer.color = Color.Lerp(eyeRenderer.color, origColor, 0.01f);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            collision.gameObject.SetActive(false);

            if (canBeHit)
            {
                hitHP--;
                if (hitHP <= 0)
                    DoEyeClose(true);
                else
                    BossHandler.bossTakeDamage(1);

                StopCoroutine(EyeHit());
                StartCoroutine(EyeHit());
            }
        }
    }
}
