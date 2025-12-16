using _Project.Systems.PlayerControllerSystem.Combat;
using UnityEngine;

namespace _Project.Systems.PlayerControllerSystem.StateMachines.Player
{
    public class PlayerAttackingState : PlayerBaseState
    {
        private readonly AttackDataSo attack;
        private float previousFrameTime;
        private bool alreadyAppliedForce = false;

        public PlayerAttackingState(PlayerStateMachine stateMachine, int attackIndex) : base(stateMachine)
        {
            attack = stateMachine.Attacks[attackIndex];
        }

        public override void Enter()
        {
            stateMachine.WeaponLogic.SetAttackAttributes(attack.DamageMultiplier);
            stateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName, attack.TransitionDuration);
        }

        public override void Tick(float deltaTime)
        {
            // Move(deltaTime, attack.AttackForceTargetPos, attack.AttackForce);
            Move(deltaTime);
            FaceTarget(stateMachine.Targeter.SelectedTarget, deltaTime);

            float normalizedTime = GetNormalizedTime();
            if (normalizedTime >= previousFrameTime && normalizedTime < 1f)
            {
                if (normalizedTime >= attack.ForceTime)
                {
                    TryApplyForce();
                }

                if (stateMachine.InputHandler.IsAttacking)
                {
                    TryComboAttack(normalizedTime);
                }
            }
            else
            {
                if (stateMachine.Targeter.SelectedTarget)
                {
                    stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
                }
                else
                {
                    stateMachine.SwitchState(new PlayerFreeLookState(stateMachine));
                }
            }

            previousFrameTime = normalizedTime;
        }


        public override void Exit()
        {
        }

        private float GetNormalizedTime()
        {
            AnimatorStateInfo currentInfo = stateMachine.Animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextInfo = stateMachine.Animator.GetNextAnimatorStateInfo(0);
            if (stateMachine.Animator.IsInTransition(0) && nextInfo.IsTag("SwordAttack"))
            {
                return nextInfo.normalizedTime;
            }
            else if (!stateMachine.Animator.IsInTransition(0) && currentInfo.IsTag("SwordAttack"))
            {
                return currentInfo.normalizedTime;
            }
            else
            {
                return 0f;
            }
        }

        private void TryComboAttack(float normalizedTime)
        {
            if (attack.ComboStateIndex == -1) return;
            if (normalizedTime < attack.ComboAttackTime) return;

            stateMachine.SwitchState(
                new PlayerAttackingState(
                    stateMachine,
                    attack.ComboStateIndex));
        }

        private void TryApplyForce()
        {
            if (alreadyAppliedForce) return;
            stateMachine.ForceReceiver.AddForce(stateMachine.transform.forward * attack.AttackForce);
            alreadyAppliedForce = true;
        }
    }
}