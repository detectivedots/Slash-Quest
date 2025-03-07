using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject _player;
    private PlayerController _playerController;

    void Start()
    {
        _player = transform.parent.parent.gameObject;
        _playerController = _player.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        DamageControl damageControl = col.GetComponent<DamageControl>();
        if (damageControl == null)
        {
            Transform parent = col.transform.parent;
            damageControl = (parent != null) ? parent.GetComponent<DamageControl>() : null;
        }

        if (damageControl != null)
        {
            if (damageControl.isImmune) return;
            if (_playerController.isAttacking)
            {
                AudioManager.Instance.PlayDamageSound();

                if (_playerController.currentAttackNumber == 0)
                {
                    damageControl.GetDamaged(
                        _playerController.spinAttackDamage);
                    damageControl.StopMoving();
                    if (!col.transform.parent.CompareTag("Spawner"))
                    {
                        Push(col, -Mathf.Sign(transform.position.x - col.gameObject.transform.position.x) *
                                  _playerController.spinAttackDamage);
                    }
                }
                else
                {
                    damageControl.GetDamaged(
                        _playerController.basicAttackDamages[_playerController.currentAttackNumber - 1]);
                    damageControl.StopMoving();
                    if (!col.transform.parent.CompareTag("Spawner"))
                    {
                        Push(col, -Mathf.Sign(transform.position.x - col.gameObject.transform.position.x) *
                                  _playerController.basicAttackPushbacks[
                                      _playerController.currentAttackNumber - 1]);
                    }
                }
            }
            else if (_playerController.isAirAttacking)
            {
                AudioManager.Instance.PlayDamageSound();

                damageControl.GetDamaged(_playerController.airAttackDamage);
                damageControl.StopMoving();
                if (!col.transform.parent.CompareTag("Spawner"))
                {
                    Push(col, -Mathf.Sign(transform.position.x - col.gameObject.transform.position.x) *
                              _playerController.airAttackPushback);
                }
            }
            else if (_playerController.isShieldAttacking)
            {
                AudioManager.Instance.PlayDamageSound();

                damageControl.GetDamaged(_playerController.shieldAttackDamage);
                damageControl.StopMoving();
                if (!col.transform.parent.CompareTag("Spawner"))
                {
                    Push(col, -Mathf.Sign(transform.position.x - col.gameObject.transform.position.x) *
                              _playerController.shieldAttackPushback);
                }
            }
            else if (_playerController.isBlocking)
            {
                damageControl.GetDamaged(0);
                damageControl.StopMoving();
                if (!col.transform.parent.CompareTag("Spawner"))
                {
                    Push(col, -Mathf.Sign(transform.position.x - col.gameObject.transform.position.x) *
                              _playerController.blockPushback);
                }
            }
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
        Vector2 boxSize = new Vector2(size.x, 0.1f);
        Vector2 direction = new Vector2(pushPower, 0f);
        return (!Physics2D.OverlapBox(center + direction, boxSize, 0f, LayerMask.GetMask("Ground")));
    }
}