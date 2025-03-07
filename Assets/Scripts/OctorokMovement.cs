using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctorokMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float healthPoints = 1f;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float timeOnEdge = 1f;
    [SerializeField] private float shootCooldown = 3f;
    [SerializeField] private Rock rock;
    [SerializeField] float rockSpeed = 1f;
    [SerializeField] float rockPush = 1f;
    [SerializeField] private float rockPower = 1f;
    [SerializeField] private int givenScore;
    private GameObject _sprite;
    private BoxCollider2D _collider2D;
    private CliffDetection _cliffDetection;
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _vision;
    private BlockDetection _blockDetection;
    private DamageControl _damageControl;
    private GroundDetection _groundDetection;
    private float _rockCooldownTimer;
    [HideInInspector]
    public bool isWalking = true;

    private bool _onEdge = false;
    

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _sprite = transform.GetChild(0).gameObject;
        _collider2D = _sprite.GetComponent<BoxCollider2D>();
        _animator = _sprite.GetComponent<Animator>();
        _cliffDetection = transform.GetChild(1).GetComponent<CliffDetection>();
        _vision = transform.GetChild(2).GetComponent<BoxCollider2D>();
        _blockDetection = transform.GetChild(3).GetComponent<BlockDetection>();
        _damageControl = GetComponent<DamageControl>();
        _groundDetection = transform.GetChild(4).GetComponent<GroundDetection>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_groundDetection.isGrounded)
        {
            isWalking = false;
            _animator.SetBool("IsWalking", false);
            _rigidbody2D.velocity = new Vector2(0f, -5f);
            return;
        }
        
        _rigidbody2D.velocity = new Vector2(isWalking ? moveSpeed * transform.localScale.x: 0, 0);

        if (_damageControl.isBeingDamaged)
        {
            isWalking = false;
            _animator.SetBool("IsWalking", false);
            return;
        }

        isWalking = (!_animator.GetCurrentAnimatorStateInfo(0).IsTag("Damaged")) && !_onEdge;
        _animator.SetBool("IsWalking", isWalking && _groundDetection.isGrounded);
        if (_vision.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            Shoot();
        }
        _rockCooldownTimer -= Time.deltaTime;
    }

    public IEnumerator DelayedFlip()
    {
        isWalking = false;
        _onEdge = true;
        yield return new WaitForSecondsRealtime(timeOnEdge);
        StartCoroutine(FlipSprite());
        isWalking = true;
        _onEdge = false;
    }

    public IEnumerator FlipSprite()
    {
        yield return new WaitForEndOfFrame();
        transform.localScale = new Vector2(-transform.localScale.x, 1f);
    }

    private void Shoot()
    {
        if (_rockCooldownTimer > 0) return;
        Rock rock1 = Instantiate(rock, gameObject.transform.position, Quaternion.identity);
        rock1.speed = rockSpeed * transform.localScale.x;
        rock1.damage = rockPower;
        rock1.pushBack = rockPush;
        _rockCooldownTimer = shootCooldown;
    }
}
