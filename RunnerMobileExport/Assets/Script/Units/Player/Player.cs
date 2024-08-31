using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityServiceLocator;


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
    [SerializeField] private SpawnPool _bulletPool;
    [SerializeField] private SpawnPool _houndBulletPool;
    public Transform bulletTransform;
    public bool canFire;
    private float fireTimer;
    public float fireCD;

    private int gameOverCounter = 0;

    public bool canFart;
    private float fartTimer;
    public float fartCD;
    private float FartAngle;

    private float _lockedTill;
    private AudioClip _currentState;

    [SerializeField] private float flyVelocity;
    [SerializeField] private float fallVelocity;

    [SerializeField] private GameObject gameTrackerScreen;
    [SerializeField] private float _WalkingDuration = 2f;
    [SerializeField] private ParticleSystem _fartSystem;

    private bool LandedThisFrame;
    [SerializeField] private PlayerAnimationHandler AnimationHandler;


    [SerializeField] private TextMeshProUGUI _bossHitsText;
    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _laserColor; 
    public static int bossHit;


    [Header("service locator speed")]
    public float speed = 8f;

    private void Awake()
    {
        ServiceLocator.ForSceneOf(this).Register<Player>(this); // Scene Scope

        int LayerPlayer = LayerMask.NameToLayer("Player");
        int LayerEnemy = LayerMask.NameToLayer("Enemy");
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        ActionSystem.onPlayerRecover += TurnCollisionOn;
        ActionSystem.onPlayerRevive += Revive;
        fly = playerControls.Player.Fly;
        fly.Enable();
        fire = playerControls.Player.Fire;
        fire.Enable();
        fire.performed += OnFire;
    }

    private void OnDisable()
    {
        ActionSystem.onPlayerRecover -= TurnCollisionOn;
        ActionSystem.onPlayerRevive -= Revive;
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
                //Instantiate(bulletPrefab, bulletTransform.position, Quaternion.identity);
                _bulletPool.Spawner(new Vector2(bulletTransform.position.x, bulletTransform.position.y));
            }
        }
    }

    private void HoundFire()
    {
        //Instantiate(houndbulletPrefab, bulletTransform.position, Quaternion.identity);
        _houndBulletPool.Spawner(new Vector2(bulletTransform.position.x, bulletTransform.position.y));
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
                    _fartSystem.Play();
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

            flyVelocity = (float)(8f + (speed * 0.15));
            body.velocity = new Vector3(0, flyVelocity, 0);
        }
        else if (fly.WasReleasedThisFrame())
        {
            isFlying = false;
            fallVelocity = (float)(6f + (speed * 0.15));
            body.velocity = new Vector3(0, -fallVelocity, 0);
        }

    }

    private void TurnCollisionOn()
    {
        isInvulnerable = false;
        Physics2D.IgnoreLayerCollision(6, 8, false);
    }



    public void Revive()
    {
        gameTrackerScreen.SetActive(true);
        TurnCollisionOn();
        gameOver = false;
        gameOverCounter = 0;
    }


    #region take Damage

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
    public void BossDamage()
    {
        if (hp >= 1)
        {
            onHound = false;
        }
        BossHit();
    }

    public void EnterBossLaser()
    {
        if (hp >= 1)
        {
            onHound = false;
        }
        BossHit();
        gameObject.GetComponent<SpriteRenderer>().material.color = _laserColor;
    }

    public void ExitBossLaser()
    {
        gameObject.GetComponent<SpriteRenderer>().material.color = _normalColor;
    }

    private void BossHit()
    {
        bossHit++;
        _bossHitsText.text = bossHit.ToString();
    }
    #endregion // take Damage


}
