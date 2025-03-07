using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public float speed = 1f;
    public float damage = 1f;
    public float pushBack = 0.1f;
    private Rigidbody2D _rigidbody2D;
    private bool _isDeflected;
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _rigidbody2D.velocity = new Vector2(speed * transform.localScale.x, 0);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Hitbox") && col.transform.parent.parent.GetComponent<PlayerController>().IsHitboxActive())
        {
            var transform1 = transform;
            var localScale = transform1.localScale;
            localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
            transform1.localScale = localScale;
            _isDeflected = true;
        }
        else if ((col.CompareTag("Player") && !_isDeflected) || (col.CompareTag("Enemy") && _isDeflected) )
        {
            DamageControl damageControl = col.GetComponent<DamageControl>();
            if (damageControl == null)
            {
                Transform parent = col.transform.parent;
                damageControl = (parent!=null)? parent.GetComponent<DamageControl>():null;
            }
            if (damageControl != null)
            {
                if (!damageControl.isImmune)
                {
                    damageControl.GetDamaged(damage);
                    damageControl.StopMoving();
                    if (transform.parent != null && !transform.parent.CompareTag("Spawner"))
                         Push(col, -Mathf.Sign(transform.position.x - col.gameObject.transform.position.x) *
                              pushBack);
                    gameObject.SetActive(false);
                    Destroy(gameObject);
                }
            }
        } else if (col.CompareTag("Ground"))
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
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
