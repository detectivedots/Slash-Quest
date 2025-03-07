using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BlockDetection : MonoBehaviour
{
    private BoxCollider2D _boxCollider2D;
    public bool isBlocked;

    private void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Ground"))
        {
            isBlocked = true;
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        isBlocked = other.CompareTag("Ground");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            isBlocked = false;
        }
    }
}
