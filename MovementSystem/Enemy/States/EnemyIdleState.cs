using _Project.Systems.CombatSystem.Enemy.States;
using _Project.Systems.MovementSystem.ScriptableObjects;
using _Project.Systems.PerceptionSystem;
using _Project.Systems.SharedGameplay.StateMachine.Enemy;
using UnityEngine;

namespace _Project.Systems.MovementSystem.Enemy.States
{
    public class EnemyIdleState : EnemyBaseState
    {
        private float timer;
        private EnemyMovementDataSo data;

        public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            data = stateMachine.EnemyConfigSo.MovementData;
            timer = 0f;

            stateMachine.Animator.CrossFadeInFixedTime(data.LocomotionBlendTreeHash,
                data.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            
            timer += deltaTime;
            Move(deltaTime);
            HandleBlocking(deltaTime, true);

            EnemyPerceptionController perception = stateMachine.EnemyPerceptionController;

            bool hasCurrentTarget = perception.CurrentTarget;
            bool isInAttackRange = perception.IsTargetInAttackRange;
            bool hasSuspiciousTarget = perception.HasSuspiciousTarget;

            Vector3 lastKnownPos = perception.LastKnownTargetPos;
            float distanceToSuspicious = Vector3.Distance(stateMachine.transform.position, lastKnownPos);

            if (isInAttackRange && hasCurrentTarget)
            {
                stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
                return;
            }

            //? if enemy fought or chased the target, then chase immediately
            //? if didn't, then goes suspicious for searching enemy

            if (hasCurrentTarget)
            {
                if (perception.IsAggressive)
                {
                    stateMachine.SwitchState(new EnemyChaseState(stateMachine));
                }
                else
                {
                    stateMachine.SwitchState(new EnemySuspiciousState(stateMachine,
                        perception.CurrentTarget.transform.position));
                }

                return;
            }

            //? Rocks arent have "Current Target", but they can be suspicious targets, so we need to check them separately
            //? If a rock hits some wall, the enemy goes to that point with suspicious state, not with chase state
            if (hasSuspiciousTarget && distanceToSuspicious > 1f)
            {
                stateMachine.SwitchState(new EnemySuspiciousState(stateMachine, lastKnownPos));
                return;
            }

            if (timer >= stateMachine.EnemyConfigSo.MovementData.PatrolCooldown)
            {
                stateMachine.SwitchState(new EnemyPatrolState(stateMachine));
            }

            stateMachine.Animator.SetFloat(data.FreeLookSpeedParamHash, 0f,
                stateMachine.EnemyConfigSo.MovementData.LocomotionAnimatorDampTime,
                deltaTime);
        }

        public override void Exit()
        {
        }
    }
}