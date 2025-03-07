using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    [SerializeField] private GameObject mobToSpawn;
    [SerializeField] private float cooldown = 5f;
    private CircleCollider2D _playerDetector;
    private float _cooldownTimer = 0f;
    [SerializeField] private int givenScore;
    void Start()
    {
        _playerDetector = GetComponent<CircleCollider2D>();
    }
    
    void Update()
    {
        _cooldownTimer -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!col.CompareTag("Player")) return;
        if (_cooldownTimer <= 0)
        {
            Instantiate(mobToSpawn, gameObject.transform.position, quaternion.identity);
            _cooldownTimer = cooldown;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        if (_cooldownTimer <= 0)
        {
            GameObject spawnedMob = Instantiate(mobToSpawn, gameObject.transform.position, quaternion.identity);
            spawnedMob.GetComponent<DamageControl>().givenScore = 0;
            _cooldownTimer = cooldown;
        }
    }
}
