using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoundaryControl : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<DamageControl>().OnPlayerFallOutOfBounds();
        }
        else
        {
            col.gameObject.SetActive(false);
            Destroy(col.gameObject);
        }
    }
}
