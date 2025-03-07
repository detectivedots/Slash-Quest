using System;
using UnityEngine;

public class AnimationEventsHandler : MonoBehaviour
{
    private void AttackTransitionFinishedTrigger()
    {
        PlayerController.Instance.Transition();
    }

    private void StopAttack()
    {
        PlayerController.Instance.SetAttackToFalse();
    }

    private void StopAirAttack()
    {
        PlayerController.Instance.SetAirAttackToFalse();
    }

    private void StopShieldAttack()
    {
        PlayerController.Instance.SetShieldAttackToFalse();
    }

    private void AttackForceReset()
    {
        PlayerController.Instance.UnfreezeAttack();
    }
}
