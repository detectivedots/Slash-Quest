using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeMovement : MonoBehaviour
{
    [SerializeField] private float flyingSpeed = 1f;
    private PlayerStalker _playerStalker;

    private Animator _animator;

    private Rigidbody2D _rigidbody2D;

    private Transform _player;

    private Vector2 _playerPosition;
    [SerializeField] private int givenScore;

    private BoxCollider2D _boxCollider2D;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
        _playerStalker = GetComponentInChildren<PlayerStalker>();
    }

    // Update is called once per frame
    void Update()
    {
        _player = _playerStalker._player;
        if (_player != null)
        {
            _playerPosition = _player.position;
            _rigidbody2D.velocity = new Vector2(
                -Mathf.Sign(transform.position.x - _playerPosition.x) * flyingSpeed,
                -Mathf.Sign(transform.position.y - _playerPosition.y) * flyingSpeed
            );
            transform.localScale = new Vector3(-Mathf.Sign(transform.position.x - _playerPosition.x), 1, 1);
        }
        else
        {
            _rigidbody2D.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            _animator.SetTrigger("Attack");
        }
    }
}
