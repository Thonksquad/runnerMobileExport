using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

public class Player : MonoBehaviour
{
    private int LayerPlayer;
    private int LayerEnemy;
    public int hp;
    public int maxHP = 1;
    public Rigidbody2D body;
    public bool gameOver = false;
    public Animator myAnim;
    public PlayerInputActions playerControls;

    public bool isInvulnerable;
    public bool onHound;
    public bool isFlying;
    public bool isShooting;

    public InputAction fly;
    public InputAction fire;

    public GameObject houndbulletPrefab;
    public GameObject bulletPrefab;
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

    [SerializeField] private Transform _Koda;
    [SerializeField] private Fart _FartPrefab;
    public Coroutine fartRoutine;

    private bool LandedThisFrame;
    [SerializeField] private PlayerAnimationHandler AnimationHandler;

    private void Awake()
    {
        int LayerPlayer = LayerMask.NameToLayer("Player");
        int LayerEnemy = LayerMask.NameToLayer("Enemy");
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        ActionSystem.onAdRevive += AdRevive;
        ActionSystem.onPlayerRecover += TurnCollisionOn;
        fly = playerControls.Player.Fly;
        fly.Enable();
        fire = playerControls.Player.Fire;
        fire.Enable();
        fire.performed += OnFire;
    }

    private void OnDisable()
    {
        ActionSystem.onAdRevive -= AdRevive;
        ActionSystem.onPlayerRecover -= TurnCollisionOn;
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
                Instantiate(bulletPrefab, bulletTransform.position, Quaternion.identity);
            }
        }
    }

    private void HoundFire()
    {
        Instantiate(houndbulletPrefab, bulletTransform.position, Quaternion.identity);
    }

    void Update()
    {

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
                    Instantiate(_FartPrefab, _Koda.position, Quaternion.identity);
                    canFart = false;
                    fartTimer = 0;
                }
            }
        }

        if (gameOver & gameOverCounter < 1)
        {
            gameOverCounter++;
            gameTrackerScreen.SetActive(false);
            ActionSystem.onPlayerHit();
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

    private void AdRevive()
    {
        gameOver = false;
        gameOverCounter = 0;
        Physics2D.IgnoreLayerCollision(6, 8);
        isInvulnerable = true;
        gameTrackerScreen.SetActive(true);
        Invoke(nameof(TurnCollisionOn), 2);
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
}
