using _Project.Systems.CombatSystem.Enemy.States;
using _Project.Systems.MovementSystem.ScriptableObjects;
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
            
            if (IsInChaseRange() && !IsInAttackRange())
            {
                stateMachine.SwitchState(new EnemyChaseState(stateMachine));
                return;
            }

            if (IsInAttackRange())
            {
                stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
                return;
            }

            if (timer >= stateMachine.EnemyConfigSo.MovementData.patrolCooldown)
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