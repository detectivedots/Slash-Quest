using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    private BoxCollider2D _boxCollider2D;

    private void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if(_boxCollider2D.IsTouchingLayers(LayerMask.GetMask("Player")))LoadNextLevel();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerController.Instance.score += (int)Mathf.Max(0, (60 - SaveData.Instance.time) * 5);
            SaveData.Instance.UpdateData((SceneManager.GetActiveScene().buildIndex + 1) %
                                         (SceneManager.sceneCountInBuildSettings));
            SaveData.Instance.SaveToFile();
            LoadNextLevel();
        }
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }
}