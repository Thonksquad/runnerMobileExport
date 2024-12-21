using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerAnimationHandler : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private BoxCollider2D myCol;
    [SerializeField] private Rigidbody2D _player;
    [SerializeField] private AudioClip footsteps;

    private Player PlayerController;

    [SerializeField] private float _fartAnimDuration = .1f;
    [SerializeField] private float _landAnimDuration = .1f;
    private Animator _anim;
    private SpriteRenderer _renderer;

    private float _lockedTill;
    private bool _landed;
    private bool _shoot;

    private int _currentState;
    private static readonly int Fly = Animator.StringToHash("Fly");
    private static readonly int Run = Animator.StringToHash("Run");
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int InvulnerableRun = Animator.StringToHash("InvulnerableRun");
    private static readonly int HoundRun = Animator.StringToHash("HoundRun");
    private static readonly int HoundFly = Animator.StringToHash("HoundFly");

    private void Awake()
    {
        PlayerController = GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _landed = false;
        _shoot = false;

        var State = GetState();

        if (State == _currentState) return;
        _anim.CrossFade(State, 0, 0);
        _currentState = State;
    }

    private int GetState()
    {

        if (Time.time < _lockedTill) return _currentState;

        //Priorities
        if (PlayerController.isInvulnerable) return InvulnerableRun;
        if (!IsGrounded() & PlayerController.onHound) return HoundFly;
        if (!IsGrounded() & !PlayerController.onHound) return Fly;

        /**
        if (_shoot)
        {
            if (IsGrounded())
            {
                Debug.Log("RunShoot");
                //return RunShoot;
            }
            else
            {
                Debug.Log("FlyShoot");
                //return FlyShoot;
            }
        }
        **/

        //if (!IsGrounded()) Debug.Log("Fall"); return Fall;
        //if (_landed) Debug.Log("Landed"); return LockState(LandAnimation, _landAnimDuration);
        if (IsGrounded() & PlayerController.onHound) return HoundRun;
        if (IsGrounded() & !PlayerController.onHound) return Run;
        return Run;
        //return IsGrounded() ? Run : Fall;

        int LockState(int s, float t)
        {
            _lockedTill = Time.time + t;
            return s;
        }

    }

    public bool IsGrounded()
    {
        float extraHeight = .05f;
        RaycastHit2D raycastHit = Physics2D.Raycast(myCol.bounds.center, Vector2.down, myCol.bounds.extents.y + extraHeight, platformLayer);

        Color rayColor;

        if (raycastHit.collider != null)
        {
            rayColor = Color.green;
        } else
        {
            rayColor = Color.red;
        }

        Debug.DrawRay(myCol.bounds.center, Vector2.down * (myCol.bounds.extents.y + extraHeight), rayColor);
        return raycastHit.collider != null;
    }



}
