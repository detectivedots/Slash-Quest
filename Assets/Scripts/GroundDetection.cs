using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    private BoxCollider2D _boxCollider2D;
    public bool isGrounded;

    private void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Ground"))
        {
            transform.parent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            isGrounded = true;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            transform.parent.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -1);
            isGrounded = false;
        }
    }


}
