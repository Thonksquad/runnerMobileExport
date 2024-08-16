using System.Collections; 
using UnityEngine;
using static Boss1;
using Utilities.Cooldown;
using System.Collections.Generic;
using UnityServiceLocator;


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
    private EdgeCollider2D edgeCollider2D;

    private IEnumerator shootCoroutineReference;
    private Boss1 bossScrReference => transform.parent.GetComponent<Boss1>();


    private Cooldown _cd1; 
    private Cooldown _cd2 = new(0.4f);
    private Cooldown _cd3 = new(0.2f);

    private Player _player;
    private Vector3 _playerTarget;


    private void Awake()
    {
        eyeRenderer = GetComponent<SpriteRenderer>();
        laserLineRenderer = GetComponent<LineRenderer>();
        edgeCollider2D = GetComponent<EdgeCollider2D>();
    }

    private void Start()
    {
        ServiceLocator.ForSceneOf(this).Get(out _player);
        DoEyeClose(false);
    }


    private void SetEdgeCollider()
    {
        edgeCollider2D.points = new Vector2[] {
            transform.position - transform.position,
            _playerTarget - transform.position
        };
    }



    #region Open Close Eye
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
    #endregion // Open Close Eye

    #region Shoot
    private IEnumerator Handle_Shooting()
    {
        _cd1 = new(thisEyeEntry.reloadTime);
        while (canBeHit)
        { 
            _cd1.Start();
            while (_cd1.IsActive)
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
        _playerTarget = _player.transform.position;

        _cd2.Start();
        while (_cd2.IsActive)
            yield return null;


        while (laserLineRenderer.startWidth < 1.5f)
        {
            laserLineRenderer.startWidth += 0.02f;
            laserLineRenderer.endWidth += 0.02f;
            yield return null;
        }

        SetEdgeCollider();
        edgeCollider2D.enabled = true;
        _cd3.Start();
        while ( _cd3.IsActive)
            yield return null;
        edgeCollider2D.enabled = false;

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
    #endregion // Shoot

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

    #region Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Bullet>() != null)
        {
            collision.gameObject.GetComponent<Bullet>().ReturnToPool();
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

        if (collision.gameObject.GetComponent<Player>() != null)
        { 
            collision.gameObject.GetComponent<Player>().EnterBossLaser();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    { 
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            collision.gameObject.GetComponent<Player>().ExitBossLaser();
        }

    }
    #endregion // Trigger



}
