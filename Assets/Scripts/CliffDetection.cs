using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CliffDetection : MonoBehaviour
{
    private BoxCollider2D _boxCollider2D;
    public bool isOnCliff;

    private void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            if (transform.parent.GetComponent<OctorokMovement>() != null)
            {
                StartCoroutine(transform.parent.GetComponent<OctorokMovement>().DelayedFlip());
            }
            else
            {
                var vel = transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity;
                StartCoroutine(transform.parent.gameObject.GetComponent<BoarMovement>().FlipSprite());
                transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-vel.x, vel.y);
            }
        }
    }
    
    
}
