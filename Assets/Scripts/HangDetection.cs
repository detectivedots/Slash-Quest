using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangDetection : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Edge"))
        {
            var position = col.gameObject.transform.position;
            Vector2 nearsetEdge;
            float direction = transform.parent.localScale.x;
            if (direction < 0)
            {
                var bounds = col.bounds;
                nearsetEdge = new Vector2(bounds.max.x, bounds.max.y);
            }
            else
            {
                var bounds = col.bounds;
                nearsetEdge = new Vector2(bounds.min.x, bounds.max.y);
            }
            PlayerController.Instance.SetNearestEdge(nearsetEdge);
            PlayerController.Instance.canHang = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Edge"))
        {
            PlayerController.Instance.canHang = false;
        }
    }
}
