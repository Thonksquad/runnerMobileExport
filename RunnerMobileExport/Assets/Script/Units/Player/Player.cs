using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private int LayerPlayer;
    private int LayerEnemy;
    private Rigidbody2D body;
    private ObjectPool bulletPool = new ObjectPool();
    private ObjectPool fartPool = new ObjectPool();
    private ObjectPool houndBulletPool = new ObjectPool();
    private Transform _Koda;

    public int hp;
    public int maxHP = 1;
    public bool gameOver = false;
    public Animator myAnim;
    public PlayerInputActions playerControls;

    public bool isInvulnerable;
    public bool onHound;
    public bool isFlying;
    public bool isShooting;

    public InputAction fly;
    public InputAction fire;

    public GameObject bulletPrefab;
    public GameObject houndbulletPrefab;
    public Transform bulletTransform;
    public bool canFire;
    private float fireTimer;
    public float fireCD;

    private int gameOverCounter = 0;

    public bool canFart;
    private float fartTimer;
    public float fartCD;

    private float _lockedTill;
    private AudioClip _currentState;

    [SerializeField] private float flyVelocity;
    [SerializeField] private float fallVelocity;

    [SerializeField] private GameObject gameTrackerScreen;
    [SerializeField] private float _WalkingDuration = 2f;

    [SerializeField] private GameObject _FartPrefab;

    [SerializeField] private SpriteRenderer reloadBar;
    public Coroutine fartRoutine;

    private bool LandedThisFrame;
    private PlayerAnimationHandler AnimationHandler;
    public static Player Instance;

    private void Awake()
    {
        Instance = this;
        body = GetComponent<Rigidbody2D>();
        AnimationHandler = GetComponent<PlayerAnimationHandler>();
        _Koda = gameObject.transform.Find("koda");
        int LayerPlayer = LayerMask.NameToLayer("Player");
        int LayerEnemy = LayerMask.NameToLayer("Enemy");
        playerControls = new PlayerInputActions();
        reloadBar.enabled = false;
    }

    private void OnEnable()
    {
        ActionSystem.onPlayerRevive += TurnCollisionOn;
        fly = playerControls.Player.Fly;
        fly.Enable();
        fire = playerControls.Player.Fire;
        fire.Enable();
        fire.performed += OnFire;
    }

    private void OnDisable()
    {
        ActionSystem.onPlayerRevive -= TurnCollisionOn;
        fly.Disable();
        fire.Disable();
        fire.performed -= OnFire;
    }

    private void Start()
    {
        hp = maxHP;
        body = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        LandedThisFrame = AnimationHandler.IsGrounded();
        bulletPool.CreateObjectPool(bulletPrefab, 5);
        fartPool.CreateObjectPool(_FartPrefab, 2);
        houndBulletPool.CreateObjectPool(houndbulletPrefab, 6);
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        if (canFire)
        {
            isShooting = true;
            canFire = false;

            if (onHound)
            {
                Invoke(nameof(HoundFire), 0);
                Invoke(nameof(HoundFire), .1f);
                Invoke(nameof(HoundFire), .2f);
            } else
            {
                //Instantiate(bulletPrefab, bulletTransform.position, Quaternion.identity);
                bulletPool.DoSpawn(bulletTransform.position);
            }

            StartCoroutine(Handle_UIReloadBar());
        }
    }

    private void HoundFire()
    {
        houndBulletPool.DoSpawn(bulletTransform.position);
    }

    void Update()
    {
     //   HandleCursor();

        if (LandedThisFrame != AnimationHandler.IsGrounded())
        {
            LandedThisFrame = AnimationHandler.IsGrounded();
        }

        if (fly.IsPressed())
        {
            isFlying = true;
            if (!canFart)
            {
                isFlying = false;
                fartTimer += Time.deltaTime;
                if (fartTimer > fartCD)
                {
                    fartPool.DoSpawn(_Koda.position);
                    canFart = false;
                    fartTimer = 0;
                }
            }
        }

        if (gameOver & gameOverCounter < 1)
        {
            gameOverCounter++;
            gameTrackerScreen.SetActive(false);
            ActionSystem.onPlayerDeath();
            return;
        }

        if (!canFire)
        {
            isShooting = false;
            fireTimer += Time.deltaTime;
            if (fireTimer > fireCD)
            {
                canFire = true;
                fireTimer = 0;
            }
        }

        if (fly.IsPressed())
        {
            isFlying = true;

            flyVelocity = (float)(8f + (CameraManager.Instance.CamSpeed * 0.15));
            body.velocity = new Vector3(0, flyVelocity, 0);
            // jump logic
            /**
            if (AnimationHandler.IsGrounded())
            {
                body.velocity = new Vector3(0, 32f, 0);
            } else
            {
                body.velocity = new Vector3(0, 8f, 0);
            }
            **/
        }
        else if (fly.WasReleasedThisFrame())
        {
            isFlying = false;
            fallVelocity = (float)(6f + (CameraManager.Instance.CamSpeed * 0.15));
            body.velocity = new Vector3(0, -fallVelocity, 0);
        }

    }

    private void TurnCollisionOn()
    {
        isInvulnerable = false;
        Physics2D.IgnoreLayerCollision(6, 8, false);
    }

    public void TakeDamage(int amount)
    {
        hp -= amount;

        if (hp >= 1)
        {
            onHound = false;
            Physics2D.IgnoreLayerCollision(6, 8);
            isInvulnerable = true;
            Invoke(nameof(TurnCollisionOn), 2);
        }

        if (hp <= 0)
        {
            StopAllCoroutines();
            gameOver = true;
        }
    }

    private IEnumerator Handle_UIReloadBar()
    {
        reloadBar.enabled = true;
        float tempWidth = reloadBar.transform.localScale.x;

        while (!canFire)
        {
            reloadBar.transform.localScale = new Vector2(tempWidth * (fireTimer / fireCD), reloadBar.transform.localScale.y);
            reloadBar.transform.parent.position = transform.position + new Vector3(((tempWidth * (fireTimer / fireCD)) / 2) - tempWidth/2, 0, 0);
            yield return null;
        }

        reloadBar.enabled = false;
    }

  //  private IEnumerator HandleCursor
}
