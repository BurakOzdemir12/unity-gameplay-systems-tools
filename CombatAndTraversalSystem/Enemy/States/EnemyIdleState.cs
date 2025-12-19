using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Enemy.States
{
    public class EnemyIdleState : EnemyBaseState
    {
        public EnemyIdleState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.LocomotionBlendTreeHash,
                stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
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

            stateMachine.Animator.SetFloat(stateMachine.MoveSpeedParamHash, 0f,
                stateMachine.LocomotionAnimationDampTime,
                deltaTime);
        }

        public override void Exit()
        {
        }
    }
}