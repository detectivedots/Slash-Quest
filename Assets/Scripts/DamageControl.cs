using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class DamageControl : MonoBehaviour
{
    [FormerlySerializedAs("healthPoints")]
    [SerializeField] public float maxHealth = 1;
    [SerializeField] private float immuneTime = 2f;
    [SerializeField] private GameObject animatedObject;
    [SerializeField] private bool isBoss = false;
    [SerializeField] public int givenScore;
    [SerializeField] public bool isPlayer;
    public bool isImmune = false;
    [HideInInspector]
    public float healthPoints;
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private static readonly int Damaged = Animator.StringToHash("Damaged");
    private SpriteRenderer _spriteRenderer;
    public bool isBeingDamaged;

    public void Start()
    {
        _spriteRenderer = animatedObject.gameObject.GetComponent<SpriteRenderer>();
        _animator = animatedObject.GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        healthPoints = maxHealth;
    }

    public void Update()
    {
        if (!gameObject.activeSelf) return;
        if (isImmune)
        {
            _spriteRenderer.color =
                Color.white - _spriteRenderer.color;
        }
        

        if (healthPoints <= 0 && !gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.score += givenScore;
            if (!isBoss)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
            else
            {
                BossInfo.Instance.bossesKilled++;
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
        } else if (healthPoints <= 0 && (isPlayer || gameObject.CompareTag("Player")))
        {
            try
            {
                PlayerController.Instance.score /= 2;
                SaveData.Instance.StateData[2] = PlayerController.Instance.score;
                SaveData.Instance.StateData[4] = maxHealth;
                if (Application.isMobilePlatform)
                {
                    healthPoints = maxHealth;
                }
                SaveData.Instance.SaveToFile();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            catch (Exception e)
            {
                FindObjectOfType<CanvasController>().errorText.text = e.Message;
                throw;
            }
            
        }
    }

    public void StopMoving()
    {
        if (isImmune) return;
        if (gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.moveInput = new Vector2(0, 0);
            PlayerController.Instance.StopAttack();
        }

        _rigidbody2D.velocity = new Vector2(0, 0);
    }

    public void GetDamaged(float damage = 1f)
    {
        if (isImmune) return;
        if (gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.moveInput = Vector2.zero;
            _rigidbody2D.velocity = Vector2.zero;
        }
        isImmune = true;
        if (_animator != null)
            _animator.SetTrigger(Damaged);
        healthPoints -= damage;
        isBeingDamaged = true;
        StartCoroutine(CancelImmunity());
        StartCoroutine(SetIsBeingDamagedToFalse());
    }


    IEnumerator CancelImmunity()
    {
        yield return new WaitForSecondsRealtime(immuneTime);
        _spriteRenderer.color = Color.white;
        isImmune = false;
    }

    IEnumerator SetIsBeingDamagedToFalse()
    {
        yield return new WaitForEndOfFrame();
        StartCoroutine(SetIsBeingDamagedToFalse2());
    }

    IEnumerator SetIsBeingDamagedToFalse2()
    {
        if (_animator != null)
        {
            float animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
            float animSpeed = _animator.GetCurrentAnimatorStateInfo(0).speedMultiplier;
            yield return new WaitForSecondsRealtime(animLength * animSpeed);
            StartCoroutine(SetIsBeingDamagedToFalse3());
        }
    }

    IEnumerator SetIsBeingDamagedToFalse3()
    {
        yield return new WaitForEndOfFrame();
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsTag("Damaged"))
        {
            isBeingDamaged = false;
        }
    }

    public void OnPlayerFallOutOfBounds()
    {
        healthPoints--;
        GetComponent<PlayerController>().ReturnToLastPossibleCords();
        _rigidbody2D.velocity = new Vector2(0, 0);
    }
}