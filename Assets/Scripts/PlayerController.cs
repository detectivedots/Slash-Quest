using System;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private float _bigEpsilon = 0.001f;
    [Header("Move")] [SerializeField] float unarmedMoveSpeed = 3f;
    [SerializeField] private float unarmedJumpVelocity = 10f;
    [SerializeField] float armedMoveSpeed = 2f;
    [SerializeField] private float unarmedRunSpeed = 6f;
    [SerializeField] private float armedRunSpeed = 4f;
    [SerializeField] private float armedJumpVelocity = 6f;
    [SerializeField] private FixedJoystick joystick;


    [Header("Attacks")] [SerializeField] private float cooldownForBasicAttacks = 0f;
    [SerializeField] public float[] basicAttackDamages = new[] { 1f, 1f, 1f };
    [SerializeField] public float[] basicAttackPushbacks = new[] { 1f, 1f, 1f };
    [SerializeField] private float cooldownForAirAttacks = 0f;
    [SerializeField] public float airAttackDamage = 1f;
    [SerializeField] public float airAttackPushback = 1f;
    [SerializeField] public float spinAttackDamage = 1f;
    [SerializeField] public float spinAttackPushback = 1f;
    [SerializeField] private float velocityIncreaseForAttackUp = 1f;
    [SerializeField] public float shieldAttackDamage = 1f;
    [SerializeField] public float shieldAttackPushback = 1f;
    [SerializeField] private float shieldAttackCooldown = 0f;
    [SerializeField] public float blockPushback = 1f;
    public int currentAttackNumber;

    [Header("Arrows")] [SerializeField] private Arrow _arrow;
    [SerializeField] private float arrowDamage = 1f;
    [SerializeField] private float shootCooldown = 0f;
    [SerializeField] private float fullChargeTime = 5f;
    [SerializeField] private float minimumPower = 1f;
    [SerializeField] private float maximumPower = 5f;
    
    [Header("Buttons")]
    [SerializeField] private Image rhButtonImage;
    [FormerlySerializedAs("ShootButtonImage")] [SerializeField] private Image shootButtonImage;
    [SerializeField] private Image runButtonImage;
    [Header("Others")]
    public static PlayerController Instance;
    private Animator _animator;
    public Vector2 moveInput;
    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _feetCollider;
    private BoxCollider2D _swordCollider;
    private bool _isFalling;
    private bool _isArmed;
    private bool _isWalking;
    public bool isAttacking;
    public bool canHang;
    private bool _isHanging;
    public bool isAirAttacking;
    private bool _airAttacked;
    private float _runInput;
    private float _gravity;
    private float _basicAttackCooldownTimer;
    private float _airAttackCooldownTimer;
    private float _shieldAttackCooldownTimer;
    private Vector2 _lastPossibleCords;
    public bool isBlocking;
    private bool _wasBlocking;
    public bool isShieldAttacking;
    private bool _isCharging;
    private float _chargeTime;
    private float _shootCooldownTimer;
    private DamageControl _damageControl;
    private Vector2 _nearestEdge;
    [HideInInspector]
    public int score;
    private readonly string[] _attackAnimationNames = { "atk1", "atk2", "atk3" };
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsFalling = Animator.StringToHash("IsFalling");
    private static readonly int IsJumping = Animator.StringToHash("IsJumping");
    private static readonly int IsSwordDrew = Animator.StringToHash("IsSwordDrew");
    private static readonly int Direction = Animator.StringToHash("Direction");
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int AttackNumber = Animator.StringToHash("AttackNumber");
    private static readonly int HangOnEdge = Animator.StringToHash("HangOnEdge");
    private static readonly int IsHanging = Animator.StringToHash("IsHanging");
    private static readonly int AirAtk = Animator.StringToHash("AirAtk");
    private static readonly int IsAirAttacking = Animator.StringToHash("IsAirAttacking");
    private static readonly int AirAttackUp = Animator.StringToHash("AirAttackUp");
    private static readonly int AirAttackDown = Animator.StringToHash("AirAttackDown");
    private static readonly int SpinAtk = Animator.StringToHash("SpinAtk");
    private static readonly int Block1 = Animator.StringToHash("Block");
    private static readonly int IsBlocking = Animator.StringToHash("IsBlocking");
    private static readonly int ShieldAttack = Animator.StringToHash("ShieldAttack");
    private static readonly int IsShieldAttacking = Animator.StringToHash("IsShieldAttacking");
    private static readonly int ChargeSpeed = Animator.StringToHash("ChargeSpeed");
    private static readonly int IsArrowBeingLoaded = Animator.StringToHash("IsArrowBeingLoaded");


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _gravity = _rigidbody2D.gravityScale;
        _feetCollider = GetComponent<BoxCollider2D>();
        _swordCollider = transform.GetChild(0).transform.GetChild(0).GetComponent<BoxCollider2D>();
        _animator.SetFloat(ChargeSpeed, 1 / fullChargeTime);
        _damageControl = GetComponent<DamageControl>();
        SaveData.Instance.time = 0;
        score = (int)SaveData.Instance.StateData[2];
        GetComponent<DamageControl>().healthPoints = (float)SaveData.Instance.StateData[4];
    }


    void Update()
    {
        BackButtonMenu();
        if (joystick.isActiveAndEnabled)
            CheckJoystick();
        UpdateAnimations();
        if (!_isHanging && !_isCharging && !isAttacking && !_damageControl.isBeingDamaged)
        {
            if (!isBlocking)
            {
                Walk();
            }

            ResetAttack();
            FlipSprite();
            Block();
        }
        else if (!_isHanging && !_isCharging && !isAttacking && _damageControl.isBeingDamaged)
        {
            ResetAttack();
        }

        ChangeIfAirborne();
        CooldownUpdates();
        Hang();
        StopMovingWhileBlocking();
        ChargeArrow();
        UpdateLastCords();
    }

    void CheckJoystick()
    {
        Vector2 value = new Vector2(Mathf.Round(joystick.Horizontal),
            Mathf.Round(joystick.Vertical)
        );
        if (!_isHanging && !_damageControl.isBeingDamaged)
        {
            moveInput = value;
            moveInput = new Vector2(moveInput.x != 0 ? Mathf.Sign(moveInput.x) : 0,
                moveInput.y != 0 ? Mathf.Sign(moveInput.y) : 0
            );
        }
        else
        {
            var v2d = value;
            if (v2d.y >= 1 - _bigEpsilon)
            {
                Jump();
            }
            else if (v2d.y <= -1 + _bigEpsilon)
            {
                _animator.SetBool(IsHanging, false);
                moveInput = Vector2.zero;
                var transform1 = transform;
                var position = transform1.position;
                transform1.position = position;
                _isHanging = false;
            }
        }
        
    }

    private void UpdateLastCords()
    {
        Bounds bounds = _feetCollider.bounds;
        Vector2 center = bounds.center;
        Vector2 boxSize = bounds.size;
        if (Physics2D.OverlapBox(center, boxSize, 0f, LayerMask.GetMask("Ground")))
        {
            if (transform.localScale.x > 0f)
            {
                _lastPossibleCords = new Vector2(bounds.min.x, transform.position.y + 1);
            }
            else
            {
                _lastPossibleCords = new Vector2(bounds.max.x, transform.position.y + 1);
            }
            
        }
    }

    public void ReturnToLastPossibleCords()
    {
        transform.position = _lastPossibleCords;
    }


    private void OnMove(InputValue value)
    {
        if (!_isHanging && !_damageControl.isBeingDamaged)
        {
            moveInput = value.Get<Vector2>();
            moveInput = new Vector2(moveInput.x != 0 ? Mathf.Sign(moveInput.x) : 0,
                moveInput.y != 0 ? Mathf.Sign(moveInput.y) : 0
            );
        }
        else
        {
            var v2d = value.Get<Vector2>();
            if (v2d.y >= 1 - _bigEpsilon)
            {
                Jump();
            }
            else if (v2d.y <= -1 + _bigEpsilon)
            {
                _animator.SetBool(IsHanging, false);
                moveInput = Vector2.zero;
                var transform1 = transform;
                var position = transform1.position;
                transform1.position = position;
                _isHanging = false;
            }
        }
    }

    private void Walk()
    {
        Vector2 newVelocity = new Vector2(
            (_isArmed
                ? (armedMoveSpeed + ((armedRunSpeed - armedMoveSpeed) * _runInput))
                : unarmedMoveSpeed + (unarmedRunSpeed - unarmedMoveSpeed) * _runInput) * moveInput.x,
            _rigidbody2D.velocity.y);
        _rigidbody2D.velocity = newVelocity;
    }


    void FlipSprite()
    {
        bool isMoving = moveInput.x != 0;
        if (isMoving)
        {
            float direction = Mathf.Sign(moveInput.x);
            transform.localScale = new Vector2(direction, 1f);
        }
    }

    void BackButtonMenu()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            OnPause();
        }
    }

    private void OnJump()
    {
        Jump();
    }

    public void Jump()
    {
        if (isAttacking || _isCharging || _damageControl.isBeingDamaged) return;
        bool wasHanging = _isHanging;
        if (_isHanging)
        {
            _animator.SetBool(IsHanging, false);
            moveInput = Vector2.zero;
            var transform1 = transform;
            var position = transform1.position;
            transform1.position = position;
            _isHanging = false;
        }

        if (!IsAirBorne() || wasHanging)
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x,
                (_isArmed ? armedJumpVelocity : unarmedJumpVelocity));
    }

    private bool IsAirBorne()
    {
        return !_feetCollider.IsTouchingLayers(LayerMask.GetMask($"Ground"));
    }

    private void OnShoot(InputValue value)
    {
        _isCharging = (value.Get<float>() != 0) && !IsShootCooldowning();
    }

    public void OnShootButton()
    {
        _isCharging = !_isCharging && !IsShootCooldowning();
    }

    private void ChargeArrow()
    {
        if (isAttacking || moveInput.x != 0 || isAttacking || isAirAttacking ||
            !_feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || _isHanging || !_isArmed ||
            isBlocking || _isWalking || IsAirBorne() || IsShootCooldowning() || isShieldAttacking ||
            _damageControl.isBeingDamaged)
        {
            return;
        }

        if (_isCharging && _chargeTime < fullChargeTime)
        {
            shootButtonImage.color = Color.yellow;
            _chargeTime = Mathf.Clamp(_chargeTime + Time.deltaTime, 0, fullChargeTime);
        }
        else if (_chargeTime > 0)
        {
            shootButtonImage.color = Color.white;
            Shoot();
        }
    }

    private void Shoot()
    {
        _isCharging = false;
        float shootPower = (maximumPower - minimumPower) * (_chargeTime / fullChargeTime) + minimumPower;
        _chargeTime = 0;
        Arrow arrow = Instantiate(_arrow, gameObject.transform.position, Quaternion.identity);
        arrow.gameObject.transform.localScale = transform.localScale;
        arrow.SetVelocities(shootPower, minimumPower, maximumPower);
        ShootCooldown(shootCooldown);
    }

    public void OnPause()
    {
        CanvasController canvasController = FindObjectOfType<CanvasController>();
        if (Time.timeScale > 0)
        {
            canvasController.Pause();
        }
        else
        {
            // canvasController.Resume();
        }
    }

    public void OnLeftHand()
    {
        if (_damageControl.isBeingDamaged) return;
        if (!_isArmed)
        {
            DrawSword();
        }
        else if (!isShieldAttacking)
        {
            if (IsAirBorne() && !isBlocking) AirAttack();
            else if (isBlocking) StartShieldAttack();
            else if (moveInput.x == 0) Attack();
        }
    }

    public void OnDrawReturn()
    {
        DrawSword();
    }

    private void DrawSword()
    {
        if (!_isWalking && Mathf.Abs(_rigidbody2D.velocity.y) < Mathf.Epsilon && !_damageControl.isBeingDamaged)
            _isArmed = !_isArmed;
    }

    public void OnRunButton()
    {
        _runInput = 1f - _runInput;
        runButtonImage.color = _runInput > 0 ? Color.yellow : Color.white;
    }

    private void OnRun(InputValue value)
    {
        _runInput = value.Get<float>();
    }


    private void ChangeIfAirborne()
    {
        if (!_feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            _runInput = 0f;
        }
    }

    private void Attack()
    {
        if (IsAirBorne() || IsBasicAttackCooldowning() || isShieldAttacking || _damageControl.isBeingDamaged)
        {
            return;
        }

        isAttacking = true;

        if (moveInput.y != 0f)
        {
            StartSpinAttack();
            return;
        }
        if (currentAttackNumber > 0) return;
        _animator.SetBool(IsAttacking, isAttacking);
        _animator.SetTrigger(_attackAnimationNames[currentAttackNumber++]);
    }

    private void AirAttack()
    {
        if (!IsAirBorne() || isAirAttacking || _airAttacked || IsAirAttackCooldowning() ||
            _damageControl.isBeingDamaged) return;
        isAirAttacking = true;
        _airAttacked = true;
        if (moveInput.y > 0)
        {
            StartAirAttackUp();
        }
        else if (moveInput.y < 0)
        {
            StartAirAttackDown();
        }
        else
        {
            _animator.SetTrigger(AirAtk);
        }
    }

    private void StartAirAttackUp()
    {
        var velocity = _rigidbody2D.velocity;
        velocity = new Vector2(velocity.x, velocity.y + velocityIncreaseForAttackUp);
        _rigidbody2D.velocity = velocity;
        _animator.SetTrigger(AirAttackUp);
    }

    private void StartSpinAttack()
    {
        _animator.SetTrigger(SpinAtk);
    }

    private void StartAirAttackDown()
    {
        _rigidbody2D.gravityScale *= 2;
        _animator.SetTrigger(AirAttackDown);
    }

    private void StartShieldAttack()
    {
        if (isShieldAttacking || IsShieldAttackCooldowning()) return;
        isShieldAttacking = true;
        _animator.SetTrigger(ShieldAttack);
        isBlocking = false;
    }

    public void SetAirAttackToFalse()
    {
        isAirAttacking = false;
        AirAttackCooldown(cooldownForAirAttacks);
    }

    public void SetShieldAttackToFalse()
    {
        isBlocking = true;
        isShieldAttacking = false;
        ShieldAttackCooldown(shieldAttackCooldown);
    }

    public bool IsAnyAttacking()
    {
        return isAttacking || isAirAttacking || isShieldAttacking;
    }

    public bool IsHitboxActive()
    {
        return IsAnyAttacking() || isBlocking;
    }

    public void ResetAttack()
    {
        if (!IsAirBorne())
        {
            _airAttacked = isAirAttacking = false;
            _rigidbody2D.gravityScale = _gravity;
        }

        if (IsAirBorne())
        {
            isAttacking = false;
            currentAttackNumber = 0;
        }
    }

    public void OnUnfreeze()
    {
        isAttacking = false;
        transform.position = new Vector2(_lastPossibleCords.x, _lastPossibleCords.y + 1);
        UnfreezeAttack();
        _animator.gameObject.SetActive(false);
        _animator.gameObject.SetActive(true);
    }

    public void UnfreezeAttack()
    {
        currentAttackNumber = 0;
        _airAttacked = isAirAttacking = false;
        _rigidbody2D.gravityScale = _gravity;
    }


    public void Transition()
    {
        if (currentAttackNumber < 3 && isAttacking)
        {
            _animator.SetTrigger(_attackAnimationNames[currentAttackNumber++]);
        }
        else
        {
            isAttacking = false;
            currentAttackNumber = 0;
            BasicAttackCooldown(cooldownForBasicAttacks);
        }
    }

    public void StopAttack()
    {
        isAttacking = false;
        currentAttackNumber = 0;
        BasicAttackCooldown(cooldownForBasicAttacks);
    }

    void BasicAttackCooldown(float cooldown = 0)
    {
        _basicAttackCooldownTimer = cooldown;
    }


    void AirAttackCooldown(float cooldown = 0)
    {
        _airAttackCooldownTimer = cooldown;
    }

    void ShieldAttackCooldown(float cooldown = 0)
    {
        _shieldAttackCooldownTimer = cooldown;
    }

    void ShootCooldown(float cooldown = 0)
    {
        _shootCooldownTimer = cooldown;
    }


    public void SetAttackToFalse()
    {
        isAttacking = false;
        if (currentAttackNumber == _attackAnimationNames.Length)
        {
            currentAttackNumber = 0;
            BasicAttackCooldown(cooldownForBasicAttacks);
        }
    }


    public void SetNearestEdge(Vector2 edge)
    {
        this._nearestEdge = edge;
    }

    private void Hang()
    {
        if (canHang && Mathf.Abs(moveInput.x - Mathf.Sign(transform.localScale.x)) <= _bigEpsilon && !_isHanging)
        {
            if (Math.Abs(
                    Mathf.Sign(_nearestEdge.x - transform.position.x) - Mathf.Sign(transform.localScale.x)) >
                _bigEpsilon) return;
            _isHanging = true;
            _animator.SetBool(IsHanging, true);
            moveInput = Vector2.zero;
        }

        if (_isHanging)
        {
            var position = _nearestEdge;
            gameObject.transform.position = new Vector2(
                position.x - Mathf.Sign(transform.localScale.x) * 0.189f,
                position.y  - 0.434f
            );
            _rigidbody2D.velocity = Vector2.zero;
        }
    }


    public void OnRightHand(InputValue value)
    {
        if (_damageControl.isBeingDamaged || IsAirBorne()) return;
        isBlocking = (value.Get<float>() > 0f) && _isArmed;
    }

    public void OnRightHandButton()
    {
        if (_damageControl.isBeingDamaged || IsAirBorne()) return;
        isBlocking = !isBlocking;
        rhButtonImage.color = isBlocking ? Color.yellow : Color.white;
    }

    private void Block()
    {
        if (isBlocking && !_wasBlocking)
        {
            _animator.SetTrigger(Block1);
        }

        _wasBlocking = isBlocking;
    }

    private void StopMovingWhileBlocking()
    {
        if (isBlocking || isShieldAttacking)
        {
            moveInput = Vector2.zero;
            _rigidbody2D.velocity = Vector2.zero;
        }
    }


    private void UpdateAnimations()
    {
        WalkingAnimationControl();
        FallAnimationControl();
        JumpAnimationControl();
        ArmedAnimationControl();
        DirectionAnimationControl();
        RunAnimationControl();
        AttackAnimationControl();
        BlockAnimationsControl();
        ShieldAttackAnimationControl();
        ShootingAnimationControl();
    }

    void WalkingAnimationControl()
    {
        _isWalking = moveInput.x != 0f;
        _animator.SetBool(IsWalking, _isWalking);
    }

    private void FallAnimationControl()
    {
        _isFalling = _rigidbody2D.velocity.y < -1 && IsAirBorne();
        _animator.SetBool(IsFalling, _isFalling && !_damageControl.isBeingDamaged);
    }


    private void BlockAnimationsControl()
    {
        _animator.SetBool(IsBlocking, isBlocking);
    }

    private void ShieldAttackAnimationControl()
    {
        _animator.SetBool(IsShieldAttacking, isShieldAttacking);
    }

    private void JumpAnimationControl()
    {
        _animator.SetBool(IsJumping,
            _rigidbody2D.velocity.y > _bigEpsilon || (IsAirBorne() && !_isFalling) && !isAirAttacking);
    }

    private void ArmedAnimationControl()
    {
        _animator.SetBool(IsSwordDrew, _isArmed);
    }

    private void DirectionAnimationControl()
    {
        _animator.SetInteger(Direction, (int)transform.localScale.x);
    }

    private void RunAnimationControl()
    {
        _animator.SetBool(IsRunning, _runInput > Mathf.Epsilon);
    }

    private void AttackAnimationControl()
    {
        _animator.SetBool(IsAttacking, isAttacking);
        _animator.SetBool(IsAirAttacking, isAirAttacking);
    }

    private void ShootingAnimationControl()
    {
        _animator.SetBool(IsArrowBeingLoaded, _isCharging);
    }

    private void CooldownUpdates()
    {
        UpdateBasicAttackCooldown();
        UpdateAirAttackCooldown();
        UpdateShieldAttackCooldown();
        UpdateShootCooldown();
    }

    void UpdateBasicAttackCooldown()
    {
        if (IsBasicAttackCooldowning())
            _basicAttackCooldownTimer -= Time.deltaTime;
    }

    private bool IsBasicAttackCooldowning()
    {
        return _basicAttackCooldownTimer > 0;
    }


    void UpdateAirAttackCooldown()
    {
        if (IsAirAttackCooldowning())
            _airAttackCooldownTimer -= Time.deltaTime;
    }

    private bool IsAirAttackCooldowning()
    {
        return _airAttackCooldownTimer > 0;
    }

    void UpdateShieldAttackCooldown()
    {
        if (IsShieldAttackCooldowning())
            _shieldAttackCooldownTimer -= Time.deltaTime;
    }

    private bool IsShieldAttackCooldowning()
    {
        return _shieldAttackCooldownTimer > 0;
    }

    void UpdateShootCooldown()
    {
        if (IsShootCooldowning())
            _shootCooldownTimer -= Time.deltaTime;
    }

    private bool IsShootCooldowning()
    {
        return _shootCooldownTimer > 0;
    }
}