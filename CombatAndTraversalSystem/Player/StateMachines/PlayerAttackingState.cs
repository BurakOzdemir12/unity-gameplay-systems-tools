using _Project.Systems.CombatAndTraversalSystem.Player.Combat;
using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Player.StateMachines
{
    public class PlayerAttackingState : PlayerBaseState
    {
        private readonly AttackDataSo attack;
        private float previousFrameTime;
        private bool alreadyAppliedForce = false;
        private const string ATTACK_TAG = "Attack";

        public PlayerAttackingState(PlayerStateMachine stateMachine, int attackIndex) : base(stateMachine)
        {
            attack = stateMachine.Attacks[attackIndex];
        }

        public override void Enter()
        {
            // stateMachine.WeaponLogic.SetAttackAttributes(attack.DamageMultiplier, attack.KnockBackForce,
            //     stateMachine.attackDamage);
            float finalDamage = stateMachine.AttackDamage * attack.DamageMultiplier;
            float finalKnockbackForce = attack.KnockBackForce;

            stateMachine.WeaponLogic.BeginAttack(finalDamage, finalKnockbackForce);

            stateMachine.Animator.CrossFadeInFixedTime(attack.AnimationName, attack.TransitionDuration);
        }

        public override void Tick(float deltaTime)
        {
            // Move(deltaTime, attack.AttackForceTargetPos, attack.AttackForce);
            Move(deltaTime);
            // FaceTarget(stateMachine.Targeter.SelectedTarget, deltaTime);
            FaceAttackToLook(deltaTime);

            float normalizedTime = GetNormalizedTime(stateMachine.Animator, ATTACK_TAG);

            if (normalizedTime >= previousFrameTime && normalizedTime < 1f)
            {
                if (normalizedTime >= attack.ForceTime) //TODO Create Event action for this in scriptable object
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
                DecideTargetOrLocomotion();
            }

            previousFrameTime = normalizedTime;
        }

        public override void Exit()
        {
            stateMachine.WeaponLogic.EndAttack();
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