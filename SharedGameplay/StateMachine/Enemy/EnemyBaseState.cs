using System.Collections.Generic;
using System.Linq;
using _Project.Systems._Core.StateMachine;
using _Project.Systems.CombatSystem.Enemy.States;
using UnityEngine;

namespace _Project.Systems.SharedGameplay.StateMachine.Enemy
{
    public abstract class EnemyBaseState : State
    {
        protected readonly EnemyStateMachine stateMachine;

        protected EnemyBaseState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        protected void Move(Vector3 movement, float deltaTime)
        {
            var motion = stateMachine.ForceReceiver.Movement;
            stateMachine.Controller.Move((motion + movement) * deltaTime);
        }

        protected void Move(float deltaTime)
        {
            Move(Vector3.zero, deltaTime);
        }

        protected void RotateToPlayer(float deltaTime)
        {
            Transform t = stateMachine.transform;
            GameObject currentTarget = stateMachine.EnemyPerceptionController.CurrentTarget;
            if (currentTarget)
            {
                Vector3 directionToPlayer = currentTarget.transform.position - stateMachine.transform.position;
                directionToPlayer.y = 0f;
                directionToPlayer.Normalize();

                var targetRot = Quaternion.LookRotation(directionToPlayer);
                t.rotation = Quaternion.Slerp(t.rotation, targetRot,
                    stateMachine.EnemyConfigSo.MovementData.RotationDampTime * deltaTime
                );
                return;
            }

            Vector3 directionToTarget = stateMachine.Agent.steeringTarget - stateMachine.transform.position;
            directionToTarget.y = 0f;
            directionToTarget.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            t.rotation = Quaternion.Slerp(t.rotation, targetRotation,
                stateMachine.EnemyConfigSo.MovementData.RotationDampTime * deltaTime
            );
        }

        protected void HandleBlocking(float deltaTime, bool allowBlocking = true)
        {
            if (stateMachine.CurrentState is EnemyDeadState or EnemyAttackingState or EnemyParryState)
            {
                return;
            }


            if (!stateMachine.ShieldHandler.CurrentShieldHitbox)
            {
                stateMachine.Animator.SetBool(stateMachine.EnemyConfigSo.CombatData.IsBlockingParamHash, false);
                return;
            }

            if (!stateMachine.ShieldHandler.CurrentShieldLogic)
            {
                stateMachine.Animator.SetBool(stateMachine.EnemyConfigSo.CombatData.IsBlockingParamHash, false);
                stateMachine.Animator.SetLayerWeight(stateMachine.BlockingLayerIndex,
                    0f
                );
                return;
            }


            bool wantsBlock = allowBlocking && stateMachine.EnemyDefenceBrain.canBlockAttack;

            if (wantsBlock)
            {
                stateMachine.ShieldHandler.EnableShield();
            }
            else
            {
                stateMachine.ShieldHandler.DisableShield();
            }

            stateMachine.Animator.SetBool(stateMachine.EnemyConfigSo.CombatData.IsBlockingParamHash, wantsBlock);

            float targetWeight = wantsBlock ? 1f : 0f;

            stateMachine.blockLayerWeight = Mathf.MoveTowards(
                stateMachine.blockLayerWeight,
                targetWeight,
                deltaTime * stateMachine.EnemyConfigSo.CombatData.BlockingLayerChangeSpeed
            );

            stateMachine.Animator.SetLayerWeight(stateMachine.BlockingLayerIndex,
                stateMachine.blockLayerWeight
            );
        }
    }
}