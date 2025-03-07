using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float healthPoints = 1f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float runSpeed = 2f;
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
        if (_rigidbody2D.velocity.x == 0)
        {
            _animator.SetBool("IsWalking", false);
        }

        if (!_groundDetection.isGrounded)
        {
            _animator.SetBool("IsWalking", false);
            _rigidbody2D.velocity = new Vector2(0f, -5f);
            return;
        }

        if (_damageControl.isBeingDamaged)
        {
            _animator.SetBool("IsWalking", false);
            return;
        }

        _animator.SetBool("IsWalking", true);
        TurnAroundOnEdge();
        if (_vision.IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            _animator.SetBool("IsRunning", true);
            _rigidbody2D.velocity = new Vector2(runSpeed * Mathf.Sign(transform.localScale.x), _rigidbody2D.velocity.y);
        }
        else
        {
            _animator.SetBool("IsRunning", false);
            _rigidbody2D.velocity = new Vector2(moveSpeed * Mathf.Sign(transform.localScale.x), _rigidbody2D.velocity.y);
        }
    }

    void TurnAroundOnEdge()
    {
        if ((_cliffDetection.isOnCliff ||
             _blockDetection.isBlocked) &&
            _groundDetection.isGrounded
           )
        {
            StartCoroutine(FlipSprite());
        }
    }

    public IEnumerator FlipSprite()
    {
        yield return new WaitForEndOfFrame();
        transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
    }
}