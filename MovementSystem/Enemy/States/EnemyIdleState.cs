using _Project.Systems._Core.BaseScriptableObjects.Characters;
using _Project.Systems._Core.StateMachine.Enemy;
using _Project.Systems.CombatSystem.Enemy.States;
using UnityEngine;

namespace _Project.Systems.MovementSystem.Enemy.States
{
    public class EnemyIdleState : EnemyBaseState
    {
        public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        private float timer;

        public override void Enter()
        {
            timer = 0f;

            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.LocomotionBlendTreeHash,
                stateMachine.EnemyConfigSo.MovementData.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            timer += deltaTime;
            Move(deltaTime);


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

            stateMachine.Animator.SetFloat(stateMachine.MoveSpeedParamHash, 0f,
                stateMachine.EnemyConfigSo.MovementData.LocomotionAnimatorDampTime,
                deltaTime);
        }

        public override void Exit()
        {
        }
    }
}