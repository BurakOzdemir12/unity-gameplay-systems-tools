using _Project.Systems.CombatAndTraversalSystem.Player.StateMachines;
using UnityEngine;

namespace _Project.Systems.CombatAndTraversalSystem.Enemy.States
{
    public class EnemyChaseState : EnemyBaseState
    {
        public EnemyChaseState(EnemyStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Enter()
        {
            stateMachine.Agent.isStopped = false;

            stateMachine.Animator.CrossFadeInFixedTime(stateMachine.LocomotionBlendTreeHash,
                stateMachine.CrossFadeDuration);
        }

        public override void Tick(float deltaTime)
        {
            if (!IsInChaseRange())
            {
                stateMachine.SwitchState(new EnemyIdleState(stateMachine));
                return;
            }

            if (IsInAttackRange())
            {
                stateMachine.SwitchState(new EnemyAttackingState(stateMachine));
                return;
            }

            MoveToPlayer(deltaTime);
            stateMachine.Animator.SetFloat(stateMachine.MoveSpeedParamHash, 1f,
                stateMachine.locomotionAnimationDampTime,
                deltaTime);
        }

        public override void Exit()
        {
            stateMachine.Agent.ResetPath();
            stateMachine.Agent.isStopped = true;
        }

        private void MoveToPlayer(float deltaTime)
        {
            if (stateMachine.Player == null) return;

            Vector3 detectedPlayerPos = stateMachine.Player.transform.position;
            stateMachine.Agent.SetDestination(detectedPlayerPos);

            Transform enemyT = stateMachine.transform;
            Vector3 to = stateMachine.Agent.steeringTarget - enemyT.position;
            to.y = 0f;

            Vector3 dir = to.sqrMagnitude > 0.001f ? to.normalized : Vector3.zero;

            Move(dir * stateMachine.MoveSpeed, deltaTime);

            RotateToPlayer(deltaTime);
            // stateMachine.Agent.velocity = stateMachine.Controller.velocity;
            stateMachine.Agent.nextPosition = stateMachine.transform.position;
        }
    }
}