using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float damage = 1;
    [SerializeField] private float pushPower = 1;
    private Rigidbody2D _rigidbody2D;

    private BoxCollider2D _boxCollider2D;

    private Animator _animator;

    private float _maxVelocity;
    private float _minVelocity;
    private float _initialVelocity;
    private static readonly int HighSpeed = Animator.StringToHash("HighSpeed");
    private static readonly int MidSpeed = Animator.StringToHash("MidSpeed");
    private static readonly int LowSpeed = Animator.StringToHash("LowSpeed");

    // Start is called before the first frame update
    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        StartCoroutine(DestroyArrow(20f));
    }

    // Update is called once per frame
    void Update()
    {
        float velocity = Mathf.Abs(_rigidbody2D.velocity.x);
        if (velocity >= (2 / 3.0f) * _maxVelocity)
        {
            _animator.SetTrigger(HighSpeed);
        } else if (velocity >= (1 / 3.0f) * _maxVelocity)
        {
            _animator.SetTrigger(MidSpeed);
        }
        else
        {
            _animator.SetTrigger(LowSpeed);
        }

        _rigidbody2D.gravityScale = (1 - Mathf.Abs(_rigidbody2D.velocity.x / _maxVelocity));
    }

    public void SetVelocities(float initialVelocity, float minVelocity, float maxVelocity)
    {
        this._initialVelocity = initialVelocity;
        this._maxVelocity = maxVelocity;
        this._minVelocity = minVelocity;
        _rigidbody2D.velocity = new Vector2(initialVelocity * Mathf.Sign(transform.localScale.x), 0);
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        StartCoroutine(DestroyArrow(1f));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            DamageControl dc = col.transform.parent.GetComponent<DamageControl>();
            if (dc.isImmune)
            {
                return;
            }
            dc.StopMoving();
            dc.GetDamaged(damage);
            AudioManager.Instance.PlayDamageSound();
            if (!col.transform.parent.CompareTag("Spawner"))
                Push(col, transform.localScale.x * pushPower);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyArrow(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
    
    
    private void Push(Collider2D objectToPush, float pushPower)
    {
        Bounds bounds = objectToPush.bounds;
        if (SafeToPush(bounds, pushPower))
        {
            var o = objectToPush.gameObject;
            if (o.transform.parent != null && o.transform.parent.GetComponent<DamageControl>() != null)
                o = o.transform.parent.gameObject;
            var position = o.transform.position;
            position = new Vector2(position.x + pushPower, position.y);
            o.transform.position = position;
        }
    }

    private bool SafeToPush(Bounds bounds, float pushPower)
    {
        Vector2 center = bounds.center;
        Vector2 size = bounds.size;
        Vector2 boxSize = new Vector2(size.x, size.y + 0.1f); // Add a little extra height to the box
        Vector2 direction = new Vector2(pushPower, 0f);

        return (!Physics2D.OverlapBox(center + direction, boxSize, 0f, LayerMask.GetMask("Ground")));
    }
}
