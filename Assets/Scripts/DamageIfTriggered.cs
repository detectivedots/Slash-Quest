using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIfTriggered : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float pushBack = 1f;
    private DamageControl _damageControl;

    private void OnTriggerEnter2D(Collider2D col)
    {
        _damageControl = col.GetComponent<DamageControl>();
        if (col.tag == "Hitbox")
        {
            if (col.transform.parent.parent.GetComponent<PlayerController>().isBlocking)
            {
                DamageControl dc = transform.parent.GetComponent<DamageControl>();
                dc.StopMoving();
                dc.GetDamaged(0);
            }
        }
        if (_damageControl != null)
        {
            if (!_damageControl.isImmune)
            {
                _damageControl.GetDamaged(damage);
                _damageControl.StopMoving();
                Push(col, -Mathf.Sign(transform.position.x - col.gameObject.transform.position.x) *
                                         pushBack);
            }
        }
    }
    
    private void Push(Collider2D objectToPush, float pushPower)
    {
        Bounds bounds = objectToPush.bounds;
        if (SafeToPush(bounds, pushPower))
        {
            var o = objectToPush.gameObject;
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